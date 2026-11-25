// The Slot Binding contract over everything the core declares: on Linux every hook and call on a migrated interface
// (ISource2Server, IServerGameClients, IVEngineServer2, ICvar) and every game-class virtual goes through a Slot
// Binding verified at boot, never through a compiled header slot position. Windows keeps today's behaviour: no Vtable
// Check; interface methods at their header-derived index, game-class virtuals at their Gamedata index.
#include <doctest.h>

#include <algorithm>
#include <array>
#include <fstream>
#include <map>
#include <string>
#include <string_view>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;
using nlohmann::json;

namespace {

constexpr std::array<std::string_view, 4> kMigratedInterfaces{ "ISource2Server::", "IServerGameClients::", "IVEngineServer2::",
                                                                "ICvar::" };

json Committed()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return json::parse(stream);
}

std::string_view InterfaceOf(std::string_view key)
{
    for (const auto prefix : kMigratedInterfaces)
        if (key.starts_with(prefix)) return prefix;
    return {};
}

bool IsGameClassVirtual(const BindingDeclaration& declaration)
{
    return declaration.kind == BindingKind::Slot && !declaration.headerIndex;
}

// The Slot Bindings the contract covers, as the core declares them for the committed Gamedata.
std::vector<BindingDeclaration> ContractSlotBindings(const Gamedata& gamedata)
{
    std::vector<BindingDeclaration> slots;
    for (const auto& declaration : CoreBindingDeclarations(gamedata))
        if (declaration.kind == BindingKind::Slot && (!InterfaceOf(declaration.key).empty() || IsGameClassVirtual(declaration)))
            slots.push_back(declaration);
    return slots;
}

} // namespace

TEST_CASE("Contract: every Slot Binding on a migrated interface or a game-class virtual carries its Linux fingerprint window "
          "in the committed Gamedata, so it is verified at boot")
{
    const auto committed = Committed();
    const auto slots = ContractSlotBindings(ParseGamedata(committed, "linux"));

    std::map<std::string_view, std::size_t> perInterface;
    std::size_t gameClass = 0;
    for (const auto& declaration : slots)
    {
        CAPTURE(declaration.key);
        REQUIRE(committed.contains(declaration.key));
        const auto& entry = committed.at(declaration.key);
        CHECK(entry["offsets"]["linux"].is_number_integer());
        REQUIRE(entry.contains("vtable"));
        CHECK(entry["vtable"]["class"].is_string());
        CHECK(entry["vtable"]["library"].is_string());
        CHECK(entry["vtable"]["linux"]["window"].size() == 3);
        CHECK(entry["vtable"]["linux"]["size"].is_number_unsigned());
        CHECK_FALSE(entry["vtable"].contains("windows")); // Windows has no Vtable Check
        if (IsGameClassVirtual(declaration)) ++gameClass;
        else
            ++perInterface[InterfaceOf(declaration.key)];
    }
    for (const auto prefix : kMigratedInterfaces)
    {
        CAPTURE(prefix);
        CHECK(perInterface[prefix] > 0);
    }
    CHECK(gameClass > 0);
}

TEST_CASE("Contract: on Linux a migrated Slot Binding is never used at a compiled header index")
{
    const auto committed = Committed();
    const auto slots = ContractSlotBindings(ParseGamedata(committed, "linux"));
    REQUIRE_FALSE(slots.empty());

    SUBCASE("absent from Gamedata it fails, so its Engine Features are disabled")
    {
        const auto report = BuiltinResolver(Platform::Linux).Resolve("b", {}, {}, slots);
        for (const auto& binding : report.Bindings())
        {
            CAPTURE(binding.key);
            CHECK(binding.status == BindingStatus::Failed);
            CHECK(binding.reason == FailureReason::MissingFromGamedata);
        }
    }

    SUBCASE("in Gamedata without its fingerprint window it is unverified at the Gamedata index, not a header one")
    {
        // e.g. a gamedata.json from before Slot Bindings: index only.
        json windowless = json::object();
        for (const auto& declaration : slots)
            windowless[declaration.key] = { { "offsets", { { "linux", 3 } } } };
        const auto report = BuiltinResolver(Platform::Linux).Resolve("b", ParseGamedata(windowless, "linux"), {}, slots);
        for (const auto& binding : report.Bindings())
        {
            CAPTURE(binding.key);
            CHECK(binding.status == BindingStatus::Unverified);
            CHECK(binding.offset == 3);
        }
    }
}

TEST_CASE("Contract: Windows behaves as today with the committed Gamedata: no Slot Binding is checked or fails; interface "
          "methods use their header index, game-class virtuals their Gamedata index")
{
    const auto committed = Committed();
    const auto gamedata = ParseGamedata(committed, "windows");
    std::vector<BindingDeclaration> slots;
    for (const auto& declaration : CoreBindingDeclarations(gamedata))
        if (declaration.kind == BindingKind::Slot) slots.push_back(declaration);
    const auto report = BuiltinResolver(Platform::Windows).Resolve("b", gamedata, {}, slots);

    for (const auto& binding : report.Bindings())
    {
        CAPTURE(binding.key);
        CHECK(binding.status == BindingStatus::Unverified);
        const auto& offsets = committed.at(binding.key)["offsets"];
        if (offsets.contains("windows")) CHECK(binding.offset == offsets["windows"].get<std::int64_t>());
        else
            CHECK(binding.offset == -1); // no Windows index in Gamedata: the compiled header index
    }
}
