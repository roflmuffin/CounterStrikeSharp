// IServerGameClients hooks (client connect, put in server, disconnect, client command, voice, client user message) as
// Slot Bindings on CSource2GameClients, each backing its own Engine Feature.
//   ./native_tests -tc="*IServerGameClients*" -s
// The real-build tests read the shared build archive (~/dev/cs2-builds, or $CSSHARP_BUILD_ARCHIVE) and skip what is
// absent; Valve binaries are never committed.
#include <doctest.h>

#include <algorithm>
#include <string>
#include <string_view>
#include <utility>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;

namespace {
struct ClientsFeature
{
    std::string_view name;
    std::vector<std::string> keys;
};

const std::vector<ClientsFeature> kClientsFeatures{
    { features::PlayerConnectionEvents,
      { "IServerGameClients::ClientConnect", "IServerGameClients::ClientPutInServer", "IServerGameClients::ClientDisconnect" } },
    { features::PlayerClientCommands, { "IServerGameClients::ClientCommand" } },
    { features::PlayerVoiceEvents, { "IServerGameClients::ClientVoice" } },
    { features::UiCustomHudLayout, { "IServerGameClients::ClientSvcUserMessage" } },
};

std::vector<std::string> AllClientsKeys()
{
    std::vector<std::string> keys;
    for (const auto& feature : kClientsFeatures)
        keys.insert(keys.end(), feature.keys.begin(), feature.keys.end());
    return keys;
}

BindingResult Slot(std::string key, BindingStatus status, FailureReason reason = FailureReason::None)
{
    BindingResult result;
    result.key = std::move(key);
    result.kind = BindingKind::Slot;
    result.status = status;
    result.reason = reason;
    return result;
}
} // namespace

TEST_CASE("The native catalog declares the IServerGameClients Engine Features on their Slot Bindings")
{
    CHECK(features::PlayerConnectionEvents == "cssharp.player/connection-events");
    CHECK(features::PlayerClientCommands == "cssharp.player/client-commands");
    CHECK(features::PlayerVoiceEvents == "cssharp.player/voice-events");
    CHECK(features::UiCustomHudLayout == "cssharp.ui/custom-hud-layout");

    for (const auto& [name, keys] : kClientsFeatures)
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

TEST_CASE("Each IServerGameClients Engine Feature fails on its own Slot Bindings only")
{
    for (const auto& failedKey : AllClientsKeys())
    {
        CAPTURE(failedKey);
        std::vector<BindingResult> results;
        for (const auto& key : AllClientsKeys())
            results.push_back(key == failedKey ? Slot(key, BindingStatus::Failed, FailureReason::FingerprintMismatch)
                                               : Slot(key, BindingStatus::Resolved));
        const ResolutionReport report("b", std::move(results));

        for (const auto& [name, keys] : kClientsFeatures)
        {
            CAPTURE(name);
            const bool usesFailedKey = std::find(keys.begin(), keys.end(), failedKey) != keys.end();
            const auto status = EvaluateEngineFeature(report, *FindNativeEngineFeature(name));
            CHECK(status.available == !usesFailedKey);
            if (usesFailedKey)
            {
                REQUIRE(status.failedBinding != nullptr);
                CHECK(status.failedBinding->key == failedKey);
            }
        }
    }
}

TEST_CASE("Healed and unverified IServerGameClients Slot Bindings keep their Engine Features available")
{
    std::vector<BindingResult> results;
    for (const auto& key : AllClientsKeys())
        results.push_back(Slot(key, key == "IServerGameClients::ClientConnect" ? BindingStatus::Healed : BindingStatus::Unverified));
    const ResolutionReport report("b", std::move(results));

    for (const auto& [name, keys] : kClientsFeatures)
    {
        CAPTURE(name);
        CHECK(EvaluateEngineFeature(report, *FindNativeEngineFeature(name)).available);
    }
}

#if defined(__linux__)
#include <cstdlib>
#include <fstream>
#include <iostream>

#include "build_archive.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {
// CSource2GameClients slots of the methods we hook. Every archived build (1.41.8.1 .. 1.42.0.1) has them at these
// indices: the SDK headers are the same at 1.41.9.7 (pinned) and 1.42.0.1 (e2e), and the research oracle's ground truth
// (global function matcher / whole-body alignment over the 8 archived pairs) maps each of them to itself.
const std::vector<std::pair<std::string, std::size_t>> kClientsSlots{
    { "IServerGameClients::ClientConnect", 12 }, { "IServerGameClients::ClientPutInServer", 13 },
    { "IServerGameClients::ClientDisconnect", 16 }, { "IServerGameClients::ClientCommand", 17 },
    { "IServerGameClients::ClientVoice", 27 }, { "IServerGameClients::ClientSvcUserMessage", 38 },
};

const std::vector<const char*> kArchivedBuilds{ "1.41.8.1", "1.41.8.2", "1.41.9.0", "1.41.9.1a", "1.41.9.1b",
                                                "1.41.9.2", "1.41.9.6", "1.41.9.7", "1.42.0.1" };

bool ServerArchived(const char* build) { return FindArchivedModule(DefaultArchiveRoot(), build, "server").has_value(); }

GamedataJson CommittedGamedata()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return GamedataJson::parse(stream);
}

// The six Slot Bindings bootstrapped on a build, each over the entry base holds for its key (if any).
GamedataJson BootstrapClients(const LoadedModule& server, const GamedataJson& base = GamedataJson::object())
{
    const auto fingerprints = server.Fingerprints("CSource2GameClients");
    GamedataJson gamedata = base;
    for (const auto& [key, index] : kClientsSlots)
    {
        const auto existing = base.contains(key) ? base.at(key) : GamedataJson::object();
        const auto generated = BootstrapSlotBinding(fingerprints, { key, "CSource2GameClients", "server", index }, existing);
        CAPTURE(key);
        CHECK(generated.warnings.empty());
        gamedata[key] = generated.entry;
    }
    return gamedata;
}

ResolutionReport ResolveClients(const LoadedModule& server, const GamedataJson& gamedataJson)
{
    const auto gamedata = ParseGamedata(nlohmann::json::parse(gamedataJson.dump()), "linux");
    std::vector<BindingDeclaration> declarations;
    for (const auto& [key, index] : kClientsSlots)
        declarations.push_back({ key, BindingKind::Slot });
    return BuiltinResolver(Platform::Linux).Resolve("archive", gamedata, { { "server", server.Image() } }, declarations);
}
} // namespace

TEST_CASE("archive: bootstrapping the IServerGameClients Slot Bindings on 1.42.0.1 reproduces the committed Gamedata (local)" *
          doctest::skip(!ServerArchived("1.42.0.1")))
{
    const auto server = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.42.0.1", "server");
    const auto committed = CommittedGamedata();

    const auto generated = BootstrapClients(server, committed);

    for (const auto& [key, index] : kClientsSlots)
    {
        CAPTURE(key);
        REQUIRE(committed.contains(key));
        CHECK(generated.at(key) == committed.at(key));
    }
}

TEST_CASE("archive: the IServerGameClients Slot Bindings bootstrapped on 1.41.9.7 carry forward to the committed 1.42.0.1 "
          "entries (local)" *
          doctest::skip(!ServerArchived("1.41.9.7") || !ServerArchived("1.42.0.1")))
{
    ArchivedBuild previous(DefaultArchiveRoot(), "1.41.9.7");
    ArchivedBuild next(DefaultArchiveRoot(), "1.42.0.1");
    const auto committed = CommittedGamedata();
    // The committed Gamedata with only these six entries taken back to the bootstrap build.
    const auto gamedata = BootstrapClients(previous.Module("server"), committed);

    const auto result = CarryForward(gamedata, previous.Vtables(), next.Vtables());

    for (const auto& held : result.held)
        CHECK_MESSAGE(held.key.rfind("IServerGameClients::", 0) != 0, "held " << held.key << ": " << held.reason);
    std::size_t carried = 0;
    for (const auto& entry : result.carried)
    {
        if (entry.key.rfind("IServerGameClients::", 0) != 0) continue;
        CAPTURE(entry.key);
        CHECK(entry.from == entry.to);
        ++carried;
    }
    CHECK(carried == kClientsSlots.size());
    for (const auto& [key, index] : kClientsSlots)
    {
        CAPTURE(key);
        CHECK(result.gamedata.at(key) == committed.at(key));
    }
}

TEST_CASE("archive: the IServerGameClients Slot Bindings resolve at their true index across every archived build pair, "
          "never anywhere else (local)")
{
    std::size_t pairs = 0;
    for (std::size_t i = 0; i + 1 < kArchivedBuilds.size(); ++i)
    {
        const char* from = kArchivedBuilds[i];
        const char* to = kArchivedBuilds[i + 1];
        if (!ServerArchived(from) || !ServerArchived(to)) continue;
        CAPTURE(from);
        CAPTURE(to);

        const auto previous = LoadedModule::FromArchive(DefaultArchiveRoot(), from, "server");
        const auto live = LoadedModule::FromArchive(DefaultArchiveRoot(), to, "server");
        const auto report = ResolveClients(live, BootstrapClients(previous));

        for (const auto& [key, index] : kClientsSlots)
        {
            CAPTURE(key);
            const auto* result = report.Find(key);
            REQUIRE(result != nullptr);
            // Zero wrong: a resolved or healed Binding is at its true index. These slots never moved, so all resolve.
            CHECK(result->status == BindingStatus::Resolved);
            if (result->Resolved()) CHECK(result->offset == static_cast<std::int64_t>(index));
        }
        ++pairs;
    }
    std::cout << "IServerGameClients: " << pairs << " archived build pairs replayed\n";
    if (pairs == 0) MESSAGE("no archived build pair present; nothing replayed");
}

TEST_CASE("archive: the committed IServerGameClients Slot Bindings also resolve on 1.41.9.7 (local)" *
          doctest::skip(!ServerArchived("1.41.9.7")))
{
    const auto server = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.41.9.7", "server");

    const auto report = ResolveClients(server, CommittedGamedata());

    for (const auto& [key, index] : kClientsSlots)
    {
        CAPTURE(key);
        const auto* result = report.Find(key);
        REQUIRE(result != nullptr);
        CHECK(result->status == BindingStatus::Resolved);
        CHECK(result->offset == static_cast<std::int64_t>(index));
    }
}
#endif
