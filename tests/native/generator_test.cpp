// The Gamedata generator on synthetic vtables: what its bootstrap emits is what the resolver reads; carry-forward moves
// a Slot Binding only when its window matches exactly once on the next build.
#include <doctest.h>

#include <unistd.h>

#include <filesystem>
#include <map>
#include <stdexcept>
#include <string>
#include <vector>

#include "build_archive.h"
#include "core/binding_resolver.h"
#include "generator.h"

using namespace counterstrikesharp;
using namespace counterstrikesharp::generator;

namespace {
std::vector<vtable::Fingerprint> Vtable(std::initializer_list<const char*> fingerprints) { return { fingerprints.begin(), fingerprints.end() }; }

const SlotBindingRequest kGameFrame{ "ISource2Server::GameFrame", "CSource2Server", "server", 2 };
} // namespace

TEST_CASE("Bootstrap emits a Slot Binding entry the resolver reads back as recorded")
{
    const auto vtable = Vtable({ "a", "b", "c", "d" });

    const auto generated = BootstrapSlotBinding(vtable, kGameFrame);

    CHECK(generated.warnings.empty());
    CHECK(generated.entry == GamedataJson::parse(R"({
        "offsets": { "linux": 2 },
        "vtable": { "class": "CSource2Server", "library": "server",
                    "linux": { "window": ["b", "c", "d"], "uniqueNearby": true, "size": 4 } } })"));

    const auto parsed = bindings::ParseGamedata(nlohmann::json::parse(GamedataJson{ { kGameFrame.key, generated.entry } }.dump()), "linux");
    const auto& entry = parsed.at(kGameFrame.key);
    CHECK(entry.offset == 2);
    REQUIRE(entry.slot);
    CHECK(entry.slot->className == "CSource2Server");
    CHECK(entry.slot->library == "server");
    CHECK(entry.slot->window == vtable::WindowAt(vtable, 2));
    CHECK(entry.slot->uniqueNearby);
    CHECK(entry.slot->vtableSize == 4u);
}

TEST_CASE("Bootstrap marks a neighbour outside the vtable as null")
{
    const auto generated = BootstrapSlotBinding(Vtable({ "a", "b" }), { "K", "C", "server", 0 });
    CHECK(generated.entry["vtable"]["linux"]["window"] == GamedataJson::parse(R"([null, "a", "b"])"));
}

TEST_CASE("Bootstrap keeps the rest of an existing entry")
{
    const auto existing = GamedataJson::parse(R"({ "offsets": { "windows": 19, "linux": 1 }, "note": "kept" })");

    const auto generated = BootstrapSlotBinding(Vtable({ "a", "b", "c", "d" }), kGameFrame, existing);

    CHECK(generated.entry["offsets"]["windows"] == 19);
    CHECK(generated.entry["offsets"]["linux"] == 2);
    CHECK(generated.entry["note"] == "kept");
    // Key order is kept, so a regenerated file reviews as a small diff.
    CHECK(generated.entry.begin().key() == "offsets");
    CHECK(generated.entry["offsets"].begin().key() == "windows");
}

TEST_CASE("Bootstrap warns when the window also matches nearby (identical bodies), and never records outside the vtable")
{
    const auto generated = BootstrapSlotBinding(Vtable({ "x", "x", "x", "x", "x" }), kGameFrame);
    REQUIRE(generated.warnings.size() == 1);
    CHECK(generated.warnings[0].find("also matches at 1, 3") != std::string::npos);
    CHECK_FALSE(generated.entry["vtable"]["linux"]["uniqueNearby"].get<bool>());

    CHECK_THROWS_AS(BootstrapSlotBinding(Vtable({ "a", "b" }), kGameFrame), std::runtime_error);
}

// --- Carry-forward ---

namespace {
// One build's vtables, by class (library ignored).
struct FakeBuild
{
    std::map<std::string, std::vector<vtable::Fingerprint>, std::less<>> classes;

    [[nodiscard]] VtableFingerprints Source() const
    {
        return [this](std::string_view, std::string_view className) {
            const auto it = classes.find(className);
            if (it == classes.end()) throw std::runtime_error("no class " + std::string(className));
            return it->second;
        };
    }
};

FakeBuild ServerBuild(std::initializer_list<const char*> fingerprints) { return { { { "CSource2Server", Vtable(fingerprints) } } }; }

// Gamedata holding GameFrame bootstrapped at index on build.
GamedataJson GamedataWithGameFrame(const FakeBuild& build, std::size_t index)
{
    return GamedataJson{
        { kGameFrame.key, BootstrapSlotBinding(build.classes.at("CSource2Server"), { kGameFrame.key, "CSource2Server", "server", index }).entry }
    };
}
} // namespace

TEST_CASE("Carry-forward moves a Slot Binding past an inserted virtual and refreshes its window and size")
{
    const auto previous = ServerBuild({ "a", "b", "c", "d", "e" });
    const auto next = ServerBuild({ "a", "new", "b", "c", "d", "e" });

    const auto result = CarryForward(GamedataWithGameFrame(previous, 2), previous.Source(), next.Source());

    CHECK(result.held.empty());
    REQUIRE(result.carried.size() == 1);
    CHECK(result.carried[0].key == kGameFrame.key);
    CHECK(result.carried[0].from == 2);
    CHECK(result.carried[0].to == 3);
    CHECK(result.gamedata.at(kGameFrame.key) == GamedataJson::parse(R"({
        "offsets": { "linux": 3 },
        "vtable": { "class": "CSource2Server", "library": "server",
                    "linux": { "window": ["b", "c", "d"], "uniqueNearby": true, "size": 6 } } })"));
}

TEST_CASE("Carry-forward keeps the last slot where it is when the vtable grew past it, and moves it when it is still last")
{
    const auto previous = ServerBuild({ "a", "b", "c", "d", "e" });
    const auto gamedata = GamedataWithGameFrame(previous, 4); // window [d, e, null]

    const auto grown = CarryForward(gamedata, previous.Source(), ServerBuild({ "a", "b", "c", "d", "e", "f" }).Source());
    REQUIRE(grown.carried.size() == 1);
    CHECK(grown.carried[0].to == 4);

    const auto moved = CarryForward(gamedata, previous.Source(), ServerBuild({ "a", "new", "b", "c", "d", "e" }).Source());
    REQUIRE(moved.carried.size() == 1);
    CHECK(moved.carried[0].to == 5);
}

TEST_CASE("Carry-forward onto an unchanged build changes nothing")
{
    const auto build = ServerBuild({ "a", "b", "c", "d", "e" });
    const auto gamedata = GamedataWithGameFrame(build, 2);

    const auto result = CarryForward(gamedata, build.Source(), build.Source());

    CHECK(result.held.empty());
    REQUIRE(result.carried.size() == 1);
    CHECK(result.carried[0].from == result.carried[0].to);
    CHECK(result.gamedata == gamedata);
}

TEST_CASE("Carry-forward holds a Slot Binding whose window is ambiguous or gone on the next build, leaving its entry unchanged")
{
    const auto previous = ServerBuild({ "a", "b", "c", "d", "e" });
    const auto gamedata = GamedataWithGameFrame(previous, 2);

    SUBCASE("the window matches twice")
    {
        const auto next = ServerBuild({ "b", "c", "d", "x", "b", "c", "d" });
        const auto result = CarryForward(gamedata, previous.Source(), next.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].key == kGameFrame.key);
        CHECK(result.held[0].reason.find("ambiguous: its window matches at 1, 5") != std::string::npos);
        CHECK(result.carried.empty());
        CHECK(result.gamedata == gamedata);
    }
    SUBCASE("the slot's body changed")
    {
        const auto next = ServerBuild({ "a", "b", "c2", "d", "e" });
        const auto result = CarryForward(gamedata, previous.Source(), next.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].reason.find("matches nowhere") != std::string::npos);
        CHECK(result.gamedata == gamedata);
    }
    SUBCASE("a neighbour changed: not carried even though the boot check would still resolve it by its own fingerprint")
    {
        const auto next = ServerBuild({ "a", "b2", "c", "d", "e" });
        const auto result = CarryForward(gamedata, previous.Source(), next.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.gamedata == gamedata);
    }
    SUBCASE("the last slot, moved while the vtable grew past it: its window's edge no longer matches")
    {
        const auto last = GamedataWithGameFrame(previous, 4); // window [d, e, null]
        const auto next = ServerBuild({ "a", "new", "b", "c", "d", "e", "f" });
        const auto result = CarryForward(last, previous.Source(), next.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].reason.find("matches nowhere") != std::string::npos);
        CHECK(result.gamedata == last);
    }
    SUBCASE("the class is gone")
    {
        const FakeBuild next{ { { "COther", Vtable({ "a", "b", "c", "d", "e" }) } } };
        const auto result = CarryForward(gamedata, previous.Source(), next.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].reason.find("next build: no class CSource2Server") != std::string::npos);
        CHECK(result.gamedata == gamedata);
    }
}

TEST_CASE("Carry-forward holds a Slot Binding it cannot align on the previous build, leaving its entry unchanged")
{
    SUBCASE("no window (e.g. Gamedata replaced by an auto-update)")
    {
        const auto gamedata = GamedataJson::parse(R"({ "ISource2Server::GameFrame": {
            "offsets": { "linux": 2, "windows": 19 }, "vtable": { "class": "CSource2Server", "library": "server" } } })");
        const auto build = ServerBuild({ "a", "b", "c", "d", "e" });
        const auto result = CarryForward(gamedata, build.Source(), build.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].reason.find("no fingerprint window") != std::string::npos);
        CHECK(result.gamedata == gamedata);
    }
    SUBCASE("the window does not match the previous build at its index (Gamedata for another build)")
    {
        const auto gamedata = GamedataWithGameFrame(ServerBuild({ "a", "b", "c", "d", "e" }), 2);
        const auto previous = ServerBuild({ "a", "x", "b", "c", "d", "e" });
        const auto result = CarryForward(gamedata, previous.Source(), previous.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].reason.find("does not match the previous build at index 2") != std::string::npos);
        CHECK(result.gamedata == gamedata);
    }
    SUBCASE("the window occurs twice on the previous build: which copy moved where is a guess")
    {
        const auto previous = ServerBuild({ "b", "c", "d", "x", "b", "c", "d" });
        const auto next = ServerBuild({ "b", "c", "d", "x", "y", "b", "c", "d" });
        const auto gamedata = GamedataWithGameFrame(previous, 1);
        const auto result = CarryForward(gamedata, previous.Source(), next.Source());
        REQUIRE(result.held.size() == 1);
        CHECK(result.held[0].reason.find("occurs at 1, 5 on the previous build") != std::string::npos);
        CHECK(result.gamedata == gamedata);
    }
}

TEST_CASE("Carry-forward aligns the previous build's window when the recorded one is stale but still verifies there")
{
    // Recorded on a build whose neighbour 1 differed; the previous build resolves it by its own fingerprint.
    const auto gamedata = GamedataWithGameFrame(ServerBuild({ "a", "old", "c", "d", "e" }), 2);
    const auto previous = ServerBuild({ "a", "b", "c", "d", "e" });
    const auto next = ServerBuild({ "a", "new", "b", "c", "d", "e" });

    const auto result = CarryForward(gamedata, previous.Source(), next.Source());

    CHECK(result.held.empty());
    REQUIRE(result.carried.size() == 1);
    CHECK(result.carried[0].to == 3);
    CHECK(result.gamedata.at(kGameFrame.key)["vtable"]["linux"]["window"] == GamedataJson::parse(R"(["b", "c", "d"])"));
}

TEST_CASE("Carry-forward passes signatures and every other entry through untouched, in their order")
{
    const auto previous = ServerBuild({ "a", "b", "c", "d", "e" });
    const auto next = ServerBuild({ "a", "new", "b", "c", "d", "e" });
    auto gamedata = GamedataJson::parse(R"({
        "UTIL_ClientPrint": { "signatures": { "library": "server", "windows": "48 8B", "linux": "55 48 89 E5" } },
        "CBasePlayerPawn_CommitSuicide": { "offsets": { "windows": 386, "linux": 387 } },
        "ISource2Server::GameFrame": {},
        "GameEntitySystem": { "offsets": { "windows": 88, "linux": 80 } } })");
    gamedata[kGameFrame.key] = GamedataWithGameFrame(previous, 2).at(kGameFrame.key);
    gamedata[kGameFrame.key]["offsets"]["windows"] = 19;

    const auto result = CarryForward(gamedata, previous.Source(), next.Source());

    REQUIRE(result.carried.size() == 1);
    auto expected = gamedata;
    expected[kGameFrame.key]["offsets"]["linux"] = 3;
    expected[kGameFrame.key]["vtable"]["linux"]["size"] = 6;
    CHECK(result.gamedata == expected);
    std::vector<std::string> keys;
    for (const auto& [key, value] : result.gamedata.items())
        keys.push_back(key);
    CHECK(keys == std::vector<std::string>{ "UTIL_ClientPrint", "CBasePlayerPawn_CommitSuicide", "ISource2Server::GameFrame", "GameEntitySystem" });
    CHECK(SlotBindingLibraries(gamedata) == std::vector<std::string>{ "server" });
}

TEST_CASE("An archived build that is missing, or lacks a module, fails naming the build and module")
{
    const auto root = std::filesystem::temp_directory_path() / ("cssharp-archive-test-" + std::to_string(::getpid()));
    std::filesystem::create_directories(root / "1.0.0.1" / "linux");
    const auto message = [&](const char* build, const std::vector<std::string>& libraries) {
        try
        {
            ArchivedBuild(root, build).Require(libraries);
        }
        catch (const std::runtime_error& error)
        {
            return std::string(error.what());
        }
        return std::string("no error");
    };

    CHECK(message("9.9.9.9", { "server" }) == "build 9.9.9.9 is not in the build archive " + root.string());
    CHECK(message("1.0.0.1", { "engine" }) == "build 1.0.0.1 in " + root.string() + " has no libengine2.so or .img");
    CHECK(message("1.0.0.1", {}) == "no error");
    std::filesystem::remove_all(root);
}
