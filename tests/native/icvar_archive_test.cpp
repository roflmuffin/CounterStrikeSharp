// Real-build tests of the ICvar Slot Bindings (CCvar in libtier0) against the shared build archive. Local only:
// skipped when the archive lacks the tier0 modules; Valve binaries are never committed.
//   ./native_tests -tc="archive: ICvar*" -s
#include <doctest.h>

#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "build_archive.h"
#include "core/binding_resolver.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {
bool ArchivedTier0(const char* build) { return FindArchivedModule(DefaultArchiveRoot(), build, "tier0").has_value(); }

GamedataJson Committed()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return GamedataJson::parse(stream);
}

// The committed ICvar Slot Bindings only, keyed as in Gamedata.
GamedataJson CommittedIcvar()
{
    const auto committed = Committed();
    GamedataJson icvar = GamedataJson::object();
    for (const auto& [key, entry] : committed.items())
        if (key.rfind("ICvar::", 0) == 0) icvar[key] = entry;
    return icvar;
}

bindings::ResolutionReport ResolveOnTier0(const LoadedModule& tier0, const GamedataJson& gamedataJson)
{
    const auto gamedata = bindings::ParseGamedata(nlohmann::json::parse(gamedataJson.dump()), "linux");
    std::vector<bindings::BindingDeclaration> declarations;
    for (const auto& [key, entry] : gamedataJson.items())
        declarations.push_back({ key, bindings::BindingKind::Slot });
    return bindings::BuiltinResolver(bindings::Platform::Linux).Resolve("archive", gamedata, { { "tier0", tier0.Image() } }, declarations);
}

// One Slot Binding per slot of CCvar on a build, keyed "CCvar::<index>".
GamedataJson EveryCcvarSlot(const LoadedModule& tier0)
{
    const auto fingerprints = tier0.Fingerprints("CCvar");
    GamedataJson gamedata = GamedataJson::object();
    for (std::size_t index = 0; index < fingerprints.size(); ++index)
    {
        const auto key = "CCvar::" + std::to_string(index);
        gamedata[key] = BootstrapSlotBinding(fingerprints, { key, "CCvar", "tier0", index }).entry;
    }
    return gamedata;
}
} // namespace

TEST_CASE("archive: ICvar committed Slot Bindings resolve at their index on 1.42.0.1 and 1.41.9.7, by window (local)" *
          doctest::skip(!ArchivedTier0("1.42.0.1") || !ArchivedTier0("1.41.9.7")))
{
    const auto committed = CommittedIcvar();
    REQUIRE(committed.size() == 16);

    for (const char* build : { "1.42.0.1", "1.41.9.7" })
    {
        CAPTURE(build);
        const auto tier0 = LoadedModule::FromArchive(DefaultArchiveRoot(), build, "tier0");
        const auto report = ResolveOnTier0(tier0, committed);

        for (const auto& binding : report.Bindings())
        {
            CAPTURE(binding.key);
            CHECK(binding.status == bindings::BindingStatus::Resolved);
            CHECK(binding.offset == committed[binding.key]["offsets"]["linux"].get<std::int64_t>());
            REQUIRE(binding.slot);
            CHECK(binding.slot->match == vtable::SlotMatch::Window);
            CHECK(binding.slot->className == "CCvar");
            CHECK(binding.slot->vtableSize == 46);
        }
        CHECK(bindings::FormatVtableSizeWarnings(report).empty());
        CHECK(bindings::FormatHeals(report).empty());
    }
}

TEST_CASE("archive: ICvar Slot Bindings bootstrapped on 1.41.9.7 carry forward to exactly the committed 1.42.0.1 entries "
          "(local)" *
          doctest::skip(!ArchivedTier0("1.42.0.1") || !ArchivedTier0("1.41.9.7")))
{
    const auto committed = CommittedIcvar();
    const auto old = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.41.9.7", "tier0");
    const auto fingerprints = old.Fingerprints("CCvar");

    // The SDK header indices (the same in the pinned SDK and the e2e SDK: ICvar did not change between the two builds).
    GamedataJson bootstrapped = GamedataJson::object();
    for (const auto& [key, entry] : committed.items())
    {
        const auto index = entry["offsets"]["linux"].get<std::size_t>();
        const auto generated = BootstrapSlotBinding(fingerprints, { key, "CCvar", "tier0", index });
        CHECK_MESSAGE(generated.warnings.empty(), key);
        bootstrapped[key] = generated.entry;
    }

    ArchivedBuild previous(DefaultArchiveRoot(), "1.41.9.7");
    ArchivedBuild next(DefaultArchiveRoot(), "1.42.0.1");
    const auto result = CarryForward(bootstrapped, previous.Vtables(), next.Vtables());

    CHECK(result.held.empty());
    for (const auto& carried : result.carried)
        CHECK_MESSAGE(carried.from == carried.to, carried.key);
    CHECK(result.gamedata == committed);
}

TEST_CASE("archive: an ICvar Slot Binding one slot off heals on the real tier0; a corrupted fingerprint fails (local)" *
          doctest::skip(!ArchivedTier0("1.42.0.1")))
{
    const auto tier0 = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.42.0.1", "tier0");

    // Console dispatch stated one slot too low (as if a virtual had been inserted before it), window unchanged.
    auto gamedata = GamedataJson::object();
    gamedata["ICvar::DispatchConCommand"] = CommittedIcvar()["ICvar::DispatchConCommand"];
    gamedata["ICvar::DispatchConCommand"]["offsets"]["linux"] = 19;
    auto report = ResolveOnTier0(tier0, gamedata);
    auto dispatch = report.Bindings().at(0);
    CHECK(dispatch.status == bindings::BindingStatus::Healed);
    CHECK(dispatch.offset == 20);
    CHECK(dispatch.slot->statedIndex == 19);
    REQUIRE(bindings::FormatHeals(report).size() == 1);
    MESSAGE(bindings::FormatHeals(report)[0]);

    // The change listener's own fingerprint corrupted: nothing matches it anywhere, so it fails rather than guess.
    gamedata = GamedataJson::object();
    gamedata["ICvar::InstallGlobalChangeCallback"] = CommittedIcvar()["ICvar::InstallGlobalChangeCallback"];
    gamedata["ICvar::InstallGlobalChangeCallback"]["vtable"]["linux"]["window"][1] = "0000000000000000";
    report = ResolveOnTier0(tier0, gamedata);
    const auto& install = report.Bindings().at(0);
    CHECK(install.status == bindings::BindingStatus::Failed);
    CHECK(install.reason == bindings::FailureReason::FingerprintMismatch);
}

TEST_CASE("archive: no CCvar slot resolves wrong from 1.41.9.7 to 1.42.0.1, at its index or one slot off (local)" *
          doctest::skip(!ArchivedTier0("1.42.0.1") || !ArchivedTier0("1.41.9.7")))
{
    // CCvar kept its layout between these builds, so slot i is slot i. Every slot is bootstrapped on 1.41.9.7 and
    // resolved on 1.42.0.1 at its index and, to simulate an insert or removal, one slot off either way. A resolution is
    // wrong if it lands anywhere but i; a failure is allowed (the check never guesses).
    const auto old = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.41.9.7", "tier0");
    const auto live = LoadedModule::FromArchive(DefaultArchiveRoot(), "1.42.0.1", "tier0");
    const auto everySlot = EveryCcvarSlot(old);

    std::size_t resolved = 0, healed = 0, failed = 0, wrong = 0;
    for (const int shift : { 0, -1, 1 })
    {
        GamedataJson shifted = everySlot;
        for (auto& [key, entry] : shifted.items())
        {
            const auto index = entry["offsets"]["linux"].get<std::int64_t>();
            entry["offsets"]["linux"] = index + shift;
        }
        const auto report = ResolveOnTier0(live, shifted);
        for (const auto& binding : report.Bindings())
        {
            const auto truth = everySlot[binding.key]["offsets"]["linux"].get<std::int64_t>();
            if (!binding.Resolved()) ++failed;
            else if (binding.offset != truth) ++wrong;
            else if (binding.status == bindings::BindingStatus::Healed) ++healed;
            else ++resolved;
            if (binding.Resolved()) CHECK_MESSAGE(binding.offset == truth, binding.key << " shifted by " << shift);
        }
    }
    std::cout << "CCvar 1.41.9.7 -> 1.42.0.1 over shifts 0/-1/+1: " << resolved << " resolved, " << healed << " healed, "
              << failed << " failed, " << wrong << " wrong\n";
    CHECK(wrong == 0);
    CHECK(resolved == everySlot.size());
}
