// The never-moved interfaces' Slot Bindings against real archived builds (local only; skipped without the archive), and
// the generator's handling of the Slot Bindings that stay unverified.
//   ./native_tests -tc="never-moved*" -s
#include <doctest.h>

#include <fstream>
#include <string>
#include <vector>

#include "build_archive.h"
#include "core/binding_resolver.h"
#include "core/engine_features.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {
GamedataJson CommittedGamedata()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return GamedataJson::parse(stream);
}

bool ArchivedPair()
{
    for (const char* build : { "1.41.9.7", "1.42.0.1" })
        for (const char* library : { "server", "engine" })
            if (!FindArchivedModule(DefaultArchiveRoot(), build, library)) return false;
    return true;
}

struct NeverMoved
{
    const char* key;
    const char* className;
    const char* library;
    std::size_t index; // SDK header index (the same in the pinned SDK and the e2e SDK): what the plugin's compiled calls use
};

const std::vector<NeverMoved> kNeverMoved{
    { "IGameEventManager2::LoadEventsFromFile", "CGameEventManager", "server", 2 },
    { "IGameEventManager2::AddListener", "CGameEventManager", "server", 4 },
    { "IGameEventManager2::FindListener", "CGameEventManager", "server", 5 },
    { "IGameEventManager2::RemoveListener", "CGameEventManager", "server", 6 },
    { "IGameEventManager2::CreateEvent", "CGameEventManager", "server", 7 },
    { "IGameEventManager2::FireEvent", "CGameEventManager", "server", 8 },
    { "IGameEventManager2::FreeEvent", "CGameEventManager", "server", 11 },
    { "IGameEventSystem::PostEventAbstract", "CGameEventSystem", "engine", 15 },
    { "IGameEventSystem::PostEventAbstract_Filter", "CGameEventSystem", "engine", 16 },
    { "INetworkServerService::GetIGameServer", "CNetworkServerService", "engine", 24 },
    { "INetworkServerService::StartupServer", "CNetworkServerService", "engine", 27 },
    { "INetworkGameServer::GetGlobals", "CNetworkGameServer", "engine", 7 },
    { "IEngineServiceMgr::RegisterLoopMode", "CEngineServiceMgr", "engine", 13 },
    { "IEngineServiceMgr::FindService", "CEngineServiceMgr", "engine", 17 },
};

// Only the never-moved keys of a Gamedata file.
GamedataJson NeverMovedOf(const GamedataJson& gamedata)
{
    GamedataJson subset = GamedataJson::object();
    for (const auto& slot : kNeverMoved)
        subset[slot.key] = gamedata.at(slot.key);
    return subset;
}

// Resolves the never-moved Slot Bindings of gamedata on one archived build, each as a compiled call at its SDK index.
bindings::ResolutionReport ResolveOn(ArchivedBuild& build, const GamedataJson& gamedata)
{
    std::vector<bindings::BindingDeclaration> declarations;
    for (const auto& slot : kNeverMoved)
        declarations.push_back({ slot.key, bindings::BindingKind::Slot, "", static_cast<std::int64_t>(slot.index) });
    const bindings::ModuleImages modules{ { "server", build.Module("server").Image() }, { "engine", build.Module("engine").Image() } };
    return bindings::BuiltinResolver(bindings::Platform::Linux)
        .Resolve(build.Build(), bindings::ParseGamedata(nlohmann::json::parse(gamedata.dump()), "linux"), modules, declarations);
}
} // namespace

TEST_CASE("never-moved: the committed Slot Bindings sit at their SDK header indices")
{
    const auto gamedata = CommittedGamedata();
    for (const auto& slot : kNeverMoved)
    {
        CAPTURE(slot.key);
        const auto& entry = gamedata.at(slot.key);
        CHECK(entry["offsets"]["linux"] == slot.index);
        CHECK(entry["vtable"]["class"] == slot.className);
        CHECK(entry["vtable"]["library"] == slot.library);
    }
}

TEST_CASE("never-moved: the generator needs no module for Slot Bindings without a window (they are held)")
{
    GamedataJson gamedata = GamedataJson::object();
    gamedata["INetworkMessages::FindNetworkMessagePartial"] = { { "offsets", { { "linux", 10 } } },
                                                                { "vtable", { { "class", "CNetworkMessages" }, { "library", "networksystem" } } } };
    gamedata["IGameEventManager2::FireEvent"] = CommittedGamedata().at("IGameEventManager2::FireEvent");

    CHECK(SlotBindingLibraries(gamedata) == std::vector<std::string>{ "server" });

    const VtableFingerprints none = [](std::string_view, std::string_view) -> std::vector<vtable::Fingerprint> {
        throw std::runtime_error("no module");
    };
    const auto result = CarryForward(gamedata, none, none);
    REQUIRE(result.held.size() == 2);
    CHECK(result.held[0].key == "INetworkMessages::FindNetworkMessagePartial");
    CHECK(result.held[0].reason == "no fingerprint window for linux");
}

TEST_CASE("never-moved: every committed Slot Binding verifies at its compiled index on 1.42.0.1 by its window (local)" *
          doctest::skip(!ArchivedPair()))
{
    ArchivedBuild build(DefaultArchiveRoot(), "1.42.0.1");

    const auto report = ResolveOn(build, CommittedGamedata());

    for (const auto& binding : report.Bindings())
    {
        CAPTURE(binding.key);
        CHECK(binding.status == bindings::BindingStatus::Resolved);
        REQUIRE(binding.slot);
        CHECK(binding.slot->match == vtable::SlotMatch::Window);
        CHECK_FALSE(binding.slot->SizeChanged());
    }
}

TEST_CASE("never-moved: the 1.42.0.1 Slot Bindings resolve unmoved on 1.41.9.7, and none is healed or moved (local)" *
          doctest::skip(!ArchivedPair()))
{
    ArchivedBuild build(DefaultArchiveRoot(), "1.41.9.7");

    const auto report = ResolveOn(build, CommittedGamedata());

    for (const auto& binding : report.Bindings())
    {
        CAPTURE(binding.key);
        CHECK(binding.status == bindings::BindingStatus::Resolved);
        CHECK(binding.reason == bindings::FailureReason::None);
    }
}

TEST_CASE("never-moved: bootstrapped on 1.41.9.7 and carried forward, they reproduce the committed 1.42.0.1 entries (local)" *
          doctest::skip(!ArchivedPair()))
{
    ArchivedBuild previous(DefaultArchiveRoot(), "1.41.9.7");
    ArchivedBuild next(DefaultArchiveRoot(), "1.42.0.1");
    const auto committed = NeverMovedOf(CommittedGamedata());

    GamedataJson bootstrapped = GamedataJson::object();
    for (const auto& slot : kNeverMoved)
    {
        const auto generated = BootstrapSlotBinding(previous.Vtables()(slot.library, slot.className), { slot.key, slot.className, slot.library, slot.index });
        CAPTURE(slot.key);
        CHECK(generated.warnings.empty());
        bootstrapped[slot.key] = generated.entry;
    }

    const auto result = CarryForward(bootstrapped, previous.Vtables(), next.Vtables());

    CHECK(result.held.empty());
    CHECK(result.warnings.empty());
    CHECK(result.carried.size() == kNeverMoved.size());
    for (const auto& carried : result.carried)
        CHECK(carried.from == carried.to);
    CHECK(result.gamedata == committed);
}
