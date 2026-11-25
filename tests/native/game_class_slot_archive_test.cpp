// Real-build tests for the game-class virtuals CounterStrikeSharp calls through Gamedata offsets (respawn, commit
// suicide, change team, teleport, item services, ...) as Slot Bindings, against the shared build archive. Local only:
// skipped when the archive lacks a build. Truth is the hand-maintained Gamedata history, i.e. the indices maintainers
// fixed by hand for each update (git history of gamedata.json):
//   1.41.8.1:             the 1.41.7.7 fix (faa97820), unchanged until the 1.41.8.2 fix
//   1.41.8.2 .. 1.42.0.1: the 1.41.8.2 fix (2f984988); later fixes moved none of these on Linux
//   ./native_tests -tc="archive: game-class*" -s
#include <doctest.h>

#include <algorithm>
#include <fstream>
#include <iostream>
#include <map>
#include <memory>
#include <string>
#include <vector>

#include "build_archive.h"
#include "core/binding_resolver.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {

// Every server build in the archive, oldest first.
const std::vector<std::string> kBuilds{ "1.41.8.1", "1.41.8.2", "1.41.9.0", "1.41.9.1a", "1.41.9.1b",
                                        "1.41.9.2", "1.41.9.6", "1.41.9.7", "1.42.0.1" };

struct GameClassSlot
{
    std::string key;
    std::string className; // as committed Gamedata names it ("vtable.class")
    std::size_t before; // index on 1.41.8.1
    std::size_t after; // index from 1.41.8.2 on
};

const std::vector<GameClassSlot> kSlots{
    { "ISource2GameEntities::CheckTransmit", "CSource2GameEntities", 13, 13 },
    { "CCSPlayerController_ChangeTeam", "CCSPlayerController", 102, 104 },
    { "CCSPlayerController_Respawn", "CCSPlayerController", 272, 276 },
    { "CCSPlayer_ItemServices_GiveNamedItem", "CCSPlayer_ItemServices", 24, 24 },
    { "CCSPlayer_ItemServices_DropActivePlayerWeapon", "CCSPlayer_ItemServices", 27, 27 },
    { "CCSPlayer_ItemServices_RemoveWeapons", "CCSPlayer_ItemServices", 28, 28 },
    { "CGameSceneNode_GetSkeletonInstance", "CGameSceneNode", 13, 13 },
    { "CCSGameRules_FindPickerEntity", "CCSGameRules", 27, 26 },
    { "CBaseEntity_IsPlayerPawn", "CCSPlayerPawn", 168, 170 },
    { "CBaseEntity_Teleport", "CBaseEntity", 162, 164 },
    { "CBasePlayerPawn_CommitSuicide", "CBasePlayerPawn", 384, 387 },
};

std::size_t IndexOn(const GameClassSlot& slot, const std::string& build) { return build == "1.41.8.1" ? slot.before : slot.after; }

const GameClassSlot& SlotOf(const std::string& key)
{
    const auto slot = std::find_if(kSlots.begin(), kSlots.end(), [&](const GameClassSlot& s) {
        return s.key == key;
    });
    REQUIRE(slot != kSlots.end());
    return *slot;
}

bool Archived(const std::string& build) { return FindArchivedModule(DefaultArchiveRoot(), build, "server").has_value(); }

bool AllArchived()
{
    return std::all_of(kBuilds.begin(), kBuilds.end(), Archived);
}

GamedataJson CommittedGamedata()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return GamedataJson::parse(stream);
}

// One archived build, its class vtables fingerprinted once each (the replays ask for the same classes many times).
class Build
{
  public:
    explicit Build(const std::string& id) : m_build(DefaultArchiveRoot(), id) {}

    [[nodiscard]] const std::string& Id() const { return m_build.Build(); }
    [[nodiscard]] const LoadedModule& Server() { return m_build.Module("server"); }

    [[nodiscard]] VtableFingerprints Vtables()
    {
        return [this](std::string_view library, std::string_view className) {
            const auto key = std::string(library) + "/" + std::string(className);
            auto it = m_fingerprints.find(key);
            if (it == m_fingerprints.end()) it = m_fingerprints.emplace(key, m_build.Vtables()(library, className)).first;
            return it->second;
        };
    }

  private:
    ArchivedBuild m_build;
    std::map<std::string, std::vector<vtable::Fingerprint>> m_fingerprints;
};

// Archived builds, each loaded once for all tests in this file.
Build& BuildOf(const std::string& id)
{
    static std::map<std::string, std::unique_ptr<Build>> builds;
    auto& build = builds[id];
    if (!build) build = std::make_unique<Build>(id);
    return *build;
}

// One game-class Slot Binding bootstrapped on a build at its hand-maintained index there, merged into entry.
GamedataJson BootstrapOn(Build& build, const GameClassSlot& slot, GamedataJson entry)
{
    const auto index = IndexOn(slot, build.Id());
    entry["offsets"]["linux"] = index;
    return BootstrapSlotBinding(build.Vtables()("server", slot.className), { slot.key, slot.className, "server", index }, entry).entry;
}

// The committed entries of the game-class Slot Bindings (and nothing else), bootstrapped on a build.
GamedataJson GamedataOn(Build& build)
{
    const auto committed = CommittedGamedata();
    GamedataJson gamedata = GamedataJson::object();
    for (const auto& slot : kSlots)
        gamedata[slot.key] = BootstrapOn(build, slot, committed.at(slot.key));
    return gamedata;
}

bindings::ResolutionReport Resolve(const GamedataJson& gamedataJson, const LoadedModule& server)
{
    const auto gamedata = bindings::ParseGamedata(nlohmann::json::parse(gamedataJson.dump()), "linux");
    std::vector<bindings::BindingDeclaration> declarations;
    for (const auto& slot : kSlots)
        declarations.push_back({ slot.key, bindings::BindingKind::Slot });
    return bindings::BuiltinResolver(bindings::Platform::Linux).Resolve("archive", gamedata, { { "server", server.Image() } }, declarations);
}

} // namespace

TEST_CASE("archive: committed game-class Slot Bindings are recorded on 1.42.0.1 and resolve there by their window (local)" *
          doctest::skip(!Archived("1.42.0.1")))
{
    auto& build = BuildOf("1.42.0.1");
    const auto committed = CommittedGamedata();

    for (const auto& slot : kSlots)
    {
        CAPTURE(slot.key);
        const auto& entry = committed.at(slot.key);
        REQUIRE(entry.contains("vtable"));
        CHECK(entry["vtable"]["class"] == slot.className);
        CHECK(entry["vtable"]["library"] == "server");
        CHECK(entry["offsets"]["linux"] == slot.after);
        // Bootstrapping at the committed index changes nothing: the committed fields are what the generator records.
        CHECK(BootstrapOn(build, slot, entry) == entry);
    }

    const auto report = Resolve(committed, build.Server());
    for (const auto& binding : report.Bindings())
    {
        CAPTURE(binding.key);
        CHECK(binding.status == bindings::BindingStatus::Resolved);
        CHECK(binding.slot->match == vtable::SlotMatch::Window);
    }
    CHECK(bindings::FormatVtableSizeWarnings(report).empty());
}

TEST_CASE("archive: game-class Slot Bindings over every archived pair resolve or heal at the hand-maintained index, or fail; "
          "never wrong (local)" *
          doctest::skip(!AllArchived()))
{
    std::map<std::string, std::vector<std::string>> outcomes;
    for (std::size_t pair = 0; pair + 1 < kBuilds.size(); ++pair)
    {
        auto& previous = BuildOf(kBuilds[pair]);
        auto& next = BuildOf(kBuilds[pair + 1]);
        const auto report = Resolve(GamedataOn(previous), next.Server());

        for (const auto& slot : kSlots)
        {
            const auto* binding = report.Find(slot.key);
            REQUIRE(binding != nullptr);
            const auto from = IndexOn(slot, previous.Id());
            const auto to = IndexOn(slot, next.Id());
            const auto line = previous.Id() + " -> " + next.Id() + " " + slot.key + " " + std::to_string(from) + " -> " + std::to_string(to);
            CAPTURE(line);
            // Never silently wrong: whatever resolves or heals lands on the index maintainers fixed by hand.
            if (binding->Resolved()) CHECK(binding->offset == static_cast<std::int64_t>(to));
            switch (binding->status)
            {
                case bindings::BindingStatus::Resolved:
                    outcomes["resolved"].push_back(line);
                    break;
                case bindings::BindingStatus::Healed:
                    outcomes["healed"].push_back(line);
                    break;
                default:
                    outcomes["failed"].push_back(line + " (" + bindings::DescribeFailure(*binding) + ")");
            }
        }
    }

    for (const auto& [outcome, lines] : outcomes)
        if (outcome != "resolved")
            for (const auto& line : lines)
                std::cout << outcome << "  " << line << "\n";
    std::cout << outcomes["resolved"].size() << " resolved, " << outcomes["healed"].size() << " healed, " << outcomes["failed"].size()
              << " failed\n";

    // The six moves of 1.41.8.2 (fixed by hand then): five heal; ChangeTeam's own body changed too, so it fails.
    const std::vector<std::string> expectedHeals{
        "1.41.8.1 -> 1.41.8.2 CCSPlayerController_Respawn 272 -> 276",
        "1.41.8.1 -> 1.41.8.2 CCSGameRules_FindPickerEntity 27 -> 26",
        "1.41.8.1 -> 1.41.8.2 CBaseEntity_IsPlayerPawn 168 -> 170",
        "1.41.8.1 -> 1.41.8.2 CBaseEntity_Teleport 162 -> 164",
        "1.41.8.1 -> 1.41.8.2 CBasePlayerPawn_CommitSuicide 384 -> 387",
    };
    CHECK(outcomes["healed"] == expectedHeals);
    // Fail soft where the evidence is not enough: the moved ChangeTeam, and CheckTransmit where its body was recompiled.
    const std::vector<std::string> expectedFailures{
        "1.41.8.1 -> 1.41.8.2 ISource2GameEntities::CheckTransmit 13 -> 13 (fingerprint mismatch)",
        "1.41.8.1 -> 1.41.8.2 CCSPlayerController_ChangeTeam 102 -> 104 (fingerprint mismatch)",
        "1.41.9.6 -> 1.41.9.7 ISource2GameEntities::CheckTransmit 13 -> 13 (fingerprint mismatch)",
    };
    CHECK(outcomes["failed"] == expectedFailures);
}

TEST_CASE("archive: the generator replays the hand-maintained game-class offset history from 1.41.8.1 to 1.42.0.1: every move "
          "regenerated correctly or held, ending at the committed Gamedata (local)" *
          doctest::skip(!AllArchived()))
{
    auto gamedata = GamedataOn(BuildOf(kBuilds.front()));
    std::vector<std::string> moved;
    std::vector<std::string> held;
    for (std::size_t pair = 0; pair + 1 < kBuilds.size(); ++pair)
    {
        auto& previous = BuildOf(kBuilds[pair]);
        auto& next = BuildOf(kBuilds[pair + 1]);
        auto result = CarryForward(gamedata, previous.Vtables(), next.Vtables());

        CHECK(result.carried.size() + result.held.size() == kSlots.size());
        for (const auto& carried : result.carried)
        {
            const auto& slot = SlotOf(carried.key);
            const auto line = next.Id() + " " + carried.key + " " + std::to_string(carried.from) + " -> " + std::to_string(carried.to);
            CAPTURE(line);
            CHECK(carried.from == IndexOn(slot, previous.Id()));
            CHECK(carried.to == IndexOn(slot, next.Id())); // never wrong
            if (carried.from != carried.to) moved.push_back(line);
        }
        // A held Slot Binding is what a maintainer fixes by hand: re-bootstrap it at the index they found.
        for (const auto& entry : result.held)
        {
            held.push_back(next.Id() + " " + entry.key + ": " + entry.reason);
            result.gamedata[entry.key] = BootstrapOn(next, SlotOf(entry.key), result.gamedata[entry.key]);
        }
        gamedata = std::move(result.gamedata);
    }

    for (const auto& line : moved)
        std::cout << "moved  " << line << "\n";
    for (const auto& line : held)
        std::cout << "held   " << line << "\n";

    // The same evidence as the boot check: five of the six hand-fixed moves of 1.41.8.2 are regenerated, ChangeTeam is
    // held for a human, and so is CheckTransmit wherever its body was recompiled.
    CHECK(moved == std::vector<std::string>{
                       "1.41.8.2 CCSPlayerController_Respawn 272 -> 276",
                       "1.41.8.2 CCSGameRules_FindPickerEntity 27 -> 26",
                       "1.41.8.2 CBaseEntity_IsPlayerPawn 168 -> 170",
                       "1.41.8.2 CBaseEntity_Teleport 162 -> 164",
                       "1.41.8.2 CBasePlayerPawn_CommitSuicide 384 -> 387",
                   });
    REQUIRE(held.size() == 3);
    CHECK(held[0].starts_with("1.41.8.2 ISource2GameEntities::CheckTransmit: "));
    CHECK(held[1].starts_with("1.41.8.2 CCSPlayerController_ChangeTeam: "));
    CHECK(held[2].starts_with("1.41.9.7 ISource2GameEntities::CheckTransmit: "));

    const auto committed = CommittedGamedata();
    for (const auto& slot : kSlots)
    {
        CAPTURE(slot.key);
        CHECK(gamedata.at(slot.key) == committed.at(slot.key));
    }
}
