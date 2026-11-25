#include "build_archive.h"

#include <elf.h>

#include <algorithm>
#include <cstdlib>
#include <cstring>
#include <fstream>
#include <iterator>
#include <stdexcept>

namespace counterstrikesharp::generator {
namespace {

// Where undefined __cxa_pure_virtual / __cxa_deleted_virtual relocations point in a loaded image: never code.
constexpr std::uintptr_t kPureVirtualMarker = 0x10;
constexpr std::uintptr_t kDeletedVirtualMarker = 0x18;
constexpr std::size_t kWord = sizeof(std::uint64_t);

std::vector<std::uint8_t> ReadFile(const std::filesystem::path& file)
{
    std::ifstream stream(file, std::ios::binary);
    if (!stream) throw std::runtime_error("cannot read " + file.string());
    return { std::istreambuf_iterator<char>(stream), std::istreambuf_iterator<char>() };
}

std::string ModuleFileStem(std::string_view library)
{
    if (library == "engine" || library == "engine2") return "libengine2";
    return "lib" + std::string(library);
}

} // namespace

std::filesystem::path DefaultArchiveRoot()
{
    if (const char* root = std::getenv("CSSHARP_BUILD_ARCHIVE"); root != nullptr && *root != '\0') return root;
    const char* home = std::getenv("HOME");
    return std::filesystem::path(home != nullptr ? home : "") / "dev" / "cs2-builds";
}

std::optional<std::filesystem::path> FindArchivedModule(const std::filesystem::path& root, std::string_view build, std::string_view library)
{
    const auto directory = root / std::string(build) / "linux";
    for (const char* extension : { ".so", ".img" })
    {
        const auto file = directory / (ModuleFileStem(library) + extension);
        if (std::filesystem::exists(file)) return file;
    }
    return std::nullopt;
}

LoadedModule LoadedModule::FromArchive(const std::filesystem::path& root, std::string_view build, std::string_view library)
{
    if (!std::filesystem::is_directory(root / std::string(build)))
        throw std::runtime_error("build " + std::string(build) + " is not in the build archive " + root.string());
    const auto file = FindArchivedModule(root, build, library);
    if (!file)
        throw std::runtime_error("build " + std::string(build) + " in " + root.string() + " has no " + ModuleFileStem(library) +
                                 ".so or .img");
    return Load(*file);
}

LoadedModule LoadedModule::Load(const std::filesystem::path& file)
{
    const auto bytes = ReadFile(file);
    const auto fail = [&](const std::string& why) {
        return std::runtime_error(file.string() + ": " + why);
    };
    if (bytes.size() < sizeof(Elf64_Ehdr) || std::memcmp(bytes.data(), ELFMAG, SELFMAG) != 0 || bytes[EI_CLASS] != ELFCLASS64)
        throw fail("not a 64-bit ELF image");

    // A flat image (.img) is already laid out by virtual address (file offset == VA; its program headers' file offsets
    // are stale). A real ELF is mapped segment by segment.
    const bool flat = file.extension() == ".img";
    Elf64_Ehdr header;
    std::memcpy(&header, bytes.data(), sizeof(header));
    if (header.e_phoff + static_cast<std::size_t>(header.e_phnum) * sizeof(Elf64_Phdr) > bytes.size()) throw fail("truncated program headers");
    std::vector<Elf64_Phdr> segments(header.e_phnum);
    for (std::size_t i = 0; i < segments.size(); ++i)
        std::memcpy(&segments[i], bytes.data() + header.e_phoff + i * header.e_phentsize, sizeof(Elf64_Phdr));

    std::uint64_t imageSize = flat ? bytes.size() : 0;
    for (const auto& segment : segments)
        if (segment.p_type == PT_LOAD) imageSize = std::max<std::uint64_t>(imageSize, segment.p_vaddr + segment.p_memsz);

    LoadedModule module;
    module.m_name = file.filename().string();
    module.m_memory.assign((imageSize + kWord - 1) / kWord, 0);
    auto* base = reinterpret_cast<std::uint8_t*>(module.m_memory.data());
    const auto baseAddress = reinterpret_cast<std::uintptr_t>(base);
    if (flat) std::memcpy(base, bytes.data(), bytes.size());
    else
        for (const auto& segment : segments)
        {
            if (segment.p_type != PT_LOAD) continue;
            if (segment.p_offset + segment.p_filesz > bytes.size()) throw fail("segment outside the file");
            std::memcpy(base + segment.p_vaddr, bytes.data() + segment.p_offset, segment.p_filesz);
        }

    const auto inImage = [&](std::uint64_t address, std::uint64_t size) {
        return address <= imageSize && size <= imageSize - address;
    };
    const auto read = [&](std::uint64_t address, auto& value) {
        if (!inImage(address, sizeof(value))) throw fail("read outside the image");
        std::memcpy(&value, base + address, sizeof(value));
    };

    // Relocations, from the dynamic section (inside the image in both formats).
    const auto dynamic = std::find_if(segments.begin(), segments.end(), [](const Elf64_Phdr& segment) {
        return segment.p_type == PT_DYNAMIC;
    });
    if (dynamic == segments.end()) throw fail("no PT_DYNAMIC");
    std::uint64_t rela = 0, relaSize = 0, jmprel = 0, jmprelSize = 0, symtab = 0, strtab = 0;
    for (std::uint64_t at = dynamic->p_vaddr;; at += sizeof(Elf64_Dyn))
    {
        Elf64_Dyn entry;
        read(at, entry);
        if (entry.d_tag == DT_NULL) break;
        switch (entry.d_tag)
        {
            case DT_RELA: rela = entry.d_un.d_ptr; break;
            case DT_RELASZ: relaSize = entry.d_un.d_val; break;
            case DT_JMPREL: jmprel = entry.d_un.d_ptr; break;
            case DT_PLTRELSZ: jmprelSize = entry.d_un.d_val; break;
            case DT_SYMTAB: symtab = entry.d_un.d_ptr; break;
            case DT_STRTAB: strtab = entry.d_un.d_ptr; break;
            case 36: throw fail("DT_RELR relocations are not supported"); // DT_RELR
            default: break;
        }
    }

    for (const auto [table, size] : { std::pair{ rela, relaSize }, std::pair{ jmprel, jmprelSize } })
    {
        for (std::uint64_t at = table; table != 0 && at + sizeof(Elf64_Rela) <= table + size; at += sizeof(Elf64_Rela))
        {
            Elf64_Rela relocation;
            read(at, relocation);
            const auto type = ELF64_R_TYPE(relocation.r_info);
            Elf64_Sym symbol{};
            if (type != R_X86_64_RELATIVE) read(symtab + ELF64_R_SYM(relocation.r_info) * sizeof(Elf64_Sym), symbol);

            std::uint64_t value;
            switch (type)
            {
                case R_X86_64_RELATIVE:
                    value = baseAddress + relocation.r_addend;
                    break;
                case R_X86_64_64:
                case R_X86_64_GLOB_DAT:
                {
                    const std::uint64_t addend = type == R_X86_64_64 ? relocation.r_addend : 0;
                    if (symbol.st_shndx != SHN_UNDEF)
                    {
                        value = baseAddress + symbol.st_value + addend;
                        break;
                    }
                    // Undefined: only the markers vtables need; everything else (libstdc++ typeinfo vtables) stays.
                    const auto* name = reinterpret_cast<const char*>(base + strtab + symbol.st_name);
                    if (!inImage(strtab + symbol.st_name, 1)) continue;
                    if (std::strcmp(name, "__cxa_pure_virtual") == 0) value = kPureVirtualMarker;
                    else if (std::strcmp(name, "__cxa_deleted_virtual") == 0)
                        value = kDeletedVirtualMarker;
                    else
                        continue;
                    break;
                }
                default:
                    continue;
            }
            if (!inImage(relocation.r_offset, sizeof(value))) continue;
            std::memcpy(base + relocation.r_offset, &value, sizeof(value));
        }
    }
    module.m_pureVirtual = kPureVirtualMarker;
    module.m_deletedVirtual = kDeletedVirtualMarker;

    // Regions, from program headers (valid for both formats): code, read-only data (typeinfo names) and RELRO
    // (typeinfo objects and vtables).
    for (const auto& segment : segments)
    {
        if (!inImage(segment.p_vaddr, segment.p_memsz)) continue;
        auto* start = base + segment.p_vaddr;
        if (segment.p_type == PT_LOAD && (segment.p_flags & PF_X))
            module.m_code.push_back({ reinterpret_cast<std::uintptr_t>(start), start, segment.p_filesz });
        else if (segment.p_type == PT_LOAD && segment.p_flags == PF_R)
            module.m_names.push_back({ start, segment.p_memsz });
        else if (segment.p_type == PT_GNU_RELRO)
            module.m_data.push_back({ start, segment.p_memsz });
    }
    if (module.m_code.empty() || module.m_names.empty() || module.m_data.empty()) throw fail("missing code, read-only data or RELRO segment");
    return module;
}

rtti::VtableLookup LoadedModule::FindVtable(std::string_view className) const
{
    return rtti::FindPrimaryVtable(className, m_names, m_data);
}

std::optional<std::uintptr_t> LoadedModule::ReadVtableSlot(std::uintptr_t vtable, std::size_t index) const
{
    const std::uintptr_t address = vtable + index * kWord;
    for (const auto& region : m_data)
    {
        const auto start = reinterpret_cast<std::uintptr_t>(region.data);
        if (address >= start && address + kWord <= start + region.size)
        {
            std::uint64_t value;
            std::memcpy(&value, reinterpret_cast<const void*>(address), sizeof(value));
            return value;
        }
    }
    return std::nullopt;
}

bindings::ModuleImage LoadedModule::Image() const
{
    bindings::ModuleImage image;
    image.segments = m_code;
    image.findVirtualTable = [this](std::string_view className) {
        return FindVtable(className);
    };
    image.readVtableSlot = [this](std::uintptr_t vtable, std::size_t index) {
        return ReadVtableSlot(vtable, index);
    };
    image.pureVirtual = m_pureVirtual;
    image.deletedVirtual = m_deletedVirtual;
    return image;
}

std::vector<vtable::Fingerprint> LoadedModule::Fingerprints(std::string_view className) const
{
    const auto lookup = FindVtable(className);
    if (!lookup.Found())
        throw std::runtime_error(m_name + ": " + std::to_string(lookup.matchCount) + " primary vtables of " + std::string(className) +
                                 " (need exactly one)");
    return vtable::FingerprintVtable({ [&](std::size_t index) {
                                          return ReadVtableSlot(lookup.address, index);
                                      },
                                       m_code, m_pureVirtual, m_deletedVirtual });
}

} // namespace counterstrikesharp::generator
