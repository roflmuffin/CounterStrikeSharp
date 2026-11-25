// Optional local fixture: resolves RTTI vtables, the core catalog and every Gamedata signature against a real server
// binary, loaded as the dynamic loader maps it (tooling/gamedata-generator/build_archive.h).
// Skipped unless CSSHARP_SERVER_BINARY points at a libserver.so (never committed). Linux only. The engine and tier0
// modules (IVEngineServer2 and ICvar Slot Bindings) are loaded from next to it (archive layout) or from the install's
// game/bin/linuxsteamrt64; Engine Features with a Binding in a module not found there are left out and listed.
//   CSSHARP_SERVER_BINARY=~/.cache/cs2-14201/game/csgo/bin/linuxsteamrt64/libserver.so ./native_tests -tc="*real*" -s
#include <doctest.h>

#ifdef __linux__
#include <algorithm>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <filesystem>
#include <iterator>
#include <optional>
#include <set>
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

// A sibling module of the server binary: in the same directory (build archive), or in an install's
// game/bin/linuxsteamrt64 (libserver.so lives in game/csgo/bin/linuxsteamrt64).
std::optional<std::filesystem::path> FindSiblingModule(const std::filesystem::path& server, const std::string& fileName)
{
    const auto directory = server.parent_path();
    for (const auto& candidate : { directory / fileName, directory / ".." / ".." / ".." / "bin" / "linuxsteamrt64" / fileName })
        if (std::filesystem::exists(candidate)) return candidate;
    return std::nullopt;
}

// The Gamedata library a Binding is searched in ("server" when Gamedata names none).
std::string LibraryOf(const BindingDeclaration& binding, const Gamedata& gamedata)
{
    if (binding.kind == BindingKind::RttiVtable || binding.kind == BindingKind::SchemaField)
        return binding.library.empty() ? "server" : binding.library;
    const auto entry = gamedata.find(binding.key);
    if (entry == gamedata.end()) return "server";
    if (binding.kind == BindingKind::Slot && entry->second.slot && entry->second.slot->library) return *entry->second.slot->library;
    if (binding.kind == BindingKind::Signature && entry->second.library) return *entry->second.library;
    return "server";
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

    const std::filesystem::path serverPath = std::getenv("CSSHARP_SERVER_BINARY");
    const auto server = LoadedModule::Load(serverPath);
    ModuleImages modules{ { "server", server.Image() } };
    REQUIRE_FALSE(modules.at("server").segments.empty());
    std::vector<LoadedModule> siblings;
    siblings.reserve(2); // the images point into these
    for (const auto& [library, fileName] : { std::pair{ "engine", "libengine2.so" }, std::pair{ "tier0", "libtier0.so" } })
    {
        const auto file = FindSiblingModule(serverPath, fileName);
        if (!file) continue;
        siblings.push_back(LoadedModule::Load(*file));
        modules.emplace(library, siblings.back().Image());
    }
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
    std::set<std::string> leftOut;
    for (auto feature : NativeEngineFeatures())
    {
        std::erase_if(feature.bindings, [&](const BindingDeclaration& binding) {
            return !notSchema(binding);
        });
        if (std::ranges::any_of(feature.bindings, [&](const BindingDeclaration& binding) {
                return !modules.contains(LibraryOf(binding, gamedata));
            }))
        {
            leftOut.insert(feature.name);
            continue;
        }
        CHECK_MESSAGE(EvaluateEngineFeature(core, feature).available, feature.name);
    }
    for (const auto& name : leftOut)
        std::cout << "left out (a Binding's module is not next to the server binary): " << name << "\n";
}
#endif
