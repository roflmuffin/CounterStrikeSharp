// Interfaces that never shifted a slot we use (game event manager, game event system, network server service, engine
// service manager, network game server): their slots are Slot Bindings verified at boot, but their direct calls stay
// compiled. A compiled call cannot follow a Heal, so a Binding verified at another index than the compiled one fails.
#include <doctest.h>

#include <cstdint>
#include <fstream>
#include <string>
#include <string_view>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;
using namespace counterstrikesharp;
using nlohmann::json;

namespace {
constexpr std::uintptr_t kCodeAddress = 0x500000;
constexpr std::uintptr_t kVtableAddress = 0x900000;
constexpr std::uintptr_t kPureVirtual = 0x42;
constexpr const char* kKey = "IGameEventManager2::FireEvent";
constexpr const char* kClass = "CGameEventManager";

// A distinct function body per id: three pushes (registers spell the id in octal) and ret.
std::vector<std::uint8_t> Body(int id)
{
    return { static_cast<std::uint8_t>(0x50 + ((id >> 6) & 7)), static_cast<std::uint8_t>(0x50 + ((id >> 3) & 7)),
             static_cast<std::uint8_t>(0x50 + (id & 7)), 0xC3 };
}

// One build of the server module whose CGameEventManager vtable holds the functions with these body ids, in order.
struct FakeBuild
{
    std::vector<std::uint8_t> code;
    std::vector<std::uintptr_t> vtable;

    explicit FakeBuild(const std::vector<int>& bodies)
    {
        for (int id : bodies)
        {
            vtable.push_back(kCodeAddress + code.size());
            const auto body = Body(id);
            code.insert(code.end(), body.begin(), body.end());
        }
    }

    ModuleImage Image() const
    {
        ModuleImage image;
        image.segments.push_back({ kCodeAddress, code.data(), code.size() });
        image.findVirtualTable = [](std::string_view name) -> rtti::VtableLookup {
            if (name != kClass) return {};
            return { kVtableAddress, 1 };
        };
        image.readVtableSlot = [this](std::uintptr_t address, std::size_t index) -> std::optional<std::uintptr_t> {
            if (address != kVtableAddress || index > vtable.size()) return std::nullopt;
            return index < vtable.size() ? vtable[index] : 0;
        };
        image.pureVirtual = kPureVirtual;
        return image;
    }

    std::vector<vtable::Fingerprint> Fingerprints() const
    {
        const auto image = Image();
        return vtable::FingerprintVtable({ [&](std::size_t index) {
                                              return image.readVtableSlot(kVtableAddress, index);
                                          },
                                           image.segments, kPureVirtual, 0 });
    }
};

json WindowJson(const vtable::FingerprintWindow& window)
{
    auto slots = json::array();
    for (const auto& fingerprint : window.slots)
        slots.push_back(fingerprint ? json(*fingerprint) : json(nullptr));
    return slots;
}

// The Gamedata entry the generator records for slot index of build.
json SlotEntry(const FakeBuild& build, std::size_t index)
{
    const auto record = vtable::RecordSlot(build.Fingerprints(), index);
    return { { "offsets", { { "linux", index } } },
             { "vtable",
               { { "class", kClass },
                 { "library", "server" },
                 { "linux", { { "window", WindowJson(record.window) }, { "uniqueNearby", record.uniqueNearby }, { "size", *record.vtableSize } } } } } };
}

// FireEvent as a compiled call: built against headers that put it at compiledIndex.
ResolutionReport Resolve(const json& gamedata, const FakeBuild& live, std::int64_t compiledIndex, Platform platform = Platform::Linux,
                         const char* os = "linux")
{
    const std::vector<BindingDeclaration> declarations{ { kKey, BindingKind::Slot, "", compiledIndex } };
    return BuiltinResolver(platform).Resolve("test-build", ParseGamedata(gamedata, os), { { "server", live.Image() } }, declarations);
}

const EngineFeatureDeclaration kFeature{ "cssharp.test/compiled-call", { { kKey, BindingKind::Slot } } };

std::vector<int> Bodies(int count)
{
    std::vector<int> bodies;
    for (int id = 1; id <= count; ++id)
        bodies.push_back(id);
    return bodies;
}

std::vector<int> Inserted(std::vector<int> bodies, std::size_t position)
{
    bodies.insert(bodies.begin() + static_cast<std::ptrdiff_t>(position), 100);
    return bodies;
}
} // namespace

TEST_CASE("A compiled call's Slot Binding verified at its compiled index resolves")
{
    const FakeBuild build(Bodies(20));

    const auto report = Resolve({ { kKey, SlotEntry(build, 8) } }, build, 8);

    const auto& result = report.Bindings().at(0);
    CHECK(result.status == BindingStatus::Resolved);
    CHECK(result.offset == 8);
    CHECK(EvaluateEngineFeature(report, kFeature).available);
}

TEST_CASE("A compiled call's Slot Binding healed onto its compiled index stays available")
{
    // Gamedata says 7 but the window sits at 8, where the compiled call goes: the compiled call is right.
    const FakeBuild build(Bodies(20));
    auto entry = SlotEntry(build, 8);
    entry["offsets"]["linux"] = 7;

    const auto report = Resolve({ { kKey, entry } }, build, 8);

    const auto& result = report.Bindings().at(0);
    CHECK(result.status == BindingStatus::Healed);
    CHECK(result.offset == 8);
    CHECK(EvaluateEngineFeature(report, kFeature).available);
}

TEST_CASE("A compiled call's Slot Binding healed away from its compiled index fails: the compiled call cannot follow")
{
    // A virtual inserted before FireEvent: the window heals at 9, the compiled call would still call slot 8.
    const FakeBuild generation(Bodies(20));
    const FakeBuild live(Inserted(Bodies(20), 3));

    const auto report = Resolve({ { kKey, SlotEntry(generation, 8) } }, live, 8);

    const auto& result = report.Bindings().at(0);
    CHECK(result.status == BindingStatus::Failed);
    CHECK(result.reason == FailureReason::CompiledSlotMoved);
    CHECK_FALSE(result.Resolved());
    // The evidence of where the slot went stays in the report.
    CHECK(result.offset == 9);
    REQUIRE(result.slot);
    CHECK(result.slot->statedIndex == 8);
    CHECK(result.slot->match == vtable::SlotMatch::Heal);

    CHECK(DescribeFailure(result) == "compiled call's slot moved");
    CHECK_FALSE(EvaluateEngineFeature(report, kFeature).available);
    CHECK(DescribeUnavailable(report, kFeature) ==
          "Engine Feature cssharp.test/compiled-call is disabled (IGameEventManager2::FireEvent: compiled call's slot moved)");
    CHECK(FormatHeals(report).empty());
    CHECK(FormatBindingSummary(report) ==
          "Bindings for build test-build: 1 of 1 failed: IGameEventManager2::FireEvent (compiled call's slot moved)");
    const auto wire = json::parse(ResolutionReportToJson(report, {}));
    CHECK(wire["bindings"][0]["reason"] == "CompiledSlotMoved");
    CHECK(wire["bindings"][0]["status"] == "Failed");
}

TEST_CASE("A compiled call's Slot Binding verified at a Gamedata index other than the compiled one fails")
{
    // Gamedata regenerated for a build where FireEvent moved to 9; the plugin was compiled with 8.
    const FakeBuild live(Inserted(Bodies(20), 3));

    const auto report = Resolve({ { kKey, SlotEntry(live, 9) } }, live, 8);

    const auto& result = report.Bindings().at(0);
    CHECK(result.status == BindingStatus::Failed);
    CHECK(result.reason == FailureReason::CompiledSlotMoved);
    CHECK(result.offset == 9);
    CHECK(result.slot->match == vtable::SlotMatch::Window);
}

TEST_CASE("A hooked-only Slot Binding (no compiled call) follows a Heal")
{
    const FakeBuild generation(Bodies(20));
    const FakeBuild live(Inserted(Bodies(20), 3));
    const std::vector<BindingDeclaration> declarations{ { kKey, BindingKind::Slot } };

    const auto report = BuiltinResolver(Platform::Linux)
                            .Resolve("test-build", ParseGamedata({ { kKey, SlotEntry(generation, 8) } }, "linux"),
                                     { { "server", live.Image() } }, declarations);

    CHECK(report.Bindings().at(0).status == BindingStatus::Healed);
    CHECK(report.Bindings().at(0).offset == 9);
}

TEST_CASE("Unverified Slot Bindings of compiled calls are not compared with the compiled index")
{
    const FakeBuild build(Bodies(20));

    SUBCASE("Windows: Gamedata index, no Vtable Check")
    {
        json entry = SlotEntry(build, 8);
        entry["offsets"]["windows"] = 9;
        const auto report = Resolve({ { kKey, entry } }, build, 8, Platform::Windows, "windows");
        CHECK(report.Bindings().at(0).status == BindingStatus::Unverified);
        CHECK(EvaluateEngineFeature(report, kFeature).available);
    }

    SUBCASE("Linux without a window: Gamedata index as it is")
    {
        const json entry = { { "offsets", { { "linux", 8 } } }, { "vtable", { { "class", kClass }, { "library", "server" } } } };
        const auto report = Resolve({ { kKey, entry } }, build, 8);
        CHECK(report.Bindings().at(0).status == BindingStatus::Unverified);
        CHECK(report.Bindings().at(0).offset == 8);
        CHECK(EvaluateEngineFeature(report, kFeature).available);
    }
}

TEST_CASE("A failed compiled call's Slot Binding keeps its own failure reason")
{
    const FakeBuild generation(Bodies(20));
    auto entry = SlotEntry(generation, 8);
    entry["vtable"]["linux"]["window"][1] = "0000000000000000";

    const auto report = Resolve({ { kKey, entry } }, generation, 8);

    CHECK(report.Bindings().at(0).reason == FailureReason::FingerprintMismatch);
}

// --- The catalog ---

namespace {
std::vector<std::string> KeysOf(std::string_view feature)
{
    const auto* declaration = FindNativeEngineFeature(feature);
    REQUIRE_MESSAGE(declaration != nullptr, feature);
    std::vector<std::string> keys;
    for (const auto& binding : declaration->bindings)
        keys.push_back(binding.key);
    return keys;
}

bool Declares(std::string_view feature, std::string_view key)
{
    const auto* declaration = FindNativeEngineFeature(feature);
    REQUIRE_MESSAGE(declaration != nullptr, feature);
    for (const auto& binding : declaration->bindings)
        if (binding.key == key) return binding.kind == BindingKind::Slot;
    return false;
}
} // namespace

TEST_CASE("The catalog declares the never-moved interfaces' Engine Features on their Slot Bindings")
{
    CHECK(features::Startup == "cssharp.server/startup");
    CHECK(KeysOf(features::Startup) == std::vector<std::string>{ "INetworkServerService::StartupServer" });
    CHECK(Declares(features::Startup, "INetworkServerService::StartupServer"));

    CHECK(features::LoopMode == "cssharp.server/loop-mode");
    CHECK(KeysOf(features::LoopMode) == std::vector<std::string>{ "IEngineServiceMgr::RegisterLoopMode", "IEngineServiceMgr::FindService" });

    CHECK(features::UserMessages == "cssharp.network/user-messages");
    CHECK(KeysOf(features::UserMessages) ==
          std::vector<std::string>{ "IGameEventSystem::PostEventAbstract", "IGameEventSystem::PostEventAbstract_Filter" });

    CHECK(features::GlobalVars == "cssharp.server/global-vars");
    CHECK(KeysOf(features::GlobalVars) == std::vector<std::string>{ "INetworkServerService::GetIGameServer", "INetworkGameServer::GetGlobals" });

    for (const auto* feature : { &features::Startup, &features::LoopMode, &features::UserMessages, &features::GlobalVars })
        for (const auto& key : KeysOf(*feature))
            CHECK_MESSAGE(Declares(*feature, key), key);
}

TEST_CASE("game-events also needs every game event manager slot it calls or hooks, and the loop-mode hook that flushes "
          "early event hooks")
{
    for (const char* key : { "IGameEventManager2::LoadEventsFromFile", "IGameEventManager2::AddListener", "IGameEventManager2::FindListener",
                             "IGameEventManager2::RemoveListener", "IGameEventManager2::CreateEvent", "IGameEventManager2::FireEvent",
                             "IGameEventManager2::FreeEvent", "IEngineServiceMgr::RegisterLoopMode" })
        CHECK_MESSAGE(Declares(features::GameEvents, key), key);
}

TEST_CASE("game-frame also needs the global vars its handler reads (tick count, current time)")
{
    CHECK(Declares(features::GameFrame, "INetworkServerService::GetIGameServer"));
    CHECK(Declares(features::GameFrame, "INetworkGameServer::GetGlobals"));
}

// --- The committed Gamedata ---

namespace {
json CommittedGamedataJson()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return json::parse(stream);
}

// Interfaces in modules the build archive does not hold (networksystem) or the boot does not fingerprint
// (schemasystem): Slot Bindings with no window, so they are reported unverified.
const std::vector<const char*> kUnverifiedKeys{ "INetworkMessages::FindNetworkMessagePartial", "INetworkMessages::FindNetworkMessageById",
                                                "CSchemaSystem::FindTypeScopeForModule", "CSchemaSystem::GlobalTypeScope" };
} // namespace

TEST_CASE("Committed Gamedata: every never-moved Slot Binding has a Linux window, class and library")
{
    const auto gamedata = CommittedGamedataJson();
    for (const char* feature : { "cssharp.server/startup", "cssharp.server/loop-mode", "cssharp.network/user-messages",
                                 "cssharp.server/global-vars", "cssharp.events/game-events" })
    {
        for (const auto& binding : FindNativeEngineFeature(feature)->bindings)
        {
            if (binding.kind != BindingKind::Slot) continue;
            CAPTURE(binding.key);
            REQUIRE(gamedata.contains(binding.key));
            const auto& entry = gamedata[binding.key];
            CHECK(entry["offsets"]["linux"].is_number_integer());
            CHECK(entry["vtable"]["class"].is_string());
            CHECK(entry["vtable"]["library"].is_string());
            CHECK(entry["vtable"]["linux"]["window"].is_array());
        }
    }
}

TEST_CASE("Committed Gamedata: network messages and the schema system are reported as unverified Slot Bindings")
{
    const auto gamedata = ParseGamedata(CommittedGamedataJson(), "linux");
    const auto declarations = CoreBindingDeclarations(gamedata);

    // No modules: an unverified Slot Binding never needs one.
    const auto report = BuiltinResolver(Platform::Linux).Resolve("test-build", gamedata, {}, declarations);

    for (const char* key : kUnverifiedKeys)
    {
        CAPTURE(key);
        const auto* binding = report.Find(key);
        REQUIRE(binding != nullptr);
        CHECK(binding->kind == BindingKind::Slot);
        CHECK(binding->status == BindingStatus::Unverified);
        CHECK(binding->offset >= 0);
        REQUIRE(binding->slot);
        CHECK_FALSE(binding->slot->className.empty());
    }
}
