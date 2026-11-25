// Real-build tests against the shared build archive (~/dev/cs2-builds, or $CSSHARP_BUILD_ARCHIVE). Local only: each test
// is skipped when the builds it needs are absent; Valve binaries are never committed.
//   ./native_tests -tc="archive*" -s
// The oracle test also needs the research oracle's fixture directory (Valve-derived, kept outside the repo):
//   CSSHARP_FINGERPRINT_ORACLE=<dir with index.json> ./native_tests -tc="archive: fingerprints*" -s
#include <doctest.h>

#include <cstdlib>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "build_archive.h"
#include "core/binding_resolver.h"
#include "core/engine_features.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {
bool Archived(const char* build) { return FindArchivedModule(DefaultArchiveRoot(), build, "server").has_value(); }

const char* OracleDirectory() { return std::getenv("CSSHARP_FINGERPRINT_ORACLE"); }

GamedataJson CommittedGamedata()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return GamedataJson::parse(stream);
}

// The build an archived module of the oracle's fixture names ("engine2") is loaded from, by Gamedata library name.
std::string LibraryOf(const std::string& module) { return module == "engine2" ? "engine" : module; }

constexpr const char* kGameFrame = "ISource2Server::GameFrame";

bindings::ResolutionReport ResolveGameFrame(const LoadedModule& server, const GamedataJson& gamedataJson)
{
    const auto gamedata = bindings::ParseGamedata(nlohmann::json::parse(gamedataJson.dump()), "linux");
    const std::vector<bindings::BindingDeclaration> declarations{ { kGameFrame, bindings::BindingKind::Slot } };
    return bindings::BuiltinResolver(bindings::Platform::Linux).Resolve("archive", gamedata, { { "server", server.Image() } }, declarations);
}
} // namespace

TEST_CASE("archive: fingerprints match the research oracle for every slot of every fixture vtable (local)" *
          doctest::skip(OracleDirectory() == nullptr))
{
    const std::filesystem::path oracle = OracleDirectory();
    std::ifstream indexStream(oracle / "index.json");
    REQUIRE(indexStream.good());
    const auto index = nlohmann::json::parse(indexStream);

    std::size_t compared = 0;
    std::size_t underMaskedByOracle = 0;
    for (const auto& fixture : index["fixtures"])
    {
        const auto build = fixture["build"].get<std::string>();
        const auto library = LibraryOf(fixture["module"].get<std::string>());
        const auto className = fixture["cls"].get<std::string>();
        const auto file = fixture["file"].get<std::string>();
        // A build may have fixtures from both its flat image and its real ELF; compare the one the archive serves.
        const auto module = FindArchivedModule(DefaultArchiveRoot(), build, library);
        if (!module || std::filesystem::path(fixture["image"].get<std::string>()).extension() != module->extension()) continue;
        CAPTURE(file);

        std::ifstream expectedStream(oracle / file);
        REQUIRE(expectedStream.good());
        const auto expected = nlohmann::json::parse(expectedStream);

        const auto loaded = LoadedModule::Load(*module);
        const auto fingerprints = loaded.Fingerprints(className);
        const auto vtableAddress = loaded.FindVtable(className).address;

        REQUIRE(fingerprints.size() == expected["size"].get<std::size_t>());
        for (const auto& slot : expected["slots"])
        {
            const auto slotIndex = slot["idx"].get<std::size_t>();
            CAPTURE(slotIndex);
            if (fingerprints[slotIndex] == slot["fp"].get<std::string>()) continue;

            // Known oracle defect: capstone sizes some 66-prefixed SSE RIP displacements as 2 bytes, so the oracle
            // leaves the upper half unmasked. Accept a difference only where we zeroed bytes the oracle kept.
            const auto ours = vtable::FingerprintFunction(loaded.Code(), *loaded.ReadVtableSlot(vtableAddress, slotIndex)).canonical;
            const auto theirs = slot["canon"].get<std::string>();
            CAPTURE(ours);
            CAPTURE(theirs);
            REQUIRE(ours.size() == theirs.size());
            for (std::size_t i = 0; i < ours.size(); ++i)
                if (ours[i] != theirs[i]) CHECK(ours[i] == '0');
            ++underMaskedByOracle;
        }
        ++compared;
    }
    std::cout << "compared " << compared << " vtables with the oracle; " << underMaskedByOracle
              << " slots differ only where the oracle left part of a displacement unmasked\n";
    CHECK(compared > 0);
}

TEST_CASE("archive: bootstrapping GameFrame on 1.42.0.1 reproduces the committed Gamedata entry (local)" *
          doctest::skip(!Archived("1.42.0.1")))
{
    const auto server = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.42.0.1", "server");
    const auto committed = CommittedGamedata().at(kGameFrame);

    // Index 20: the SDK headers the plugin is built with for 1.42.0.1.
    const auto generated = BootstrapSlotBinding(server.Fingerprints("CSource2Server"), { kGameFrame, "CSource2Server", "server", 20 }, committed);

    CHECK(generated.warnings.empty());
    CHECK(generated.entry == committed);
}

TEST_CASE("archive: committed GameFrame resolves on 1.42.0.1 by its window, with no size warning (local)" *
          doctest::skip(!Archived("1.42.0.1")))
{
    const auto server = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.42.0.1", "server");

    const auto report = ResolveGameFrame(server, CommittedGamedata());

    const auto& gameFrame = report.Bindings().at(0);
    CHECK(gameFrame.status == bindings::BindingStatus::Resolved);
    CHECK(gameFrame.offset == 20);
    CHECK(gameFrame.slot->match == vtable::SlotMatch::Window);
    CHECK(gameFrame.slot->vtableSize == 105);
    CHECK(bindings::FormatVtableSizeWarnings(report).empty());
}

TEST_CASE("archive: GameFrame on 1.41.9.7 sits one slot earlier with the same fingerprint; the 1.42.0.1 Binding heals "
          "there at -1 (local)" *
          doctest::skip(!Archived("1.41.9.7")))
{
    const auto server = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.41.9.7", "server");
    const auto fingerprints = server.Fingerprints("CSource2Server");
    const auto committed = CommittedGamedata().at(kGameFrame);

    // Index 19: the repo's pinned SDK headers, known correct for 1.41.9.7 (the bootstrap build).
    const auto bootstrapped = BootstrapSlotBinding(fingerprints, { kGameFrame, "CSource2Server", "server", 19 });
    CHECK(bootstrapped.warnings.empty());
    CHECK(bootstrapped.entry["vtable"]["linux"]["window"][1] == committed["vtable"]["linux"]["window"][1]);
    CHECK(bootstrapped.entry["vtable"]["linux"]["size"] == 104);

    const auto report = ResolveGameFrame(server, CommittedGamedata());
    const auto& gameFrame = report.Bindings().at(0);
    CHECK(gameFrame.status == bindings::BindingStatus::Healed);
    CHECK(gameFrame.offset == 19);
    CHECK(gameFrame.slot->statedIndex == 20);
    CHECK(gameFrame.slot->vtableSize == 104);
    CHECK(gameFrame.slot->SizeChanged());
}

TEST_CASE("archive: GameFrame's Slot Binding from 1.41.9.7 heals at +1 on 1.42.0.1 (local)" *
          doctest::skip(!Archived("1.41.9.7") || !Archived("1.42.0.1")))
{
    const auto old = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.41.9.7", "server");
    const auto bootstrapped = BootstrapSlotBinding(old.Fingerprints("CSource2Server"), { kGameFrame, "CSource2Server", "server", 19 });
    GamedataJson gamedata = GamedataJson::object();
    gamedata[kGameFrame] = bootstrapped.entry;

    const auto live = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.42.0.1", "server");
    const auto report = ResolveGameFrame(live, gamedata);

    const auto& gameFrame = report.Bindings().at(0);
    CHECK(gameFrame.status == bindings::BindingStatus::Healed);
    CHECK(gameFrame.Resolved());
    CHECK(gameFrame.offset == 20);
    REQUIRE(gameFrame.slot);
    CHECK(gameFrame.slot->statedIndex == 19);
    CHECK(gameFrame.slot->match == vtable::SlotMatch::Heal);
    CHECK(gameFrame.slot->vtableSize == 105);
    CHECK(gameFrame.slot->recordedVtableSize == 104u);

    // The healed index is the one the committed 1.42.0.1 Gamedata states (bootstrapped from the e2e SDK headers).
    CHECK(gameFrame.offset == CommittedGamedata()[kGameFrame]["offsets"]["linux"].get<std::int64_t>());
    const auto heals = bindings::FormatHeals(report);
    REQUIRE(heals.size() == 1);
    MESSAGE(heals[0]);
}

// --- Carry-forward replays ---

namespace {
bool ArchivedPair(std::initializer_list<const char*> libraries)
{
    for (const char* build : { "1.41.9.7", "1.42.0.1" })
        for (const char* library : libraries)
            if (!FindArchivedModule(DefaultArchiveRoot(), build, library)) return false;
    return true;
}

// The committed Gamedata with GameFrame bootstrapped on 1.41.9.7 at the pinned SDK's index 19 (the bootstrap build).
GamedataJson GamedataFor14197(const LoadedModule& server)
{
    auto gamedata = CommittedGamedata();
    gamedata[kGameFrame] = BootstrapSlotBinding(server.Fingerprints("CSource2Server"), { kGameFrame, "CSource2Server", "server", 19 }, gamedata[kGameFrame]).entry;
    return gamedata;
}

// Gamedata with one Slot Binding per slot of a class's vtable on a build, keyed "<Class>::<index>".
GamedataJson EverySlot(ArchivedBuild& build, const std::string& library, const std::string& className)
{
    const auto fingerprints = build.Vtables()(library, className);
    GamedataJson gamedata = GamedataJson::object();
    for (std::size_t index = 0; index < fingerprints.size(); ++index)
    {
        const auto key = className + "::" + std::to_string(index);
        gamedata[key] = BootstrapSlotBinding(fingerprints, { key, className, library, index }).entry;
    }
    return gamedata;
}
} // namespace

TEST_CASE("archive: carrying the 1.41.9.7 Gamedata forward moves GameFrame 19 -> 20 and reproduces the committed 1.42.0.1 "
          "Gamedata (local)" *
          doctest::skip(!ArchivedPair({ "server" })))
{
    ArchivedBuild previous(DefaultArchiveRoot(), "1.41.9.7");
    ArchivedBuild next(DefaultArchiveRoot(), "1.42.0.1");
    const auto gamedata = GamedataFor14197(previous.Module("server"));
    REQUIRE(gamedata != CommittedGamedata());

    const auto result = CarryForward(gamedata, previous.Vtables(), next.Vtables());

    CHECK(result.held.empty());
    CHECK(result.warnings.empty());
    // GameFrame moves; every other committed Slot Binding stays where it is.
    std::size_t moved = 0;
    for (const auto& carried : result.carried)
    {
        CAPTURE(carried.key);
        if (carried.key != kGameFrame)
        {
            CHECK(carried.from == carried.to);
            continue;
        }
        ++moved;
        CHECK(carried.from == 19);
        CHECK(carried.to == 20);
    }
    CHECK(moved == 1);
    CHECK(result.gamedata == CommittedGamedata());
}

TEST_CASE("archive: carrying the committed Gamedata onto the same build changes nothing (local)" * doctest::skip(!Archived("1.42.0.1")))
{
    ArchivedBuild build(DefaultArchiveRoot(), "1.42.0.1");

    const auto result = CarryForward(CommittedGamedata(), build.Vtables(), build.Vtables());

    CHECK(result.held.empty());
    for (const auto& carried : result.carried)
        CHECK(carried.from == carried.to);
    CHECK(result.gamedata == CommittedGamedata());
}

TEST_CASE("archive: the ISource2Server slots CounterStrikeSharp uses all carry forward one slot up at 1.42.0.1 (local)" *
          doctest::skip(!ArchivedPair({ "server" })))
{
    // SDK header indices: pinned SDK (1.41.9.7-correct) -> the e2e SDK (1.42.0.1-correct).
    const std::vector<std::pair<const char*, std::size_t>> methods{
        { "PreWorldUpdate", 15 },          { "GameFrame", 19 },
        { "ServerHibernationUpdate", 22 }, { "IsPaused", 31 },
        { "GameServerSteamAPIActivated", 41 }, { "GameServerSteamAPIDeactivated", 42 },
        { "OnHostNameChanged", 43 },       { "PreFatalShutdown", 44 },
        { "UpdateWhenNotInGame", 45 },     { "GetEconItemSystem", 46 },
    };
    ArchivedBuild previous(DefaultArchiveRoot(), "1.41.9.7");
    ArchivedBuild next(DefaultArchiveRoot(), "1.42.0.1");
    const auto fingerprints = previous.Vtables()("server", "CSource2Server");
    GamedataJson gamedata = GamedataJson::object();
    for (const auto& [method, index] : methods)
    {
        const auto key = std::string("ISource2Server::") + method;
        gamedata[key] = BootstrapSlotBinding(fingerprints, { key, "CSource2Server", "server", index }).entry;
    }

    const auto result = CarryForward(gamedata, previous.Vtables(), next.Vtables());

    CHECK(result.held.empty());
    REQUIRE(result.carried.size() == methods.size());
    for (std::size_t i = 0; i < methods.size(); ++i)
    {
        CAPTURE(result.carried[i].key);
        CHECK(result.carried[i].to == methods[i].second + 1);
    }
}

TEST_CASE("archive: no slot of a 1.41.9.7 vtable is carried anywhere but where it went at 1.42.0.1 (local)" *
          doctest::skip(!ArchivedPair({ "server", "engine", "tier0" })))
{
    // The one change between these builds in these classes: CSource2Server gained a virtual at index 14. Every other
    // class kept its layout. A slot is either carried to exactly that index or held; never moved anywhere else.
    struct ClassCase
    {
        const char* library;
        const char* className;
        std::size_t insertedAt;
    };
    constexpr std::size_t kNone = ~std::size_t{ 0 };
    const std::vector<ClassCase> classes{
        { "server", "CSource2Server", 14 },     { "server", "CSource2GameClients", kNone }, { "server", "CSource2GameEntities", kNone },
        { "server", "CGameEventManager", kNone }, { "engine", "CEngineServer", kNone },      { "engine", "CGameEventSystem", kNone },
        { "tier0", "CCvar", kNone },
    };
    ArchivedBuild previous(DefaultArchiveRoot(), "1.41.9.7");
    ArchivedBuild next(DefaultArchiveRoot(), "1.42.0.1");

    for (const auto& [library, className, insertedAt] : classes)
    {
        CAPTURE(className);
        const auto gamedata = EverySlot(previous, library, className);

        const auto result = CarryForward(gamedata, previous.Vtables(), next.Vtables());

        CHECK(result.carried.size() + result.held.size() == gamedata.size());
        for (const auto& carried : result.carried)
        {
            CAPTURE(carried.key);
            CHECK(carried.to == (carried.from >= insertedAt ? carried.from + 1 : carried.from));
        }
        std::cout << className << ": " << result.carried.size() << " of " << gamedata.size() << " slots carried, " << result.held.size() << " held\n";
        for (const auto& held : result.held)
            std::cout << "  held " << held.key << ": " << held.reason << "\n";
    }
}
