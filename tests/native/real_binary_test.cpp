// Optional local fixture: resolves the core catalog and every Gamedata signature against a real server binary.
// Skipped unless CSSHARP_SERVER_BINARY points at a libserver.so (never committed). Linux only.
//   CSSHARP_SERVER_BINARY=~/.cache/cs2/game/csgo/bin/linuxsteamrt64/libserver.so ./native_tests -tc="*real*" -s
#include <doctest.h>

#ifdef __linux__
#include <elf.h>

#include <algorithm>
#include <cstdlib>
#include <cstring>
#include <fstream>
#include <iostream>
#include <iterator>
#include <string>
#include <vector>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

using namespace counterstrikesharp::bindings;

namespace {
std::vector<std::uint8_t> ReadFile(const char* path)
{
    std::ifstream stream(path, std::ios::binary);
    return { std::istreambuf_iterator<char>(stream), std::istreambuf_iterator<char>() };
}

// Executable PT_LOAD segments, addressed by virtual address (module base 0), as CModule scans them.
ModuleImage ExecutableSegments(const std::vector<std::uint8_t>& file)
{
    ModuleImage image;
    if (file.size() < sizeof(Elf64_Ehdr) || std::memcmp(file.data(), ELFMAG, SELFMAG) != 0) return image;
    const auto* header = reinterpret_cast<const Elf64_Ehdr*>(file.data());
    for (int i = 0; i < header->e_phnum; ++i)
    {
        const auto* phdr = reinterpret_cast<const Elf64_Phdr*>(file.data() + header->e_phoff + i * header->e_phentsize);
        if (phdr->p_type != PT_LOAD || !(phdr->p_flags & PF_X) || !(phdr->p_flags & PF_R)) continue;
        if (phdr->p_offset + phdr->p_filesz > file.size()) continue;
        image.segments.push_back({ phdr->p_vaddr, file.data() + phdr->p_offset, phdr->p_filesz });
    }
    return image;
}

// Stand-in for CModule::FindVirtualTable: the Itanium typeinfo name ("17CGameEventManager") is in the file. The vtable
// itself is only reachable through dynamic relocations, so the live server resolves it; this checks the class exists.
std::uintptr_t FindTypeinfoName(const std::vector<std::uint8_t>& file, std::string_view className)
{
    const std::string decorated = std::to_string(className.size()) + std::string(className);
    const auto* end = file.data() + file.size();
    const auto* found = std::search(file.data(), end, decorated.c_str(), decorated.c_str() + decorated.size() + 1);
    return found == end ? 0 : static_cast<std::uintptr_t>(found - file.data());
}
} // namespace

TEST_CASE("real server binary: core catalog and Gamedata signatures (optional fixture)" *
          doctest::skip(std::getenv("CSSHARP_SERVER_BINARY") == nullptr))
{
    const auto file = ReadFile(std::getenv("CSSHARP_SERVER_BINARY"));
    REQUIRE_FALSE(file.empty());

    std::ifstream gamedataStream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(gamedataStream.good());
    const auto gamedata = ParseGamedata(nlohmann::json::parse(gamedataStream), "linux");

    auto server = ExecutableSegments(file);
    server.findVirtualTable = [&file](std::string_view name) {
        return FindTypeinfoName(file, name);
    };
    const ModuleImages modules{ { "server", server } };
    REQUIRE_FALSE(modules.at("server").segments.empty());

    std::vector<BindingDeclaration> everySignature;
    for (const auto& [key, entry] : gamedata)
        if (entry.hasSignatures) everySignature.push_back({ key, BindingKind::Signature });

    const auto all = ResolveBindings("local", gamedata, modules, everySignature);
    std::cout << "all Gamedata signatures: " << FormatBindingSummary(all) << "\n";

    const auto core = ResolveBindings("local", gamedata, modules, CoreBindingDeclarations(gamedata));
    std::cout << "core catalog: " << FormatBindingSummary(core) << "\n";
    for (const auto& feature : NativeEngineFeatures())
        CHECK_MESSAGE(EvaluateEngineFeature(core, feature).available, feature.name);
}
#endif
