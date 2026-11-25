// The ISource2Server Engine Features and their Slot Bindings, through the native catalog and the resolver seam, on a
// synthetic CSource2Server vtable laid out like the real one (the real builds are in archive_test.cpp).
#include <doctest.h>

#include <cstdint>
#include <optional>
#include <string>
#include <string_view>
#include <utility>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"
#include "core/slot_calls.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::bindings;
using nlohmann::json;

namespace {
constexpr std::uintptr_t kCodeAddress = 0x500000;
constexpr std::uintptr_t kVtableAddress = 0x900000;
constexpr std::uintptr_t kPureVirtual = 0x42;

// Engine Feature -> its ISource2Server Slot Bindings (one Engine Feature per hook, plus one per direct call).
const std::vector<std::pair<std::string_view, std::vector<std::string>>> kFeatures{
    { features::GameFrame, { "ISource2Server::GameFrame" } },
    { features::HibernationEvents, { "ISource2Server::ServerHibernationUpdate" } },
    { features::SteamApiEvents, { "ISource2Server::GameServerSteamAPIActivated", "ISource2Server::GameServerSteamAPIDeactivated" } },
    { features::HostnameChanged, { "ISource2Server::OnHostNameChanged" } },
    { features::FatalShutdown, { "ISource2Server::PreFatalShutdown" } },
    { features::UpdateWhenNotInGame, { "ISource2Server::UpdateWhenNotInGame" } },
    { features::PreWorldUpdate, { "ISource2Server::PreWorldUpdate" } },
    { features::EconItemSystem, { "ISource2Server::GetEconItemSystem" } },
    { features::IsPaused, { "ISource2Server::IsPaused" } },
};

// Slot indices on 1.41.9.7 (the pinned SDK headers).
const std::vector<std::pair<std::string, std::size_t>> kSlots{
    { "ISource2Server::PreWorldUpdate", 15 },          { "ISource2Server::GameFrame", 19 },
    { "ISource2Server::ServerHibernationUpdate", 22 }, { "ISource2Server::IsPaused", 31 },
    { "ISource2Server::GameServerSteamAPIActivated", 41 }, { "ISource2Server::GameServerSteamAPIDeactivated", 42 },
    { "ISource2Server::OnHostNameChanged", 43 },       { "ISource2Server::PreFatalShutdown", 44 },
    { "ISource2Server::UpdateWhenNotInGame", 45 },     { "ISource2Server::GetEconItemSystem", 46 },
};

// A distinct function body per id: three pushes (registers spell the id in octal) and ret.
std::vector<std::uint8_t> Body(int id)
{
    return { static_cast<std::uint8_t>(0x50 + ((id >> 6) & 7)), static_cast<std::uint8_t>(0x50 + ((id >> 3) & 7)),
             static_cast<std::uint8_t>(0x50 + (id & 7)), 0xC3 };
}

// A CSource2Server vtable of distinct functions, optionally with one virtual inserted (as at 1.42.0.1, at 14).
struct FakeServer
{
    std::vector<std::uint8_t> code;
    std::vector<std::uintptr_t> vtable;

    explicit FakeServer(std::optional<std::size_t> insertedAt = std::nullopt)
    {
        std::vector<int> bodies;
        for (int id = 1; id <= 104; ++id)
            bodies.push_back(id);
        if (insertedAt) bodies.insert(bodies.begin() + static_cast<std::ptrdiff_t>(*insertedAt), 300);
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
            if (name != "CSource2Server") return {};
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

// Gamedata for every ISource2Server Slot Binding, as the generator records it on build.
json GamedataOn(const FakeServer& build)
{
    const auto fingerprints = build.Fingerprints();
    json gamedata = json::object();
    for (const auto& [key, index] : kSlots)
    {
        const auto record = vtable::RecordSlot(fingerprints, index);
        auto window = json::array();
        for (const auto& fingerprint : record.window.slots)
            window.push_back(fingerprint ? json(*fingerprint) : json(nullptr));
        gamedata[key] = { { "offsets", { { "linux", index } } },
                          { "vtable",
                            { { "class", "CSource2Server" },
                              { "library", "server" },
                              { "linux", { { "window", window }, { "uniqueNearby", record.uniqueNearby }, { "size", *record.vtableSize } } } } } };
    }
    return gamedata;
}

// The ISource2Server Bindings only: the rest of the catalog needs Bindings this fake module does not have.
ResolutionReport ResolveCore(const json& gamedataJson, const FakeServer& live)
{
    std::vector<BindingDeclaration> declarations;
    for (const auto& [key, index] : kSlots)
        declarations.push_back({ key, BindingKind::Slot });
    return BuiltinResolver(Platform::Linux).Resolve("test-build", ParseGamedata(gamedataJson, "linux"), { { "server", live.Image() } }, declarations);
}

bool Available(const ResolutionReport& report, std::string_view feature)
{
    const auto* declaration = FindNativeEngineFeature(feature);
    REQUIRE_MESSAGE(declaration != nullptr, feature);
    return EvaluateEngineFeature(report, *declaration).available;
}
} // namespace

TEST_CASE("The native catalog declares every ISource2Server Engine Feature on its Slot Bindings")
{
    CHECK(features::HibernationEvents == "cssharp.server/hibernation-events");
    CHECK(features::SteamApiEvents == "cssharp.server/steam-api-events");
    CHECK(features::HostnameChanged == "cssharp.server/hostname-changed");
    CHECK(features::FatalShutdown == "cssharp.server/fatal-shutdown");
    CHECK(features::UpdateWhenNotInGame == "cssharp.server/update-when-not-in-game");
    CHECK(features::PreWorldUpdate == "cssharp.server/pre-world-update");
    CHECK(features::EconItemSystem == "cssharp.server/econ-item-system");
    CHECK(features::IsPaused == "cssharp.server/is-paused");

    for (const auto& [name, keys] : kFeatures)
    {
        CAPTURE(name);
        const auto* feature = FindNativeEngineFeature(name);
        REQUIRE(feature != nullptr);
        REQUIRE(feature->bindings.size() == keys.size());
        for (std::size_t i = 0; i < keys.size(); ++i)
        {
            CHECK(feature->bindings[i].key == keys[i]);
            CHECK(feature->bindings[i].kind == BindingKind::Slot);
        }
    }
}

TEST_CASE("Every ISource2Server Engine Feature is available on the build its Gamedata was recorded on")
{
    const FakeServer build;

    const auto report = ResolveCore(GamedataOn(build), build);

    CHECK(FormatBindingSummary(report) == "Bindings for build test-build: all 10 resolved");
    for (const auto& [name, keys] : kFeatures)
    {
        CAPTURE(name);
        CHECK(Available(report, name));
    }
}

TEST_CASE("A virtual inserted before them heals every ISource2Server Slot Binding and keeps its Engine Features available")
{
    const auto gamedata = GamedataOn(FakeServer());

    const auto report = ResolveCore(gamedata, FakeServer(14));

    CHECK(FormatBindingSummary(report) == "Bindings for build test-build: all 10 resolved, 10 healed");
    for (const auto& [key, index] : kSlots)
    {
        CAPTURE(key);
        const auto* binding = report.Find(key);
        REQUIRE(binding != nullptr);
        CHECK(binding->status == BindingStatus::Healed);
        CHECK(binding->offset == static_cast<std::int64_t>(index) + 1);
    }
    for (const auto& [name, keys] : kFeatures)
    {
        CAPTURE(name);
        CHECK(Available(report, name));
    }
}

TEST_CASE("Each ISource2Server Engine Feature fails alone when one of its Slot Bindings fails")
{
    const FakeServer build;

    for (const auto& [failing, failingKeys] : kFeatures)
    {
        for (const auto& failingKey : failingKeys)
        {
            CAPTURE(failingKey);
            auto gamedata = GamedataOn(build);
            gamedata[failingKey]["vtable"]["linux"]["window"][1] = "0000000000000000";

            const auto report = ResolveCore(gamedata, build);

            CHECK(FormatBindingSummary(report) == "Bindings for build test-build: 1 of 10 failed: " + failingKey + " (fingerprint mismatch)");
            for (const auto& [name, keys] : kFeatures)
            {
                CAPTURE(name);
                CHECK(Available(report, name) == (name != failing));
            }
        }
    }
}

// --- Direct calls by Slot Binding index (src/core/slot_calls.h) ---

namespace {
struct Calls
{
    virtual ~Calls() = default;
    virtual int First(int x) { return x + 1; }
    virtual int Second(int x) { return x + 2; }
    virtual int Third(int x) const { return x + 3; }
};

// Itanium: the virtual destructor takes slots 0 and 1.
constexpr int kSecondSlot = 3;
constexpr int kThirdSlot = 4;
} // namespace

TEST_CASE("CallSlot calls the method at the Slot Binding's index, not at its compiled position")
{
    Calls calls;
    const Calls& constCalls = calls;

    CHECK(CallSlot(kSecondSlot, &Calls::First, &calls, 10) == 12);
    CHECK(CallSlot(kThirdSlot, &Calls::Third, &constCalls, 10) == 13);
#ifndef _WIN32
    // No index (the Binding failed): nothing is called.
    CHECK(CallSlot(std::nullopt, &Calls::First, &calls, 10) == 0);
    CHECK(CallSlot(std::nullopt, &Calls::Third, &constCalls, 10) == 0);
#endif
}

TEST_CASE("CallSlot through a SlotMethod calls at the index the given report states, and follows a new report")
{
    constexpr SlotMethod kFirst{ "Calls::First", &Calls::First };
    const auto reportWith = [](std::int64_t index, BindingStatus status) {
        return ResolutionReport("b", { { "Calls::First", BindingKind::Slot, status, FailureReason::None, 0, 0, index } });
    };
    Calls calls;

    CHECK(CallSlot(reportWith(2, BindingStatus::Resolved), kFirst, &calls, 10) == 11);
    CHECK(CallSlot(reportWith(3, BindingStatus::Healed), kFirst, &calls, 10) == 12);
#ifndef _WIN32
    CHECK(CallSlot(reportWith(3, BindingStatus::Failed), kFirst, &calls, 10) == 0);
    CHECK(CallSlot(ResolutionReport(), kFirst, &calls, 10) == 0);
#endif
}
