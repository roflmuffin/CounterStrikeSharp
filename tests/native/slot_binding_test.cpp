// Slot Bindings through the resolver seam, on synthetic vtables: Gamedata recorded on one build, resolved on another.
#include <doctest.h>

#include <cstdint>
#include <fstream>
#include <tuple>
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

// A larger build for Heals: bodies 1..40, so a slot in the middle can move up to the search range either way.
std::vector<int> Bodies(int count)
{
    std::vector<int> bodies;
    for (int id = 1; id <= count; ++id)
        bodies.push_back(id);
    return bodies;
}

// bodies with count new virtuals (ids from 100) inserted at position.
std::vector<int> Inserted(std::vector<int> bodies, std::size_t position, int count)
{
    for (int i = 0; i < count; ++i)
        bodies.insert(bodies.begin() + static_cast<std::ptrdiff_t>(position), 100 + i);
    return bodies;
}

// bodies with count virtuals removed at position.
std::vector<int> Removed(std::vector<int> bodies, std::size_t position, int count)
{
    bodies.erase(bodies.begin() + static_cast<std::ptrdiff_t>(position), bodies.begin() + static_cast<std::ptrdiff_t>(position) + count);
    return bodies;
}
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
    // At its stated index the slot has not moved: a vtable grown past it still verifies it.
    CHECK(Resolve({ { kKey, SlotEntry(build, 7) } }, FakeBuild({ 1, 2, 3, 4, 5, 6, 7, 8, 60 })).slot->match == vtable::SlotMatch::Window);
}

TEST_CASE("A Heal needs the whole recorded window: a neighbour recorded outside the vtable is outside it at the new index too")
{
    const json gamedata{ { kKey, SlotEntry(FakeBuild(kOldBuild), 7) } }; // the last slot: window [7, 8, null]

    SUBCASE("moved by an insert and still the last slot: heals")
    {
        const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 100, 3, 4, 5, 6, 7, 8 }));
        CHECK(result.status == BindingStatus::Healed);
        CHECK(result.offset == 8);
    }
    SUBCASE("moved by an insert while the vtable also grew past it: two of three fingerprints are not enough")
    {
        const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 100, 3, 4, 5, 6, 7, 8, 60 }));
        CHECK(result.status == BindingStatus::Failed);
        CHECK(result.reason == FailureReason::FingerprintMismatch);
    }
    SUBCASE("the first slot never heals: nothing can be outside the vtable before any other index")
    {
        const json first{ { kKey, SlotEntry(FakeBuild(kOldBuild), 0) } }; // window [null, 1, 2]
        const auto result = Resolve(first, FakeBuild({ 100, 1, 2, 3, 4, 5, 6, 7, 8 }));
        CHECK(result.status == BindingStatus::Failed);
        CHECK(result.reason == FailureReason::FingerprintMismatch);
    }
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

TEST_CASE("On Windows a game-class Slot Binding is unverified at its Gamedata index; without one it is missing for this OS")
{
    // Game-class virtuals have no compiled header index to fall back on, unlike interface methods.
    const BindingDeclaration declaration{ kKey, BindingKind::Slot, {}, false };
    const auto resolve = [&](const json& gamedata) {
        const auto report = BuiltinResolver(Platform::Windows)
                                .Resolve("b", ParseGamedata(gamedata, "windows"), { { "server", FakeBuild({ 9, 9, 9 }).Image() } },
                                         std::span(&declaration, 1));
        return report.Bindings().at(0);
    };
    auto gamedata = json{ { kKey, SlotEntry(FakeBuild(kOldBuild), 4) } };
    gamedata[kKey]["offsets"]["windows"] = 3;

    const auto unverified = resolve(gamedata);
    CHECK(unverified.status == BindingStatus::Unverified);
    CHECK(unverified.offset == 3);

    gamedata[kKey]["offsets"].erase("windows");
    const auto missing = resolve(gamedata);
    CHECK(missing.status == BindingStatus::Failed);
    CHECK(missing.reason == FailureReason::MissingForThisOs);
    CHECK(resolve(json::object()).reason == FailureReason::MissingFromGamedata);
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
    // Only Gamedata declares it, so no native code calls it through a compiled method: no header index to fall back on.
    const auto someSlot = std::find_if(declarations.begin(), declarations.end(), [](const BindingDeclaration& d) {
        return d.key == "Some_Slot";
    });
    CHECK_FALSE(someSlot->headerIndex);
}

TEST_CASE("Game-class virtuals are Slot Bindings without a header index, even in Gamedata without their vtable fields")
{
    // e.g. an auto-updated gamedata.json from before Slot Bindings: still a Slot Binding, unverified at its index.
    const FakeBuild build(kOldBuild);
    const json upstream{ { "CBasePlayerPawn_CommitSuicide", { { "offsets", { { "linux", 4 }, { "windows", 4 } } } } },
                         { "CTakeDamageInfo_HitGroup", { { "offsets", { { "linux", 104 }, { "windows", 104 } } } } } };
    const auto gamedata = ParseGamedata(upstream, "linux");

    const auto declarations = CoreBindingDeclarations(gamedata);
    const auto find = [&](std::string_view key) {
        const auto it = std::find_if(declarations.begin(), declarations.end(), [&](const BindingDeclaration& d) {
            return d.key == key;
        });
        REQUIRE(it != declarations.end());
        return *it;
    };
    for (const auto* key : { "ISource2GameEntities::CheckTransmit", "CCSPlayerController_ChangeTeam", "CCSPlayerController_Respawn",
                             "CCSPlayer_ItemServices_GiveNamedItem", "CCSPlayer_ItemServices_DropActivePlayerWeapon",
                             "CCSPlayer_ItemServices_RemoveWeapons", "CGameSceneNode_GetSkeletonInstance", "CCSGameRules_FindPickerEntity",
                             "CBaseEntity_IsPlayerPawn", "CBaseEntity_Teleport", "CBasePlayerPawn_CommitSuicide" })
    {
        CAPTURE(key);
        CHECK(find(key).kind == BindingKind::Slot);
        CHECK_FALSE(find(key).headerIndex);
    }
    // Struct field offsets are not Slot Bindings.
    CHECK(find("CTakeDamageInfo_HitGroup").kind == BindingKind::Offset);
    // Interface methods have a compiled header index (Windows falls back to it).
    CHECK(find("ISource2Server::GameFrame").headerIndex);

    const auto report = ResolveBindings("b", gamedata, { { "server", build.Image() } }, declarations);
    const auto* commitSuicide = report.Find("CBasePlayerPawn_CommitSuicide");
    REQUIRE(commitSuicide != nullptr);
    CHECK(commitSuicide->status == BindingStatus::Unverified);
    CHECK(commitSuicide->offset == 4);
}

TEST_CASE("A Slot Binding whose window moved within the search range heals at the new index, with evidence")
{
    const FakeBuild old(Bodies(40));
    const json gamedata{ { kKey, SlotEntry(old, 20) } };
    const auto recordedWindow = vtable::RecordSlot(old.Fingerprints(), 20).window;

    std::vector<int> live;
    std::int64_t expected = 0;
    SUBCASE("one virtual inserted before the slot (as at 1.42.0.1)")
    {
        live = Inserted(Bodies(40), 5, 1);
        expected = 21;
    }
    SUBCASE("three inserted")
    {
        live = Inserted(Bodies(40), 5, 3);
        expected = 23;
    }
    SUBCASE("sixteen inserted: the edge of the search range")
    {
        live = Inserted(Bodies(40), 5, 16);
        expected = 36;
    }
    SUBCASE("one removed before the slot")
    {
        live = Removed(Bodies(40), 5, 1);
        expected = 19;
    }
    SUBCASE("sixteen removed")
    {
        live = Removed(Bodies(40), 2, 16);
        expected = 4;
    }

    const auto result = Resolve(gamedata, FakeBuild(live));

    CHECK(result.status == BindingStatus::Healed);
    CHECK(result.Resolved());
    CHECK(result.reason == FailureReason::None);
    CHECK(result.offset == expected);
    REQUIRE(result.slot);
    CHECK(result.slot->statedIndex == 20);
    CHECK(result.slot->match == vtable::SlotMatch::Heal);
    REQUIRE(result.slot->window);
    CHECK(*result.slot->window == recordedWindow);
}

TEST_CASE("A window that matches more than once nearby fails as an ambiguous heal")
{
    const json gamedata{ { kKey, SlotEntry(FakeBuild(kOldBuild), 4) } }; // window: bodies 4, 5, 6

    // An insert before the slot, and a second copy of the window's three bodies further on.
    const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 3, 50, 4, 5, 6, 7, 4, 5, 6, 8 }));

    CHECK(result.status == BindingStatus::Failed);
    CHECK(result.reason == FailureReason::AmbiguousHeal);
    CHECK(result.matchCount == 2);
    CHECK(DescribeFailure(result) == "ambiguous heal (2 matches)");
    CHECK(result.slot->match == vtable::SlotMatch::None);
}

TEST_CASE("A shift across identical function bodies fails, never heals")
{
    // Slot 4 shares its body (9) with slot 3: after one more copy is inserted, which copy moved is unknowable, even
    // though the window [9, 9, 6] now matches exactly once, one slot later.
    const json gamedata{ { kKey, SlotEntry(FakeBuild({ 1, 2, 3, 9, 9, 6, 7, 8 }), 4) } };

    const auto result = Resolve(gamedata, FakeBuild({ 1, 2, 3, 9, 9, 9, 6, 7, 8 }));

    CHECK(result.status == BindingStatus::Failed);
    CHECK(result.reason == FailureReason::FingerprintMismatch);
}

TEST_CASE("A window that moved beyond the search range fails with fingerprint mismatch")
{
    const json gamedata{ { kKey, SlotEntry(FakeBuild(Bodies(40)), 20) } };

    const auto result = Resolve(gamedata, FakeBuild(Inserted(Bodies(40), 5, 17)));

    CHECK(result.status == BindingStatus::Failed);
    CHECK(result.reason == FailureReason::FingerprintMismatch);
}

TEST_CASE("A healed Slot Binding counts as available, and the report shows the Heal with its evidence")
{
    const FakeBuild old(Bodies(40));
    const auto fingerprints = old.Fingerprints();
    const EngineFeatureDeclaration feature{ "cssharp.server/game-frame", { { kKey, BindingKind::Slot } } };

    const auto report = ResolveBindings("b", ParseGamedata({ { kKey, SlotEntry(old, 20) } }, "linux"),
                                        { { "server", FakeBuild(Inserted(Bodies(40), 5, 1)).Image() } }, feature.bindings);

    CHECK(EvaluateEngineFeature(report, feature).available);
    CHECK(DescribeUnavailable(report, feature) == "Engine Feature cssharp.server/game-frame is available");
    CHECK(FormatBindingSummary(report) == "Bindings for build b: all 1 resolved, 1 healed");

    const auto heals = FormatHeals(report);
    REQUIRE(heals.size() == 1);
    CHECK(heals[0] == "Healed ISource2Server::GameFrame: CSource2Server (server) slot 20 -> 21, window [" + fingerprints[19] + ", " +
                          fingerprints[20] + ", " + fingerprints[21] + "] matched once within 16 slots");

    const auto binding = json::parse(ResolutionReportToJson(report, {}))["bindings"][0];
    CHECK(binding["status"] == "Healed");
    CHECK(binding["resolved"] == true);
    CHECK(binding["reason"] == "None");
    CHECK(binding["offset"] == 21);
    CHECK(binding["slot"]["statedIndex"] == 20);
    CHECK(binding["slot"]["match"] == "Heal");
    CHECK(binding["slot"]["window"] == json::array({ fingerprints[19], fingerprints[20], fingerprints[21] }));
}

TEST_CASE("The summary counts Heals next to failures, and a neighbour outside the vtable prints as null")
{
    const FakeBuild old(kOldBuild);
    const std::vector<BindingDeclaration> declarations{ { kKey, BindingKind::Slot }, { "Missing", BindingKind::Slot } };
    const json gamedata{ { kKey, SlotEntry(old, 7) }, { "Missing", { { "offsets", json::object() } } } };

    const auto report =
        ResolveBindings("b", ParseGamedata(gamedata, "linux"), { { "server", FakeBuild({ 1, 50, 2, 3, 4, 5, 6, 7, 8 }).Image() } }, declarations);

    CHECK(FormatBindingSummary(report) == "Bindings for build b: 1 of 2 failed: Missing (missing for this OS); 1 healed");
    const auto heals = FormatHeals(report);
    REQUIRE(heals.size() == 1);
    CHECK(heals[0].starts_with("Healed ISource2Server::GameFrame: CSource2Server (server) slot 7 -> 8, window ["));
    CHECK(heals[0].find(", null] matched once") != std::string::npos);
}

TEST_CASE("Committed Gamedata: every game-class virtual carries its Slot Binding fields; struct field offsets are unchanged")
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    const auto committed = json::parse(stream);

    std::size_t gameClassSlots = 0;
    for (const auto& declaration : CoreBindingDeclarations({}))
    {
        if (declaration.kind != BindingKind::Slot || declaration.headerIndex) continue;
        CAPTURE(declaration.key);
        ++gameClassSlots;
        const auto& entry = committed.at(declaration.key);
        CHECK(entry["offsets"]["linux"].is_number_integer());
        CHECK(entry["offsets"]["windows"].is_number_integer()); // Windows: unverified at its Gamedata index
        CHECK(entry["vtable"]["class"].is_string());
        CHECK(entry["vtable"]["library"] == "server");
        CHECK(entry["vtable"]["linux"]["window"].size() == 3);
        CHECK(entry["vtable"]["linux"]["size"].is_number_unsigned());
        CHECK_FALSE(entry["vtable"].contains("windows"));
    }
    CHECK(gameClassSlots == 11);

    // Struct field and other plain offsets: no Slot Binding fields, values as before.
    const std::vector<std::tuple<const char*, int, int>> plainOffsets{
        { "CTakeDamageInfo_HitGroup", 104, 104 }, { "CheckTransmitPlayerSlot", 576, 576 }, { "GameEntitySystem", 80, 88 },
        { "GameEventManager", 93, 93 },          { "SetStateChanged", 29, 28 },
    };
    for (const auto& [key, linuxOffset, windowsOffset] : plainOffsets)
    {
        CAPTURE(key);
        const auto& entry = committed.at(key);
        CHECK_FALSE(entry.contains("vtable"));
        CHECK(entry["offsets"]["linux"] == linuxOffset);
        CHECK(entry["offsets"]["windows"] == windowsOffset);
    }
}
