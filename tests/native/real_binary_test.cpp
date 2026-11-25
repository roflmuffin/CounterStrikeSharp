// Optional local fixture: resolves RTTI vtables, the core catalog and every Gamedata signature against a real server
// binary.
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
#include "core/rtti_vtable.h"

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

// The module as the dynamic loader would map it: PT_LOAD segments at their virtual addresses in one buffer, with the
// relocations that fill vtables and typeinfo (relative, and absolute against symbols the module defines) applied.
// Sections are then live memory at buffer + sh_addr, as CModule sees them in the server.
struct LoadedImage
{
    std::vector<std::uint64_t> memory; // 8-byte aligned, like the loader's mapping
    std::vector<counterstrikesharp::rtti::MemoryRegion> names; // .rodata
    std::vector<counterstrikesharp::rtti::MemoryRegion> data; // .data.rel.ro, .data.rel.ro.local

    [[nodiscard]] std::uint8_t* Base() { return reinterpret_cast<std::uint8_t*>(memory.data()); }

    counterstrikesharp::rtti::VtableLookup FindVtable(std::string_view className) const
    {
        return counterstrikesharp::rtti::FindPrimaryVtable(className, names, data);
    }
};

LoadedImage LoadImage(const std::vector<std::uint8_t>& file)
{
    LoadedImage image;
    if (file.size() < sizeof(Elf64_Ehdr) || std::memcmp(file.data(), ELFMAG, SELFMAG) != 0) return image;
    const auto* header = reinterpret_cast<const Elf64_Ehdr*>(file.data());
    const auto programHeader = [&](int i) {
        return reinterpret_cast<const Elf64_Phdr*>(file.data() + header->e_phoff + i * header->e_phentsize);
    };
    const auto sectionHeader = [&](int i) {
        return reinterpret_cast<const Elf64_Shdr*>(file.data() + header->e_shoff + i * header->e_shentsize);
    };

    std::uint64_t imageSize = 0;
    for (int i = 0; i < header->e_phnum; ++i)
        if (programHeader(i)->p_type == PT_LOAD) imageSize = std::max(imageSize, programHeader(i)->p_vaddr + programHeader(i)->p_memsz);
    image.memory.assign((imageSize + 7) / 8, 0);
    auto* base = image.Base();
    for (int i = 0; i < header->e_phnum; ++i)
    {
        const auto* phdr = programHeader(i);
        if (phdr->p_type != PT_LOAD || phdr->p_offset + phdr->p_filesz > file.size()) continue;
        std::memcpy(base + phdr->p_vaddr, file.data() + phdr->p_offset, phdr->p_filesz);
    }

    const char* sectionNames = reinterpret_cast<const char*>(file.data() + sectionHeader(header->e_shstrndx)->sh_offset);
    for (int i = 0; i < header->e_shnum; ++i)
    {
        const auto* shdr = sectionHeader(i);
        if (shdr->sh_type == SHT_RELA && (shdr->sh_flags & SHF_ALLOC))
        {
            const auto* symbols = reinterpret_cast<const Elf64_Sym*>(file.data() + sectionHeader(shdr->sh_link)->sh_offset);
            const auto* relocations = reinterpret_cast<const Elf64_Rela*>(file.data() + shdr->sh_offset);
            for (std::size_t r = 0; r < shdr->sh_size / sizeof(Elf64_Rela); ++r)
            {
                const auto& relocation = relocations[r];
                std::uint64_t value = 0;
                const auto& symbol = symbols[ELF64_R_SYM(relocation.r_info)];
                switch (ELF64_R_TYPE(relocation.r_info))
                {
                    case R_X86_64_RELATIVE:
                        value = reinterpret_cast<std::uint64_t>(base) + relocation.r_addend;
                        break;
                    case R_X86_64_64:
                        if (symbol.st_shndx == SHN_UNDEF) continue;
                        value = reinterpret_cast<std::uint64_t>(base) + symbol.st_value + relocation.r_addend;
                        break;
                    default:
                        continue;
                }
                if (relocation.r_offset + sizeof(value) <= imageSize) std::memcpy(base + relocation.r_offset, &value, sizeof(value));
            }
        }

        if (shdr->sh_type != SHT_PROGBITS) continue;
        const std::string_view name = sectionNames + shdr->sh_name;
        const counterstrikesharp::rtti::MemoryRegion region{ base + shdr->sh_addr, shdr->sh_size };
        if (name == ".rodata") image.names.push_back(region);
        if (name == ".data.rel.ro" || name == ".data.rel.ro.local") image.data.push_back(region);
    }
    return image;
}

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
    const auto file = ReadFile(std::getenv("CSSHARP_SERVER_BINARY"));
    REQUIRE_FALSE(file.empty());
    auto image = LoadImage(file);
    REQUIRE_FALSE(image.names.empty());
    REQUIRE_FALSE(image.data.empty());

    // Core lookups (game event manager, HUD dialog variable, nav physics interface), and classes plugins commonly look
    // up by symbol. CCSGameRules' name "12CCSGameRules" also occurs inside its pointer type's "P12CCSGameRules".
    for (const std::string className : { "CGameEventManager", "HUDPanelDialogVariableString_t", "CNavPhysicsInterface", "CCSGameRules",
                                         "CCSPlayerPawn", "CCSPlayerController", "CBaseEntity", "CCSGameRulesProxy" })
    {
        CAPTURE(className);
        const auto lookup = image.FindVtable(className);
        CHECK(lookup.matchCount == 1);
        REQUIRE(lookup.Found());
        CHECK(*reinterpret_cast<const std::int64_t*>(lookup.address - 2 * sizeof(std::uintptr_t)) == 0); // offset-to-top
        CHECK(TypeinfoNameOf(lookup.address) == std::to_string(className.size()) + className);
    }

    CHECK(image.FindVtable("CNoSuchClassAnywhere").matchCount == 0);
}

TEST_CASE("real server binary: core catalog and Gamedata signatures (optional fixture)" *
          doctest::skip(std::getenv("CSSHARP_SERVER_BINARY") == nullptr))
{
    const auto file = ReadFile(std::getenv("CSSHARP_SERVER_BINARY"));
    REQUIRE_FALSE(file.empty());

    std::ifstream gamedataStream(CSSHARP_GAMEDATA_PATH);
    REQUIRE(gamedataStream.good());
    const auto gamedata = ParseGamedata(nlohmann::json::parse(gamedataStream), "linux");

    const auto loaded = LoadImage(file);
    auto server = ExecutableSegments(file);
    server.findVirtualTable = [&loaded](std::string_view name) {
        return loaded.FindVtable(name);
    };
    const ModuleImages modules{ { "server", server } };
    REQUIRE_FALSE(modules.at("server").segments.empty());

    std::vector<BindingDeclaration> everySignature;
    for (const auto& [key, entry] : gamedata)
        if (entry.hasSignatures) everySignature.push_back({ key, BindingKind::Signature });

    const auto all = ResolveBindings("local", gamedata, modules, everySignature);
    std::cout << "all Gamedata signatures: " << FormatBindingSummary(all) << "\n";

    // Schema fields come from the running game's schema system, which a binary on disk cannot answer: leave them out
    // (the live server resolves them at boot).
    const auto notSchema = [](const BindingDeclaration& binding) {
        return binding.kind != BindingKind::SchemaField;
    };
    std::vector<BindingDeclaration> coreDeclarations;
    std::ranges::copy_if(CoreBindingDeclarations(gamedata), std::back_inserter(coreDeclarations), notSchema);

    const auto core = ResolveBindings("local", gamedata, modules, coreDeclarations);
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
