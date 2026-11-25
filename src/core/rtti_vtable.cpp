#include "core/rtti_vtable.h"

#include <algorithm>
#include <cstring>
#include <functional>
#include <set>
#include <string>

namespace counterstrikesharp::rtti {
namespace {

constexpr std::size_t kWord = sizeof(std::uintptr_t);

std::uintptr_t AddressOf(const std::uint8_t* pointer) { return reinterpret_cast<std::uintptr_t>(pointer); }

std::uintptr_t ReadWord(const std::uint8_t* at)
{
    std::uintptr_t value;
    std::memcpy(&value, at, kWord);
    return value;
}

// Calls visit(address of the word, address of the word before it or nullptr) for every pointer-aligned word whose
// value is in targets.
template <typename Visit>
void ForEachWordPointingAt(std::span<const MemoryRegion> regions, const std::set<std::uintptr_t>& targets, Visit visit)
{
    if (targets.empty()) return;
    const auto lowest = *targets.begin();
    const auto highest = *targets.rbegin();
    for (const auto& region : regions)
    {
        if (region.data == nullptr || region.size < kWord) continue;
        const auto misalignment = AddressOf(region.data) % kWord;
        const std::size_t first = misalignment == 0 ? 0 : kWord - misalignment;
        for (std::size_t offset = first; offset + kWord <= region.size; offset += kWord)
        {
            const auto* at = region.data + offset;
            const auto value = ReadWord(at);
            if (value < lowest || value > highest || !targets.contains(value)) continue;
            visit(at, offset >= kWord ? at - kWord : nullptr);
        }
    }
}

} // namespace

VtableLookup FindPrimaryVtable(std::string_view className, std::span<const MemoryRegion> names, std::span<const MemoryRegion> data)
{
    if (className.empty()) return {};

    // Every occurrence of the NUL-terminated decorated name, including those inside longer strings: a linker may store
    // the name as the tail of a longer one. What makes a match exact is a typeinfo pointing at the occurrence itself.
    std::string decorated = std::to_string(className.size());
    decorated.append(className);
    decorated.push_back('\0');
    const auto* needle = reinterpret_cast<const std::uint8_t*>(decorated.data());
    const std::boyer_moore_horspool_searcher searcher(needle, needle + decorated.size());

    std::set<std::uintptr_t> nameAddresses;
    for (const auto& region : names)
    {
        if (region.data == nullptr) continue;
        const auto* end = region.data + region.size;
        for (auto* at = std::search(region.data, end, searcher); at != end; at = std::search(at + 1, end, searcher))
            nameAddresses.insert(AddressOf(at));
    }

    // Typeinfo objects start one word before their name pointer (after the type_info vptr).
    std::set<std::uintptr_t> typeinfos;
    ForEachWordPointingAt(data, nameAddresses, [&](const std::uint8_t*, const std::uint8_t* typeinfo) {
        if (typeinfo != nullptr) typeinfos.insert(AddressOf(typeinfo));
    });

    // A vtable's typeinfo pointer is preceded by its offset-to-top, 0 for the primary vtable; the virtual functions
    // start right after the typeinfo pointer.
    std::set<std::uintptr_t> vtables;
    ForEachWordPointingAt(data, typeinfos, [&](const std::uint8_t* typeinfoPointer, const std::uint8_t* offsetToTop) {
        if (offsetToTop != nullptr && ReadWord(offsetToTop) == 0) vtables.insert(AddressOf(typeinfoPointer) + kWord);
    });

    VtableLookup lookup;
    lookup.matchCount = vtables.size();
    if (lookup.Found()) lookup.address = *vtables.begin();
    return lookup;
}

} // namespace counterstrikesharp::rtti
