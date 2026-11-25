// ICvar (CCvar in tier0): the console dispatch hook and every ICvar method CounterStrikeSharp calls, directly or
// through the SDK's convar.cpp, are Slot Bindings backing one Engine Feature per plugin-visible API.
#include <doctest.h>

#include <algorithm>
#include <fstream>
#include <map>
#include <set>
#include <string>
#include <string_view>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;

namespace {
// The spec's catalog rows for ICvar: Engine Feature -> its Slot Bindings, in declaration order.
const std::map<std::string, std::vector<std::string>> kIcvarFeatures{
    { "cssharp.commands/console-dispatch", { "ICvar::DispatchConCommand" } },
    { "cssharp.commands/add-command",
      { "ICvar::FindConCommand", "ICvar::RegisterConCommand", "ICvar::GetConCommandData", "ICvar::UnregisterConCommandCallbacks",
        "ICvar::GetCharacterSet" } },
    { "cssharp.player/execute-client-command-from-server",
      { "ICvar::GetCharacterSet", "ICvar::FindConCommand", "ICvar::DispatchConCommand" } },
    { "cssharp.convars/find", { "ICvar::FindConVar", "ICvar::GetConVarData" } },
    { "cssharp.convars/read", { "ICvar::GetConVarData" } },
    { "cssharp.convars/write",
      { "ICvar::GetConVarData", "ICvar::CallFilterCallback", "ICvar::CallChangeCallback", "ICvar::CallGlobalChangeCallbacks",
        "ICvar::QueueThreadSetValue" } },
    { "cssharp.convars/create", { "ICvar::FindConVar", "ICvar::GetConVarData", "ICvar::RegisterConVar", "ICvar::UnregisterConVarCallbacks" } },
    { "cssharp.convars/change-events",
      { "ICvar::InstallGlobalChangeCallback", "ICvar::RemoveGlobalChangeCallback", "ICvar::GetConVarData" } },
};

// Every ICvar slot CounterStrikeSharp calls or hooks, with its Linux index on the current build (SDK headers).
const std::map<std::string, std::int64_t> kIcvarSlots{
    { "ICvar::FindConVar", 11 },
    { "ICvar::CallChangeCallback", 14 },
    { "ICvar::CallFilterCallback", 16 },
    { "ICvar::FindConCommand", 17 },
    { "ICvar::DispatchConCommand", 20 },
    { "ICvar::InstallGlobalChangeCallback", 21 },
    { "ICvar::RemoveGlobalChangeCallback", 22 },
    { "ICvar::CallGlobalChangeCallbacks", 23 },
    { "ICvar::GetCharacterSet", 32 },
    { "ICvar::RegisterConVar", 38 },
    { "ICvar::UnregisterConVarCallbacks", 39 },
    { "ICvar::GetConVarData", 41 },
    { "ICvar::RegisterConCommand", 42 },
    { "ICvar::UnregisterConCommandCallbacks", 43 },
    { "ICvar::GetConCommandData", 44 },
    { "ICvar::QueueThreadSetValue", 45 },
};

nlohmann::json CommittedGamedataJson()
{
    std::ifstream stream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(stream.good());
    return nlohmann::json::parse(stream);
}

BindingResult SlotResult(const std::string& key, BindingStatus status, FailureReason reason = FailureReason::None)
{
    BindingResult result;
    result.key = key;
    result.kind = BindingKind::Slot;
    result.status = status;
    result.reason = reason;
    result.offset = kIcvarSlots.at(key);
    return result;
}

// Every ICvar Slot Binding resolved, except failedKey.
ResolutionReport ReportWithFailed(const std::string& failedKey)
{
    std::vector<BindingResult> bindings;
    for (const auto& [key, index] : kIcvarSlots)
        bindings.push_back(key == failedKey ? SlotResult(key, BindingStatus::Failed, FailureReason::FingerprintMismatch)
                                            : SlotResult(key, BindingStatus::Resolved));
    return ResolutionReport("b", std::move(bindings));
}
} // namespace

TEST_CASE("The catalog declares one Engine Feature per ICvar-backed plugin API, on ICvar Slot Bindings")
{
    for (const auto& [name, keys] : kIcvarFeatures)
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
    CHECK(features::ConsoleDispatch == "cssharp.commands/console-dispatch");
}

TEST_CASE("Every ICvar slot CounterStrikeSharp calls backs at least one Engine Feature")
{
    std::set<std::string> used;
    for (const auto& feature : NativeEngineFeatures())
        for (const auto& binding : feature.bindings)
            if (binding.key.rfind("ICvar::", 0) == 0) used.insert(binding.key);

    for (const auto& [key, index] : kIcvarSlots)
        CHECK_MESSAGE(used.count(key) == 1, key);
    CHECK(used.size() == kIcvarSlots.size());
}

TEST_CASE("A failed ICvar Slot Binding disables exactly the Engine Features using it")
{
    for (const auto& [failedKey, index] : kIcvarSlots)
    {
        CAPTURE(failedKey);
        const auto report = ReportWithFailed(failedKey);
        for (const auto& [name, keys] : kIcvarFeatures)
        {
            CAPTURE(name);
            const bool uses = std::find(keys.begin(), keys.end(), failedKey) != keys.end();
            const auto status = EvaluateEngineFeature(report, *FindNativeEngineFeature(name));
            CHECK(status.available == !uses);
            if (uses)
            {
                REQUIRE(status.failedBinding != nullptr);
                CHECK(status.failedBinding->key == failedKey);
            }
        }
    }
}

TEST_CASE("Healed and unverified ICvar Slot Bindings keep their Engine Features available")
{
    auto report = ResolutionReport("b", { SlotResult("ICvar::DispatchConCommand", BindingStatus::Healed) });
    CHECK(EvaluateEngineFeature(report, *FindNativeEngineFeature(features::ConsoleDispatch)).available);
    report = ResolutionReport("b", { SlotResult("ICvar::DispatchConCommand", BindingStatus::Unverified) });
    CHECK(EvaluateEngineFeature(report, *FindNativeEngineFeature(features::ConsoleDispatch)).available);
}

TEST_CASE("Committed Gamedata has every ICvar Slot Binding on CCvar in tier0, with a Linux window and no Windows index")
{
    const auto root = CommittedGamedataJson();
    const auto gamedata = ParseGamedata(root, "linux");
    const auto declarations = CoreBindingDeclarations(gamedata);

    for (const auto& [key, index] : kIcvarSlots)
    {
        CAPTURE(key);
        const auto entry = gamedata.find(key);
        REQUIRE(entry != gamedata.end());
        CHECK(entry->second.offset == index);
        REQUIRE(entry->second.slot);
        CHECK(entry->second.slot->className == "CCvar");
        CHECK(entry->second.slot->library == "tier0");
        CHECK(entry->second.slot->window.has_value());
        CHECK(entry->second.slot->vtableSize == 46u);

        // Windows keeps its compiled header indices (no index in Gamedata, nothing guessed).
        CHECK_FALSE(root[key]["offsets"].contains("windows"));
        const auto declared = std::find_if(declarations.begin(), declarations.end(), [&](const BindingDeclaration& d) {
            return d.key == key;
        });
        REQUIRE(declared != declarations.end());
        CHECK(declared->kind == BindingKind::Slot);
    }
}

TEST_CASE("On Windows the ICvar Slot Bindings are unverified with no index, so calls and the hook stay compiled")
{
    const auto gamedata = ParseGamedata(CommittedGamedataJson(), "windows");
    std::vector<BindingDeclaration> declarations;
    for (const auto& [key, index] : kIcvarSlots)
        declarations.push_back({ key, BindingKind::Slot });

    const auto report = BuiltinResolver(Platform::Windows).Resolve("b", gamedata, {}, declarations);

    for (const auto& binding : report.Bindings())
    {
        CAPTURE(binding.key);
        CHECK(binding.status == BindingStatus::Unverified);
        CHECK(binding.offset == -1);
    }
    for (const auto& [name, keys] : kIcvarFeatures)
        CHECK_MESSAGE(EvaluateEngineFeature(report, *FindNativeEngineFeature(name)).available, name);
}
