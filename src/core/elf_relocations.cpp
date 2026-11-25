#include "core/elf_relocations.h"

#ifdef __linux__
#include <elf.h>

#include <algorithm>
#include <cstring>

namespace counterstrikesharp::elf {

RelocatedWords::RelocatedWords(const DynamicTables& tables, const ResolveSymbol& resolve)
{
    const auto* symbols = static_cast<const Elf64_Sym*>(tables.symbols);
    for (const auto [table, size] : { std::pair{ tables.rela, tables.relaSize }, std::pair{ tables.jmprel, tables.jmprelSize } })
    {
        if (table == nullptr) continue;
        const auto* relocations = static_cast<const Elf64_Rela*>(table);
        for (std::size_t i = 0; i < size / sizeof(Elf64_Rela); ++i)
        {
            const auto& relocation = relocations[i];
            const auto type = ELF64_R_TYPE(relocation.r_info);
            std::uintptr_t value = 0;
            if (type == R_X86_64_RELATIVE)
            {
                value = tables.base + relocation.r_addend;
            }
            else if ((type == R_X86_64_64 || type == R_X86_64_GLOB_DAT) && symbols != nullptr)
            {
                const auto& symbol = symbols[ELF64_R_SYM(relocation.r_info)];
                std::uintptr_t address = 0;
                if (symbol.st_shndx != SHN_UNDEF) address = tables.base + symbol.st_value;
                else if (resolve && tables.strings != nullptr)
                    address = resolve(tables.strings + symbol.st_name);
                if (address == 0) continue;
                value = address + (type == R_X86_64_64 ? relocation.r_addend : 0);
            }
            else
            {
                continue;
            }
            m_words.emplace_back(tables.base + relocation.r_offset, value);
        }
    }
    std::sort(m_words.begin(), m_words.end());
}

std::optional<std::uintptr_t> RelocatedWords::At(std::uintptr_t address) const
{
    const auto it = std::lower_bound(m_words.begin(), m_words.end(), std::make_pair(address, std::uintptr_t{ 0 }));
    if (it == m_words.end() || it->first != address) return std::nullopt;
    return it->second;
}

} // namespace counterstrikesharp::elf

#else

namespace counterstrikesharp::elf {
RelocatedWords::RelocatedWords(const DynamicTables&, const ResolveSymbol&) {}
std::optional<std::uintptr_t> RelocatedWords::At(std::uintptr_t) const { return std::nullopt; }
} // namespace counterstrikesharp::elf

#endif
