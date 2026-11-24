#include <doctest.h>

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
