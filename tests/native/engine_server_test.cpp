// IVEngineServer2 (engine2's CEngineServer): the SetClientListening hook and every direct call CounterStrikeSharp makes
// on it go through Slot Bindings, one Engine Feature per plugin-visible API.
#include <doctest.h>

#include <fstream>
#include <string>
#include <string_view>
#include <utility>
#include <vector>

#include <nlohmann/json.hpp>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;

namespace {
// Engine Feature -> its IVEngineServer2 Slot Bindings (the spec's catalog).
const std::vector<std::pair<std::string_view, std::vector<std::string>>> kEngineServerFeatures{
    { "cssharp.player/set-listening", { "IVEngineServer2::SetClientListening" } },
    { "cssharp.server/execute-command", { "IVEngineServer2::ServerCommand" } },
    { "cssharp.server/is-map-valid", { "IVEngineServer2::IsMapValid" } },
    { "cssharp.server/precache-model", { "IVEngineServer2::PrecacheGeneric" } },
    { "cssharp.player/user-id", { "IVEngineServer2::GetPlayerUserId" } },
    { "cssharp.player/print-to-console", { "IVEngineServer2::ClientPrintf" } },
    { "cssharp.player/get-convar-value", { "IVEngineServer2::GetClientConVarValue" } },
    { "cssharp.player/set-fake-client-convar", { "IVEngineServer2::SetFakeClientConVarValue" } },
    { "cssharp.player/execute-client-command", { "IVEngineServer2::ClientCommand" } },
    { "cssharp.player/disconnect", { "IVEngineServer2::DisconnectClient" } },
    { "cssharp.player/authorization", { "IVEngineServer2::IsClientFullyAuthenticated", "IVEngineServer2::GetClientSteamID" } },
};

// Linux slot indices from the SDK headers (identical in the 1.41.9.7 and the 1.42.0.1 headers).
const std::vector<std::pair<std::string, int>> kHeaderIndices{
    { "IVEngineServer2::IsMapValid", 33 },
    { "IVEngineServer2::PrecacheGeneric", 37 },
    { "IVEngineServer2::GetPlayerUserId", 39 },
    { "IVEngineServer2::ServerCommand", 45 },
    { "IVEngineServer2::ClientCommand", 46 },
    { "IVEngineServer2::ClientPrintf", 47 },
    { "IVEngineServer2::GetClientConVarValue", 53 },
    { "IVEngineServer2::GetClientSteamID", 70 },
    { "IVEngineServer2::IsClientFullyAuthenticated", 75 },
    { "IVEngineServer2::SetFakeClientConVarValue", 77 },
    { "IVEngineServer2::DisconnectClient", 88 },
    { "IVEngineServer2::SetClientListening", 92 },
};
} // namespace

TEST_CASE("The catalog declares one Engine Feature per IVEngineServer2-backed API, each on its Slot Bindings")
{
    for (const auto& [name, keys] : kEngineServerFeatures)
    {
        CAPTURE(name);
        const auto* feature = FindNativeEngineFeature(name);
        REQUIRE(feature != nullptr);
        std::vector<std::string> declared;
        for (const auto& binding : feature->bindings)
        {
            CHECK(binding.kind == BindingKind::Slot);
            declared.push_back(binding.key);
        }
        CHECK(declared == keys);
    }
}

TEST_CASE("Committed Gamedata states every IVEngineServer2 Slot Binding on CEngineServer in engine2, at its header index")
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    const auto root = nlohmann::json::parse(stream);
    const auto gamedata = ParseGamedata(root, "linux");

    for (const auto& [key, index] : kHeaderIndices)
    {
        CAPTURE(key);
        REQUIRE(gamedata.count(key) == 1);
        const auto& entry = gamedata.at(key);
        REQUIRE(entry.slot);
        CHECK(entry.slot->className == "CEngineServer");
        CHECK(entry.slot->library == "engine");
        CHECK(entry.slot->window.has_value());
        CHECK(root.at(key).at("offsets").at("linux") == index);
        // Windows keeps today's compiled header index: no Gamedata index, so the Binding is unverified there.
        CHECK_FALSE(root.at(key).at("offsets").contains("windows"));
    }
}
