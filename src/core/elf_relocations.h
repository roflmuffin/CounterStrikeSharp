#pragma once

// The pointer-sized words a module's dynamic relocations put in place when it is loaded (Linux, x86-64 ELF): what a
// vtable slot held before anyone patched it. Metamod itself rewrites ISource2Server::Init in place while plugins load,
// and hooking libraries rewrite slots they hook, so live vtable memory is not the original; the relocation tables are
// read-only and untouched. Shared by the boot-time adapter (CModule, from the loaded module) and the generator's
// archive loader (from a file), so both read slots the same way. Pure.

#include <cstddef>
#include <cstdint>
#include <functional>
#include <optional>
#include <string_view>
#include <utility>
#include <vector>

namespace counterstrikesharp::elf {

// A module's dynamic relocation tables as mapped (all pointers into the module's memory).
struct DynamicTables
{
    std::uintptr_t base = 0; // load base: where virtual address 0 of the module is
    const void* rela = nullptr; // DT_RELA, Elf64_Rela[]
    std::size_t relaSize = 0; // DT_RELASZ, bytes
    const void* jmprel = nullptr; // DT_JMPREL, Elf64_Rela[]
    std::size_t jmprelSize = 0; // DT_PLTRELSZ, bytes
    const void* symbols = nullptr; // DT_SYMTAB, Elf64_Sym[]
    const char* strings = nullptr; // DT_STRTAB
};

// Address of an undefined symbol the module imports, or 0 if unknown (its words are then left out).
using ResolveSymbol = std::function<std::uintptr_t(std::string_view name)>;

class RelocatedWords
{
  public:
    RelocatedWords() = default;
    // R_X86_64_RELATIVE: base + addend; R_X86_64_64 / GLOB_DAT: symbol (+ addend for 64), a defined symbol at
    // base + st_value, an undefined one through resolve. Other relocation types are left out.
    RelocatedWords(const DynamicTables& tables, const ResolveSymbol& resolve);

    // The word the loader wrote at address (absolute), or nullopt if no relocation targets it.
    [[nodiscard]] std::optional<std::uintptr_t> At(std::uintptr_t address) const;

    // Every relocated word, by address.
    [[nodiscard]] const std::vector<std::pair<std::uintptr_t, std::uintptr_t>>& All() const { return m_words; }

  private:
    std::vector<std::pair<std::uintptr_t, std::uintptr_t>> m_words; // (address, value), sorted by address
};

} // namespace counterstrikesharp::elf
