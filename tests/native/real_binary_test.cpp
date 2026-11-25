// Optional local fixture: resolves RTTI vtables, the core catalog and every Gamedata signature against a real server
// binary, loaded as the dynamic loader maps it (tooling/gamedata-generator/build_archive.h).
// Skipped unless CSSHARP_SERVER_BINARY points at a libserver.so (never committed). Linux only.
//   CSSHARP_SERVER_BINARY=~/.cache/cs2-14201/game/csgo/bin/linuxsteamrt64/libserver.so ./native_tests -tc="*real*" -s
#include <doctest.h>

#ifdef __linux__
#include <algorithm>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <iterator>
#include <string>
#include <vector>

#include "build_archive.h"
#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;
using counterstrikesharp::generator::LoadedModule;

namespace {
// The vtable's own typeinfo name (vtable[-1] is the typeinfo, whose second word is its name), read back independently.
std::string TypeinfoNameOf(std::uintptr_t vtable)
{
    const auto* typeinfo = *reinterpret_cast<const std::uintptr_t* const*>(vtable - sizeof(std::uintptr_t));
    return reinterpret_cast<const char*>(typeinfo[1]);
}
} // namespace

TEST_CASE("real server binary: every class looked up by RTTI resolves to exactly its own vtable (optional fixture)" *
          doctest::skip(std::getenv("CSSHARP_SERVER_BINARY") == nullptr))
{
    const auto server = LoadedModule::Load(std::getenv("CSSHARP_SERVER_BINARY"));

    // Core lookups (game event manager, HUD dialog variable, nav physics interface), and classes plugins commonly look
    // up by symbol. CCSGameRules' name "12CCSGameRules" also occurs inside its pointer type's "P12CCSGameRules".
    for (const std::string className : { "CGameEventManager", "HUDPanelDialogVariableString_t", "CNavPhysicsInterface", "CCSGameRules",
                                         "CCSPlayerPawn", "CCSPlayerController", "CBaseEntity", "CCSGameRulesProxy" })
    {
        CAPTURE(className);
        const auto lookup = server.FindVtable(className);
        CHECK(lookup.matchCount == 1);
        REQUIRE(lookup.Found());
        CHECK(*reinterpret_cast<const std::int64_t*>(lookup.address - 2 * sizeof(std::uintptr_t)) == 0); // offset-to-top
        CHECK(TypeinfoNameOf(lookup.address) == std::to_string(className.size()) + className);
    }

    CHECK(server.FindVtable("CNoSuchClassAnywhere").matchCount == 0);
}

TEST_CASE("real server binary: core catalog and Gamedata signatures (optional fixture)" *
          doctest::skip(std::getenv("CSSHARP_SERVER_BINARY") == nullptr))
{
    std::ifstream gamedataStream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(gamedataStream.good());
    const auto gamedata = ParseGamedata(nlohmann::json::parse(gamedataStream), "linux");

    const auto server = LoadedModule::Load(std::getenv("CSSHARP_SERVER_BINARY"));
    const ModuleImages modules{ { "server", server.Image() } };
    REQUIRE_FALSE(modules.at("server").segments.empty());
    const BuiltinResolver resolver(Platform::Linux);

    std::vector<BindingDeclaration> everySignature;
    for (const auto& [key, entry] : gamedata)
        if (entry.hasSignatures) everySignature.push_back({ key, BindingKind::Signature });

    const auto all = resolver.Resolve("local", gamedata, modules, everySignature);
    std::cout << "all Gamedata signatures: " << FormatBindingSummary(all) << "\n";

    // Schema fields come from the running game's schema system, which a binary on disk cannot answer: leave them out
    // (the live server resolves them at boot).
    const auto notSchema = [](const BindingDeclaration& binding) {
        return binding.kind != BindingKind::SchemaField;
    };
    std::vector<BindingDeclaration> coreDeclarations;
    std::ranges::copy_if(CoreBindingDeclarations(gamedata), std::back_inserter(coreDeclarations), notSchema);

    const auto core = resolver.Resolve("local", gamedata, modules, coreDeclarations);
    std::cout << "core catalog (without schema fields): " << FormatBindingSummary(core) << "\n";
    for (auto feature : NativeEngineFeatures())
    {
        std::erase_if(feature.bindings, [&](const BindingDeclaration& binding) {
            return !notSchema(binding);
        });
        CHECK_MESSAGE(EvaluateEngineFeature(core, feature).available, feature.name);
    }
}
#endif
