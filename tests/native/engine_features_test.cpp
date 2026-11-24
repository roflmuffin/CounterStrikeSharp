#include <doctest.h>

#include <algorithm>
#include <set>
#include <string>
#include <string_view>
#include <utility>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;

namespace {
BindingResult Resolved(std::string key)
{
    BindingResult result;
    result.key = std::move(key);
    result.status = BindingStatus::Resolved;
    result.matchCount = 1;
    result.address = 0x1000;
    return result;
}

BindingResult Failed(std::string key, FailureReason reason, std::size_t matchCount = 0)
{
    BindingResult result;
    result.key = std::move(key);
    result.status = BindingStatus::Failed;
    result.reason = reason;
    result.matchCount = matchCount;
    return result;
}

const EngineFeatureDeclaration kTwoBindings{ "test/two", { { "A", BindingKind::Signature }, { "B", BindingKind::Offset } } };
} // namespace

TEST_CASE("An Engine Feature is available exactly when all of its Bindings resolved")
{
    const ResolutionReport allResolved("b", { Resolved("A"), Resolved("B") });
    CHECK(EvaluateEngineFeature(allResolved, kTwoBindings).available);
    CHECK(EvaluateEngineFeature(allResolved, kTwoBindings).failedBinding == nullptr);

    const ResolutionReport oneFailed("b", { Resolved("A"), Failed("B", FailureReason::MissingForThisOs) });
    auto status = EvaluateEngineFeature(oneFailed, kTwoBindings);
    CHECK_FALSE(status.available);
    REQUIRE(status.failedBinding != nullptr);
    CHECK(status.failedBinding->key == "B");

    const ResolutionReport bindingNotInReport("b", { Resolved("A") });
    CHECK_FALSE(EvaluateEngineFeature(bindingNotInReport, kTwoBindings).available);
}

TEST_CASE("DescribeFailure names every reason, ambiguous with its match count")
{
    CHECK(DescribeFailure(Failed("k", FailureReason::MissingFromGamedata)) == "missing from gamedata");
    CHECK(DescribeFailure(Failed("k", FailureReason::MissingForThisOs)) == "missing for this OS");
    CHECK(DescribeFailure(Failed("k", FailureReason::InvalidPattern)) == "invalid pattern");
    CHECK(DescribeFailure(Failed("k", FailureReason::NotFound)) == "not found");
    CHECK(DescribeFailure(Failed("k", FailureReason::Ambiguous, 3)) == "ambiguous (3 matches)");
    CHECK(DescribeFailure(Failed("k", FailureReason::RttiVtableNotFound)) == "RTTI vtable not found");
    CHECK(DescribeFailure(Failed("k", FailureReason::SchemaClassMissing)) == "schema class missing");
    CHECK(DescribeFailure(Failed("k", FailureReason::SchemaFieldMissing)) == "schema field missing");
    CHECK(DescribeFailure(Resolved("k")) == "resolved");
}

TEST_CASE("The Binding summary is one line listing failed Bindings with reasons")
{
    const ResolutionReport someFailed(
        "1.42.0.1/2000950", { Resolved("A"), Failed("Host_Say", FailureReason::NotFound), Failed("X", FailureReason::Ambiguous, 2) });
    CHECK(FormatBindingSummary(someFailed) ==
          "Bindings for build 1.42.0.1/2000950: 2 of 3 failed: Host_Say (not found), X (ambiguous (2 matches))");

    const ResolutionReport noneFailed("1.42.0.1/2000950", { Resolved("A"), Resolved("B") });
    CHECK(FormatBindingSummary(noneFailed) == "Bindings for build 1.42.0.1/2000950: all 2 resolved");
}

TEST_CASE("ParseBuildId reads PatchVersion and ServerVersion from steam.inf")
{
    CHECK(ParseBuildId("ClientVersion=2000950\r\nServerVersion=2000950\r\nPatchVersion=1.42.0.1\r\nProductName=cs2\r\n") ==
          "1.42.0.1/2000950");
    CHECK(ParseBuildId("PatchVersion=1.42.0.1\n") == "1.42.0.1");
    CHECK(ParseBuildId("") == "unknown");
}

TEST_CASE("The native Engine Feature catalog declares say-hooks on Host_Say")
{
    const auto* sayHooks = FindNativeEngineFeature(features::ChatSayHooks);
    REQUIRE(sayHooks != nullptr);
    CHECK(sayHooks->name == "cssharp.chat/say-hooks");
    REQUIRE(sayHooks->bindings.size() == 1);
    CHECK(sayHooks->bindings[0].key == "Host_Say");
    CHECK(sayHooks->bindings[0].kind == BindingKind::Signature);
    CHECK(FindNativeEngineFeature("cssharp.nope/nope") == nullptr);
}

TEST_CASE("The native catalog declares the event and network Engine Features, game-events on its RTTI vtable")
{
    auto bindingsOf = [](std::string_view name) {
        const auto* feature = FindNativeEngineFeature(name);
        REQUIRE_MESSAGE(feature != nullptr, name);
        return feature->bindings;
    };

    const auto gameEvents = bindingsOf(features::GameEvents);
    CHECK(features::GameEvents == "cssharp.events/game-events");
    REQUIRE(gameEvents.size() == 2);
    CHECK(gameEvents[0].key == "CGameEventManager_Init");
    CHECK(gameEvents[0].kind == BindingKind::Signature);
    CHECK(gameEvents[1].key == "CGameEventManager");
    CHECK(gameEvents[1].kind == BindingKind::RttiVtable);
    CHECK(gameEvents[1].library == "server");

    const auto fireEventToClient = bindingsOf(features::FireEventToClient);
    CHECK(features::FireEventToClient == "cssharp.events/fire-event-to-client");
    REQUIRE(fireEventToClient.size() == 1);
    CHECK(fireEventToClient[0].key == "LegacyGameEventListener");

    const auto stateChanged = bindingsOf(features::StateChanged);
    CHECK(features::StateChanged == "cssharp.network/state-changed");
    REQUIRE(stateChanged.size() == 2);
    CHECK(stateChanged[0].key == "NetworkStateChanged");
    CHECK(stateChanged[0].kind == BindingKind::Signature);
    CHECK(stateChanged[1].key == "SetStateChanged");
    CHECK(stateChanged[1].kind == BindingKind::Offset);

    const auto checkTransmit = bindingsOf(features::CheckTransmit);
    CHECK(features::CheckTransmit == "cssharp.network/check-transmit");
    REQUIRE(checkTransmit.size() == 2);
    CHECK(checkTransmit[0].key == "ISource2GameEntities::CheckTransmit");
    CHECK(checkTransmit[0].kind == BindingKind::Offset);
    CHECK(checkTransmit[1].key == "CheckTransmitPlayerSlot");
    CHECK(checkTransmit[1].kind == BindingKind::Offset);
}

TEST_CASE("An RTTI vtable Binding of the catalog is resolved by class name in its library, not taken from Gamedata")
{
    // Gamedata never has the class name, and a Gamedata key with that name must not turn it into a Signature.
    const nlohmann::json root = {
        { "CGameEventManager", { { "signatures", { { "library", "server" }, { "linux", "55 48" }, { "windows", "55" } } } } },
    };
    const auto gamedata = ParseGamedata(root, "linux");
    const auto declarations = CoreBindingDeclarations(gamedata);
    const auto vtable = std::find_if(declarations.begin(), declarations.end(), [](const BindingDeclaration& d) {
        return d.key == "CGameEventManager";
    });
    REQUIRE(vtable != declarations.end());
    CHECK(vtable->kind == BindingKind::RttiVtable);

    ModuleImage server;
    server.findVirtualTable = [](std::string_view name) -> std::uintptr_t { return name == "CGameEventManager" ? 0x5000 : 0; };
    ModuleImages found;
    found.emplace("server", server);
    const auto* gameEvents = FindNativeEngineFeature(features::GameEvents);
    REQUIRE(gameEvents != nullptr);

    const std::vector<BindingDeclaration> vtableOnly{ *vtable };
    const auto resolved = ResolveBindings("b", gamedata, found, vtableOnly);
    REQUIRE(resolved.Find("CGameEventManager") != nullptr);
    CHECK(resolved.Find("CGameEventManager")->address == 0x5000);

    const auto missing = ResolveBindings("b", gamedata, {}, declarations);
    const auto status = EvaluateEngineFeature(missing, *gameEvents);
    CHECK_FALSE(status.available);
    REQUIRE(status.failedBinding != nullptr);
    // CGameEventManager_Init is missing from this Gamedata; once it resolves, the vtable is what fails.
    CHECK(status.failedBinding->key == "CGameEventManager_Init");
    CHECK(DescribeFailure(*missing.Find("CGameEventManager")) == "RTTI vtable not found");
}

TEST_CASE("The native Engine Feature catalog declares game-system-events on the game-system signature and RunThink's schema fields")
{
    const auto* gameSystem = FindNativeEngineFeature(features::ServerGameSystemEvents);
    REQUIRE(gameSystem != nullptr);
    CHECK(gameSystem->name == "cssharp.server/game-system-events");

    std::vector<std::pair<std::string, BindingKind>> bindings;
    for (const auto& binding : gameSystem->bindings)
        bindings.emplace_back(binding.key, binding.kind);
    CHECK(bindings == std::vector<std::pair<std::string, BindingKind>>{
                          { "IGameSystem_InitAllSystems_pFirst", BindingKind::Signature },
                          { "CCSPlayerController::m_hPlayerPawn", BindingKind::SchemaField },
                          { "CBasePlayerController::m_iConnected", BindingKind::SchemaField },
                          { "CBasePlayerPawn::m_pMovementServices", BindingKind::SchemaField },
                          { "CPlayer_MovementServices::m_nButtons", BindingKind::SchemaField },
                          { "CInButtonState::m_pButtonStates", BindingKind::SchemaField },
                      });
}

TEST_CASE("Entity IO, sound and spawn are separate Engine Features, each on its own Binding")
{
    const std::vector<std::pair<std::string_view, std::string>> expected{
        { "cssharp.entity/output-hooks", "CEntityIOOutput_FireOutputInternal" },
        { "cssharp.entity/accept-input", "CEntityInstance_AcceptInput" },
        { "cssharp.entity/add-io-event", "CEntitySystem_AddEntityIOEvent" },
        { "cssharp.entity/emit-sound", "CBaseEntity_EmitSoundFilter" },
        { "cssharp.entity/dispatch-spawn", "CBaseEntity_DispatchSpawn" },
    };
    for (const auto& [name, key] : expected)
    {
        const auto* feature = FindNativeEngineFeature(name);
        REQUIRE_MESSAGE(feature != nullptr, name);
        REQUIRE(feature->bindings.size() == 1);
        CHECK(feature->bindings[0].key == key);
        CHECK(feature->bindings[0].kind == BindingKind::Signature);
    }
    CHECK(FindNativeEngineFeature(features::EntityOutputHooks) != nullptr);
}

TEST_CASE("Breaking either take-damage Binding disables take-damage-hooks")
{
    const auto* takeDamage = FindNativeEngineFeature(features::EntityTakeDamageHooks);
    REQUIRE(takeDamage != nullptr);
    CHECK(takeDamage->name == "cssharp.entity/take-damage-hooks");

    const ResolutionReport bothResolved("b", { Resolved("CBaseEntity_TakeDamageOld"), Resolved("CBaseEntity_IsPlayerPawn") });
    CHECK(EvaluateEngineFeature(bothResolved, *takeDamage).available);

    const ResolutionReport signatureBroken(
        "b", { Failed("CBaseEntity_TakeDamageOld", FailureReason::NotFound), Resolved("CBaseEntity_IsPlayerPawn") });
    auto status = EvaluateEngineFeature(signatureBroken, *takeDamage);
    CHECK_FALSE(status.available);
    REQUIRE(status.failedBinding != nullptr);
    CHECK(status.failedBinding->key == "CBaseEntity_TakeDamageOld");

    const ResolutionReport offsetBroken(
        "b", { Resolved("CBaseEntity_TakeDamageOld"), Failed("CBaseEntity_IsPlayerPawn", FailureReason::MissingForThisOs) });
    status = EvaluateEngineFeature(offsetBroken, *takeDamage);
    CHECK_FALSE(status.available);
    REQUIRE(status.failedBinding != nullptr);
    CHECK(status.failedBinding->key == "CBaseEntity_IsPlayerPawn");
}

TEST_CASE("The catalog is consistent: unique feature names, namespaced, and core Bindings declared once each")
{
    std::set<std::string> names;
    for (const auto& feature : NativeEngineFeatures())
    {
        CHECK_MESSAGE(names.insert(feature.name).second, "duplicate Engine Feature " << feature.name);
        CHECK_MESSAGE(feature.name.rfind("cssharp.", 0) == 0, feature.name);
        CHECK_MESSAGE(feature.name.find('/') != std::string::npos, feature.name);
        CHECK_FALSE(feature.bindings.empty());
    }

    std::set<std::string> keys;
    for (const auto& binding : CoreBindingDeclarations({}))
    {
        CHECK_MESSAGE(keys.insert(binding.key).second, "duplicate Binding " << binding.key);
    }

    for (const auto& feature : NativeEngineFeatures())
        for (const auto& binding : feature.bindings)
            CHECK_MESSAGE(keys.count(binding.key) == 1, "Binding " << binding.key << " of " << feature.name << " not declared");
}

TEST_CASE("Core Bindings are the catalog's Bindings plus every core Gamedata key, by its section kind")
{
    const nlohmann::json root = {
        { "ClientPrint", { { "signatures", { { "library", "server" }, { "linux", "55 48" }, { "windows", "55" } } } } },
        { "CBaseEntity_Teleport", { { "offsets", { { "linux", 157 }, { "windows", 156 } } } } },
        { "PatchOnly", { { "patches", nlohmann::json::object() } } },
    };
    const auto declarations = CoreBindingDeclarations(ParseGamedata(root, "linux"));

    auto find = [&](std::string_view key) -> const BindingDeclaration* {
        for (const auto& declaration : declarations)
            if (declaration.key == key) return &declaration;
        return nullptr;
    };

    // Catalog Bindings are declared even when Gamedata lacks them, so the report says why they failed.
    REQUIRE(find("Host_Say") != nullptr);
    CHECK(find("Host_Say")->kind == BindingKind::Signature);

    // Keys only managed code consumes are resolved too, so the report covers them.
    REQUIRE(find("ClientPrint") != nullptr);
    CHECK(find("ClientPrint")->kind == BindingKind::Signature);
    REQUIRE(find("CBaseEntity_Teleport") != nullptr);
    CHECK(find("CBaseEntity_Teleport")->kind == BindingKind::Offset);

    CHECK(find("PatchOnly") == nullptr);

    std::set<std::string> keys;
    for (const auto& declaration : declarations)
        CHECK_MESSAGE(keys.insert(declaration.key).second, "duplicate Binding " << declaration.key);
}

TEST_CASE("The Resolution Report crosses to managed as data: build id, every Binding, and the native catalog")
{
    BindingResult hostSay = Failed("Host_Say", FailureReason::Ambiguous, 2);
    hostSay.library = "server";
    BindingResult teleport;
    teleport.key = "CBaseEntity_Teleport";
    teleport.kind = BindingKind::Offset;
    teleport.status = BindingStatus::Resolved;
    teleport.offset = 157;
    BindingResult clientPrint = Resolved("ClientPrint");
    clientPrint.address = 0x7f0000001234;
    clientPrint.library = "server";

    const ResolutionReport report("1.42.0.1/2000950", { hostSay, teleport, clientPrint });
    const std::vector<EngineFeatureDeclaration> catalog{ { "cssharp.chat/say-hooks", { { "Host_Say", BindingKind::Signature } } } };

    const auto json = nlohmann::json::parse(ResolutionReportToJson(report, catalog));

    CHECK(json["buildId"] == "1.42.0.1/2000950");
    REQUIRE(json["bindings"].size() == 3);
    CHECK(json["bindings"][0] == nlohmann::json{ { "key", "Host_Say" },
                                                 { "kind", "Signature" },
                                                 { "resolved", false },
                                                 { "reason", "Ambiguous" },
                                                 { "matchCount", 2 },
                                                 { "address", 0 },
                                                 { "offset", 0 },
                                                 { "library", "server" } });
    CHECK(json["bindings"][1]["kind"] == "Offset");
    CHECK(json["bindings"][1]["resolved"] == true);
    CHECK(json["bindings"][1]["reason"] == "None");
    CHECK(json["bindings"][1]["offset"] == 157);
    CHECK(json["bindings"][2]["address"].get<std::uint64_t>() == 0x7f0000001234);

    REQUIRE(json["features"].size() == 1);
    CHECK(json["features"][0] == nlohmann::json{ { "name", "cssharp.chat/say-hooks" }, { "bindings", { "Host_Say" } } });
}

TEST_CASE("Every failure reason and Binding kind has its own name on the wire")
{
    std::set<std::string> names;
    for (auto reason : { FailureReason::None, FailureReason::MissingFromGamedata, FailureReason::MissingForThisOs,
                         FailureReason::InvalidPattern, FailureReason::NotFound, FailureReason::Ambiguous,
                         FailureReason::RttiVtableNotFound, FailureReason::SchemaClassMissing, FailureReason::SchemaFieldMissing })
    {
        const ResolutionReport report("b", { Failed("k", reason) });
        const auto json = nlohmann::json::parse(ResolutionReportToJson(report, {}));
        CHECK(names.insert(json["bindings"][0]["reason"].get<std::string>()).second);
    }
    CHECK(names.count("MissingForThisOs") == 1);

    BindingResult vtable = Resolved("CGameEventManager");
    vtable.kind = BindingKind::RttiVtable;
    BindingResult schemaField = Resolved("CBaseEntity::m_iTeamNum");
    schemaField.kind = BindingKind::SchemaField;
    const auto json = nlohmann::json::parse(ResolutionReportToJson(ResolutionReport("b", { vtable, schemaField }), {}));
    CHECK(json["bindings"][0]["kind"] == "RttiVtable");
    CHECK(json["bindings"][1]["kind"] == "SchemaField");
}

TEST_CASE("Calling into a disabled Engine Feature is described in the wording of the managed typed exception")
{
    const ResolutionReport oneFailed("b", { Resolved("A"), Failed("B", FailureReason::Ambiguous, 2) });
    CHECK(DescribeUnavailable(oneFailed, kTwoBindings) == "Engine Feature test/two is disabled (B: ambiguous (2 matches))");

    const ResolutionReport bindingNotInReport("b", { Resolved("A") });
    CHECK(DescribeUnavailable(bindingNotInReport, kTwoBindings) == "Engine Feature test/two is disabled (B: missing from gamedata)");

    const ResolutionReport allResolved("b", { Resolved("A"), Resolved("B") });
    CHECK(DescribeUnavailable(allResolved, kTwoBindings) == "Engine Feature test/two is available");
}
