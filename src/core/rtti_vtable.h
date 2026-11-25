#pragma once

// Finding a class's primary vtable by RTTI in a loaded Itanium C++ ABI (Linux) module. Pure: works on memory regions
// handed in by the caller, so tests/native can drive it with synthetic modules. CModule::FindVirtualTable is the adapter.

#include <cstddef>
#include <cstdint>
#include <span>
#include <string_view>

namespace counterstrikesharp::rtti {

// Live, relocated module memory (e.g. a section), read where it lies in the process.
struct MemoryRegion
{
    const std::uint8_t* data = nullptr;
    std::size_t size = 0;
};

struct VtableLookup
{
    std::uintptr_t address = 0; // first virtual function slot of the vtable; 0 unless exactly one matched
    std::size_t matchCount = 0; // distinct primary vtables of the class

    [[nodiscard]] bool Found() const { return matchCount == 1; }
    [[nodiscard]] bool Ambiguous() const { return matchCount > 1; }
};

// The primary vtable (offset-to-top 0) of the class whose typeinfo name is exactly "<length><className>". A typeinfo
// counts only if its name pointer points at the start of that string, so a name that merely occurs inside a longer one
// (e.g. "12CCSGameRules" in the pointer type's "P12CCSGameRules") never matches. More than one match is ambiguous.
//   names: where typeinfo name strings live (.rodata)
//   data:  where typeinfo objects and vtables live (.data.rel.ro, .data.rel.ro.local)
VtableLookup FindPrimaryVtable(std::string_view className, std::span<const MemoryRegion> names, std::span<const MemoryRegion> data);

} // namespace counterstrikesharp::rtti
