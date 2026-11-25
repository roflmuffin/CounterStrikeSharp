// The full real-build replay: every committed Slot Binding with a fingerprint window, over every consecutive pair of
// archived builds where its module is archived on both sides. Each pair: the Binding bootstrapped on the older build at
// its true index there, then resolved on the newer build by the boot-time Vtable Check. Local only: skipped when the
// archive lacks builds; Valve binaries are never committed.
//
// Truth: the index on 1.42.0.1 is the committed one (the e2e server's build). Earlier builds as the research oracle
// (`sim.py`, RELAX=uniq: global function matcher and whole-body alignment) and the hand-maintained Gamedata history
// found them: every ISource2Server slot one lower before 1.42.0.1 (the inserted virtual), the game-class virtuals that
// moved at 1.41.8.2 at their 1.41.8.1 index there, all else unmoved. engine2 and tier0 are archived only for
// 1.41.9.7 -> 1.42.0.1, where nothing we use moved.
//   ./native_tests -tc="contract: *" -s
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
#include "core/engine_features.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {

// Every build in the archive, oldest first.
const std::vector<std::string> kBuilds{ "1.41.8.1", "1.41.8.2", "1.41.9.0", "1.41.9.1a", "1.41.9.1b",
                                        "1.41.9.2", "1.41.9.6", "1.41.9.7", "1.42.0.1" };
constexpr const char* kCurrentBuild = "1.42.0.1";

// Indices on 1.41.8.1 that 1.41.8.2 moved (hand-fixed Gamedata 2f984988; the oracle agrees).
const std::map<std::string, std::size_t, std::less<>> kBefore1_41_8_2{
    { "CCSPlayerController_ChangeTeam", 102 }, { "CCSPlayerController_Respawn", 272 }, { "CCSGameRules_FindPickerEntity", 27 },
    { "CBaseEntity_IsPlayerPawn", 168 },       { "CBaseEntity_Teleport", 162 },        { "CBasePlayerPawn_CommitSuicide", 384 },
};

bool Archived(const std::string& build, const std::string& library)
{
    return FindArchivedModule(DefaultArchiveRoot(), build, library).has_value();
}

bool ServerArchived()
{
    return std::all_of(kBuilds.begin(), kBuilds.end(), [](const std::string& build) {
        return Archived(build, "server");
    });
}

bool CurrentBuildArchived()
{
    return Archived(kCurrentBuild, "server") && Archived(kCurrentBuild, "engine") && Archived(kCurrentBuild, "tier0");
}

GamedataJson Committed()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return GamedataJson::parse(stream);
}

struct SlotBinding
{
    std::string key;
    std::string className;
    std::string library;
    std::size_t committedIndex = 0;
};

// Every committed Slot Binding with a Linux fingerprint window (the ones verified at boot).
std::vector<SlotBinding> VerifiedSlotBindings(const GamedataJson& committed)
{
    std::vector<SlotBinding> slots;
    for (const auto& [key, entry] : committed.items())
    {
        if (!entry.contains("vtable") || !entry["vtable"].contains("linux") || !entry["vtable"]["linux"].contains("window")) continue;
        slots.push_back({ key, entry["vtable"]["class"].get<std::string>(), entry["vtable"]["library"].get<std::string>(),
                          entry["offsets"]["linux"].get<std::size_t>() });
    }
    return slots;
}

std::size_t TrueIndex(const SlotBinding& slot, const std::string& build)
{
    if (build == kCurrentBuild) return slot.committedIndex;
    if (slot.key.starts_with("ISource2Server::")) return slot.committedIndex - 1;
    if (build == "1.41.8.1")
        if (const auto moved = kBefore1_41_8_2.find(slot.key); moved != kBefore1_41_8_2.end()) return moved->second;
    return slot.committedIndex;
}

// One archived build with its class fingerprints computed once each.
class Build
{
  public:
    explicit Build(const std::string& id) : m_build(DefaultArchiveRoot(), id) {}

    [[nodiscard]] const std::string& Id() const { return m_build.Build(); }
    [[nodiscard]] bool Has(const std::string& library) const { return Archived(Id(), library); }
    [[nodiscard]] const LoadedModule& Module(const std::string& library) { return m_build.Module(library); }

    const std::vector<vtable::Fingerprint>& Fingerprints(const std::string& library, const std::string& className)
    {
        const auto key = library + "/" + className;
        auto it = m_fingerprints.find(key);
        if (it == m_fingerprints.end()) it = m_fingerprints.emplace(key, Module(library).Fingerprints(className)).first;
        return it->second;
    }

  private:
    ArchivedBuild m_build;
    std::map<std::string, std::vector<vtable::Fingerprint>> m_fingerprints;
};

bindings::ResolutionReport Resolve(const GamedataJson& gamedataJson, Build& build, const std::vector<std::string>& libraries)
{
    const auto gamedata = bindings::ParseGamedata(nlohmann::json::parse(gamedataJson.dump()), "linux");
    std::vector<bindings::BindingDeclaration> declarations;
    for (const auto& [key, entry] : gamedataJson.items())
        declarations.push_back({ key, bindings::BindingKind::Slot });
    bindings::ModuleImages images;
    for (const auto& library : libraries)
        images.emplace(library, build.Module(library).Image());
    return bindings::BuiltinResolver(bindings::Platform::Linux).Resolve(build.Id(), gamedata, images, declarations);
}

} // namespace

TEST_CASE("contract: every committed Slot Binding verifies at its Gamedata index on the current build: no Heal, no failure, "
          "no size warning; only those without a window are unverified (local)" *
          doctest::skip(!CurrentBuildArchived()))
{
    const auto committed = Committed();
    const auto gamedata = bindings::ParseGamedata(nlohmann::json::parse(committed.dump()), "linux");
    // What the boot resolves: the core's declarations, Slot Bindings only (signatures need the live process).
    std::vector<bindings::BindingDeclaration> slots;
    for (const auto& declaration : bindings::CoreBindingDeclarations(gamedata))
        if (declaration.kind == bindings::BindingKind::Slot) slots.push_back(declaration);

    Build build(kCurrentBuild);
    bindings::ModuleImages images;
    for (const char* library : { "server", "engine", "tier0" })
        images.emplace(library, build.Module(library).Image());
    const auto report = bindings::BuiltinResolver(bindings::Platform::Linux).Resolve(kCurrentBuild, gamedata, images, slots);

    std::size_t verified = 0;
    std::vector<std::string> unverified;
    for (const auto& binding : report.Bindings())
    {
        CAPTURE(binding.key);
        const auto& entry = committed.at(binding.key);
        if (!entry["vtable"]["linux"].contains("window"))
        {
            CHECK(binding.status == bindings::BindingStatus::Unverified);
            unverified.push_back(binding.key);
            continue;
        }
        ++verified;
        CHECK(binding.status == bindings::BindingStatus::Resolved);
        CHECK(binding.offset == entry["offsets"]["linux"].get<std::int64_t>());
    }
    CHECK(bindings::FormatHeals(report).empty());
    CHECK(bindings::FormatVtableSizeWarnings(report).empty());
    // Only modules the archive cannot fingerprint yet (network messages, schema system) are unverified.
    for (const auto& key : unverified)
    {
        CAPTURE(key);
        CHECK((key.starts_with("INetworkMessages::") || key.starts_with("CSchemaSystem::")));
    }
    std::cout << "contract: " << verified << " Slot Bindings verified on " << kCurrentBuild << ", " << unverified.size()
              << " unverified\n";
}

TEST_CASE("contract: the full real-build replay over every archived pair resolves no committed Slot Binding wrong; every "
          "known move heals or fails (local)" *
          doctest::skip(!ServerArchived()))
{
    const auto committed = Committed();
    const auto slots = VerifiedSlotBindings(committed);
    REQUIRE_FALSE(slots.empty());

    std::size_t resolved = 0, healed = 0, failed = 0, wrong = 0, moves = 0, movesHealed = 0;
    std::map<std::string, std::size_t> pairsPerLibrary;
    auto previous = std::make_unique<Build>(kBuilds.front());
    for (std::size_t next = 1; next < kBuilds.size(); ++next)
    {
        auto current = std::make_unique<Build>(kBuilds[next]);
        const auto& from = previous->Id();
        const auto& to = current->Id();

        // Each Binding bootstrapped on the older build at its true index there.
        GamedataJson gamedata = GamedataJson::object();
        std::vector<std::string> libraries;
        for (const auto& slot : slots)
        {
            if (!previous->Has(slot.library) || !current->Has(slot.library)) continue;
            auto entry = committed.at(slot.key);
            const auto index = TrueIndex(slot, from);
            entry["offsets"]["linux"] = index;
            gamedata[slot.key] =
                BootstrapSlotBinding(previous->Fingerprints(slot.library, slot.className), { slot.key, slot.className, slot.library, index },
                                     entry)
                    .entry;
            if (std::find(libraries.begin(), libraries.end(), slot.library) == libraries.end()) libraries.push_back(slot.library);
        }
        for (const auto& library : libraries)
            ++pairsPerLibrary[library];

        const auto report = Resolve(gamedata, *current, libraries);
        for (const auto& binding : report.Bindings())
        {
            const auto& slot = *std::find_if(slots.begin(), slots.end(), [&](const SlotBinding& s) {
                return s.key == binding.key;
            });
            const auto truth = TrueIndex(slot, to);
            const bool moved = TrueIndex(slot, from) != truth;
            CAPTURE(binding.key);
            CAPTURE(from);
            CAPTURE(to);
            CAPTURE(truth);
            moves += moved;
            if (binding.status == bindings::BindingStatus::Failed)
            {
                ++failed;
                std::cout << "  failed " << binding.key << " " << from << " -> " << to << " (truth " << truth << ")\n";
                continue;
            }
            const bool right = binding.offset == static_cast<std::int64_t>(truth);
            CHECK(right);
            CHECK(binding.status != bindings::BindingStatus::Unverified);
            if (!right) ++wrong;
            else if (binding.status == bindings::BindingStatus::Healed)
                ++healed, movesHealed += moved;
            else
                ++resolved;
            CHECK_MESSAGE((!moved || binding.status == bindings::BindingStatus::Healed), "a known move resolved in place");
        }
        previous = std::move(current);
    }

    std::cout << "contract replay over " << kBuilds.size() - 1 << " pairs (";
    for (const auto& [library, pairs] : pairsPerLibrary)
        std::cout << library << " " << pairs << " ";
    std::cout << "): " << resolved << " resolved, " << healed << " healed (" << movesHealed << " of " << moves << " known moves), "
              << failed << " failed, " << wrong << " wrong\n";
    CHECK(wrong == 0);
    CHECK(pairsPerLibrary["server"] == kBuilds.size() - 1);
    CHECK(movesHealed > 0);
}
