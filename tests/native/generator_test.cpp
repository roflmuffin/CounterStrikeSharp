// The Gamedata generator's bootstrap on synthetic vtables: what it emits is what the resolver reads.
#include <doctest.h>

#include <stdexcept>
#include <string>
#include <vector>

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
