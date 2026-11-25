// Slot Bindings through the resolver seam, on synthetic vtables: Gamedata recorded on one build, resolved on another.
#include <doctest.h>

#include <cstdint>
#include <string>
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
constexpr const char* kKey = "ISource2Server::GameFrame";
constexpr const char* kClass = "CSource2Server";

// A distinct function body per id: three pushes (registers spell the id in octal) and ret.
std::vector<std::uint8_t> Body(int id)
{
    return { static_cast<std::uint8_t>(0x50 + ((id >> 6) & 7)), static_cast<std::uint8_t>(0x50 + ((id >> 3) & 7)),
             static_cast<std::uint8_t>(0x50 + (id & 7)), 0xC3 };
}

// One build of a module with one class: its vtable holds the functions with these body ids, in order.
struct FakeBuild
{
    std::vector<std::uint8_t> code;
    std::vector<std::uintptr_t> vtable;
    std::size_t vtableMatches = 1; // primary vtables RTTI finds for the class

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
        image.findVirtualTable = [this](std::string_view name) -> rtti::VtableLookup {
            if (name != kClass) return {};
            return { vtableMatches == 1 ? kVtableAddress : 0, vtableMatches };
        };
        image.readVtableSlot = [this](std::uintptr_t address, std::size_t index) -> std::optional<std::uintptr_t> {
            if (address != kVtableAddress || index > vtable.size()) return std::nullopt;
            return index < vtable.size() ? vtable[index] : 0; // a zero word ends the vtable
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

// The Gamedata entry the generator records for slot index of build (Linux only, like the generator's bootstrap).
json SlotEntry(const FakeBuild& build, std::size_t index)
{
    const auto record = vtable::RecordSlot(build.Fingerprints(), index);
    return { { "offsets", { { "linux", index } } },
             { "vtable",
               { { "class", kClass },
                 { "library", "server" },
                 { "linux", { { "window", WindowJson(record.window) }, { "uniqueNearby", record.uniqueNearby }, { "size", *record.vtableSize } } } } } };
}

BindingResult Resolve(const json& gamedata, const FakeBuild& live, Platform platform = Platform::Linux, const char* os = "linux")
{
    const BuiltinResolver builtin(platform);
    const IBindingResolver& resolver = builtin;
    const std::vector<BindingDeclaration> declarations{ { kKey, BindingKind::Slot } };
    const auto report = resolver.Resolve("test-build", ParseGamedata(gamedata, os), { { "server", live.Image() } }, declarations);
    REQUIRE(report.Bindings().size() == 1);
    return report.Bindings()[0];
}

const std::vector<int> kOldBuild{ 1, 2, 3, 4, 5, 6, 7, 8 };
} // namespace

TEST_CASE("A Slot Binding whose window matches the live vtable resolves at its Gamedata index")
{
    const FakeBuild build(kOldBuild);

    const auto result = Resolve({ { kKey, SlotEntry(build, 4) } }, build);

    CHECK(result.status == BindingStatus::Resolved);
    CHECK(result.reason == FailureReason::None);
    CHECK(result.kind == BindingKind::Slot);
    CHECK(result.offset == 4);
    CHECK(result.library == "server");
    REQUIRE(result.slot);
    CHECK(result.slot->className == kClass);
    CHECK(result.slot->statedIndex == 4);
    CHECK(result.slot->match == vtable::SlotMatch::Window);
    CHECK(result.slot->vtableSize == 8);
    CHECK_FALSE(result.slot->SizeChanged());
}

TEST_CASE("A Slot Binding whose window does not match fails with fingerprint mismatch")
{
    const json gamedata{ { kKey, SlotEntry(FakeBuild(kOldBuild), 4) } };

    SUBCASE("the slot's function changed")
    {
        const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 3, 4, 99, 6, 7, 8 }));
        CHECK(result.status == BindingStatus::Failed);
        CHECK(result.reason == FailureReason::FingerprintMismatch);
        CHECK(DescribeFailure(result) == "fingerprint mismatch");
    }
    SUBCASE("a virtual was inserted before the slot (no Heal: the Binding fails instead of using the wrong function)")
    {
        const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 3, 50, 4, 5, 6, 7, 8 }));
        CHECK(result.reason == FailureReason::FingerprintMismatch);
    }
    SUBCASE("the vtable is shorter than the slot")
    {
        const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 3 }));
        CHECK(result.reason == FailureReason::FingerprintMismatch);
        CHECK(result.slot->vtableSize == 3);
    }
}

TEST_CASE("A vtable size change alone resolves with a warning")
{
    const json gamedata{ { kKey, SlotEntry(FakeBuild(kOldBuild), 4) } };
    const FakeBuild grown({ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

    const BuiltinResolver resolver(Platform::Linux);
    const std::vector<BindingDeclaration> declarations{ { kKey, BindingKind::Slot } };
    const auto report = resolver.Resolve("test-build", ParseGamedata(gamedata, "linux"), { { "server", grown.Image() } }, declarations);

    const auto& result = report.Bindings().at(0);
    CHECK(result.status == BindingStatus::Resolved);
    CHECK(result.offset == 4);
    REQUIRE(result.slot);
    CHECK(result.slot->SizeChanged());
    CHECK(result.slot->vtableSize == 10);
    CHECK(result.slot->recordedVtableSize == 8u);
    CHECK(FormatBindingSummary(report) == "Bindings for build test-build: all 1 resolved");
    const auto warnings = FormatVtableSizeWarnings(report);
    REQUIRE(warnings.size() == 1);
    CHECK(warnings[0] == "Vtable of CSource2Server (server) has 10 slots, Gamedata recorded 8 (ISource2Server::GameFrame)");
}

TEST_CASE("A changed neighbour does not fail a slot whose own fingerprint is unique nearby on both builds")
{
    const json gamedata{ { kKey, SlotEntry(FakeBuild(kOldBuild), 4) } };

    const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 3, 4, 5, 60, 7, 8 }));

    CHECK(result.status == BindingStatus::Resolved);
    CHECK(result.offset == 4);
    CHECK(result.slot->match == vtable::SlotMatch::OwnFingerprint);
}

TEST_CASE("Slots at either end of the vtable resolve: a neighbour outside the vtable matches anything")
{
    const FakeBuild build(kOldBuild);
    CHECK(Resolve({ { kKey, SlotEntry(build, 0) } }, build).slot->match == vtable::SlotMatch::Window);
    CHECK(Resolve({ { kKey, SlotEntry(build, 7) } }, build).slot->match == vtable::SlotMatch::Window); // neighbours outside
}

TEST_CASE("A Slot Binding fails when its class's vtable cannot be found by RTTI")
{
    FakeBuild build(kOldBuild);
    auto gamedata = json{ { kKey, SlotEntry(build, 4) } };

    SUBCASE("unknown class")
    {
        gamedata[kKey]["vtable"]["class"] = "CNoSuchClass";
        CHECK(Resolve(gamedata, build).reason == FailureReason::RttiVtableNotFound);
    }
    SUBCASE("two primary vtables")
    {
        build.vtableMatches = 2;
        const auto result = Resolve(gamedata, build);
        CHECK(result.reason == FailureReason::Ambiguous);
        CHECK(result.matchCount == 2);
    }
    SUBCASE("module not loaded")
    {
        gamedata[kKey]["vtable"]["library"] = "engine";
        CHECK(Resolve(gamedata, build).reason == FailureReason::NotFound);
    }
}

TEST_CASE("A Slot Binding missing from Gamedata, or without an index for this OS, fails like an offset")
{
    const FakeBuild build(kOldBuild);
    CHECK(Resolve(json::object(), build).reason == FailureReason::MissingFromGamedata);

    auto gamedata = json{ { kKey, SlotEntry(build, 4) } };
    gamedata[kKey]["offsets"].erase("linux");
    CHECK(Resolve(gamedata, build).reason == FailureReason::MissingForThisOs);
}

TEST_CASE("A Slot Binding without a fingerprint window is unverified at its Gamedata index")
{
    // e.g. Gamedata from before Slot Bindings (an auto-updated file): used as before, reported, never failed.
    const FakeBuild build(kOldBuild);

    const auto result = Resolve({ { kKey, { { "offsets", { { "linux", 4 } } } } } }, build);

    CHECK(result.status == BindingStatus::Unverified);
    CHECK(result.Resolved());
    CHECK(result.offset == 4);
}

TEST_CASE("On Windows a Slot Binding is unverified at its Gamedata index, never checked")
{
    auto gamedata = json{ { kKey, SlotEntry(FakeBuild(kOldBuild), 4) } };
    gamedata[kKey]["offsets"]["windows"] = 3;
    const FakeBuild changed({ 9, 9, 9 }); // would fail any check

    const auto result = Resolve(gamedata, changed, Platform::Windows, "windows");
    CHECK(result.status == BindingStatus::Unverified);
    CHECK(result.offset == 3);

    SUBCASE("without a Gamedata index it keeps its header index (-1: the caller's compiled one)")
    {
        gamedata[kKey]["offsets"].erase("windows");
        const auto headerIndexed = Resolve(gamedata, changed, Platform::Windows, "windows");
        CHECK(headerIndexed.status == BindingStatus::Unverified);
        CHECK(headerIndexed.offset == -1);
        CHECK(Resolve(json::object(), changed, Platform::Windows, "windows").status == BindingStatus::Unverified);
    }
}

TEST_CASE("Unverified Slot Bindings count as available and are counted in the summary")
{
    const FakeBuild build(kOldBuild);
    const json gamedata{ { kKey, { { "offsets", { { "linux", 4 } } } } } };
    const EngineFeatureDeclaration feature{ "cssharp.server/game-frame", { { kKey, BindingKind::Slot } } };

    const auto report = ResolveBindings("b", ParseGamedata(gamedata, "linux"), { { "server", build.Image() } }, feature.bindings);

    CHECK(EvaluateEngineFeature(report, feature).available);
    CHECK(FormatBindingSummary(report) == "Bindings for build b: all 1 resolved, 1 unverified");
}

TEST_CASE("The report carries a Slot Binding's status and Vtable Check evidence to managed")
{
    const FakeBuild build(kOldBuild);
    const std::vector<BindingDeclaration> declarations{ { kKey, BindingKind::Slot } };
    const auto report = ResolveBindings("b", ParseGamedata({ { kKey, SlotEntry(build, 4) } }, "linux"), { { "server", build.Image() } },
                                        declarations);

    const auto binding = json::parse(ResolutionReportToJson(report, {}))["bindings"][0];

    CHECK(binding["kind"] == "Slot");
    CHECK(binding["status"] == "Resolved");
    CHECK(binding["resolved"] == true);
    CHECK(binding["offset"] == 4);
    CHECK(binding["slot"]["class"] == kClass);
    CHECK(binding["slot"]["statedIndex"] == 4);
    CHECK(binding["slot"]["match"] == "Window");
    CHECK(binding["slot"]["vtableSize"] == 8);
    CHECK(binding["slot"]["recordedVtableSize"] == 8);
}

TEST_CASE("Gamedata Slot Binding fields are additive: the offset reads as before")
{
    const auto entry = SlotEntry(FakeBuild(kOldBuild), 4);

    const auto parsed = ParseGamedata({ { kKey, entry } }, "linux").at(kKey);
    CHECK(parsed.hasOffsets);
    CHECK(parsed.offset == 4);
    REQUIRE(parsed.slot);
    CHECK(parsed.slot->className == kClass);
    CHECK(parsed.slot->library == "server");
    REQUIRE(parsed.slot->window);
    CHECK(parsed.slot->vtableSize == 8u);

    // Declared as an offset (e.g. by a reader that does not know Slot Bindings), it still resolves to the index.
    const std::vector<BindingDeclaration> asOffset{ { kKey, BindingKind::Offset } };
    const auto report = ResolveBindings("b", ParseGamedata({ { kKey, entry } }, "linux"), {}, asOffset);
    CHECK(report.Bindings()[0].Resolved());
    CHECK(report.Bindings()[0].offset == 4);
}

TEST_CASE("Gamedata keys with a vtable are declared as Slot Bindings")
{
    const auto gamedata = ParseGamedata({ { "Some_Slot", SlotEntry(FakeBuild(kOldBuild), 4) },
                                          { "Some_Offset", { { "offsets", { { "linux", 1 } } } } } },
                                        "linux");

    const auto declarations = CoreBindingDeclarations(gamedata);

    const auto kindOf = [&](std::string_view key) {
        const auto it = std::find_if(declarations.begin(), declarations.end(), [&](const BindingDeclaration& d) {
            return d.key == key;
        });
        REQUIRE(it != declarations.end());
        return it->kind;
    };
    CHECK(kindOf("Some_Slot") == BindingKind::Slot);
    CHECK(kindOf("Some_Offset") == BindingKind::Offset);
}
