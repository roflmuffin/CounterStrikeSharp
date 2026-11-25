#include <doctest.h>

#include <cstdint>
#include <cstring>
#include <string>
#include <vector>

#include "core/rtti_vtable.h"

using namespace counterstrikesharp::rtti;

namespace {
// A synthetic Itanium module: typeinfo name strings in a read-only region, and typeinfo objects and vtables as 8-byte
// words in a relocated data region. Pointers are real process addresses into these buffers, as in a loaded module.
struct FakeItaniumModule
{
    std::string names;
    std::vector<std::uintptr_t> words;

    FakeItaniumModule()
    {
        names.reserve(4096);
        words.reserve(4096);
    }

    // Appends a NUL-terminated typeinfo name ("12CCSGameRules") and returns its address.
    std::uintptr_t AddName(const std::string& decorated)
    {
        const auto offset = names.size();
        names.append(decorated);
        names.push_back('\0');
        return reinterpret_cast<std::uintptr_t>(names.data() + offset);
    }

    // Appends a typeinfo object (vptr, name pointer) and returns its address.
    std::uintptr_t AddTypeinfo(std::uintptr_t name)
    {
        const auto at = Append({ 0xC1A55, name });
        return at;
    }

    // Appends a vtable group with the given offset-to-top and returns the address of its first virtual function slot.
    std::uintptr_t AddVtable(std::uintptr_t typeinfo, std::intptr_t offsetToTop = 0)
    {
        const auto at = Append({ static_cast<std::uintptr_t>(offsetToTop), typeinfo, 0xF00, 0xF01 });
        return at + 2 * sizeof(std::uintptr_t);
    }

    std::uintptr_t Append(std::initializer_list<std::uintptr_t> values)
    {
        const auto at = reinterpret_cast<std::uintptr_t>(words.data() + words.size());
        words.insert(words.end(), values);
        return at;
    }

    VtableLookup Find(std::string_view className) const
    {
        const MemoryRegion nameRegion{ reinterpret_cast<const std::uint8_t*>(names.data()), names.size() };
        const MemoryRegion dataRegion{ reinterpret_cast<const std::uint8_t*>(words.data()), words.size() * sizeof(std::uintptr_t) };
        return FindPrimaryVtable(className, { &nameRegion, 1 }, { &dataRegion, 1 });
    }
};
} // namespace

TEST_CASE("A class whose decorated name is a substring of another type's name resolves to its own vtable")
{
    FakeItaniumModule module;
    // The pointer type's name comes first and contains "12CCSGameRules\0" one byte in.
    const auto pointerTypeinfo = module.AddTypeinfo(module.AddName("P12CCSGameRules"));
    module.AddVtable(pointerTypeinfo);
    const auto classTypeinfo = module.AddTypeinfo(module.AddName("12CCSGameRules"));
    const auto vtable = module.AddVtable(classTypeinfo);

    const auto lookup = module.Find("CCSGameRules");
    CHECK(lookup.Found());
    CHECK(lookup.matchCount == 1);
    CHECK(lookup.address == vtable);
}

TEST_CASE("A class whose typeinfo name is stored as the tail of a longer name still resolves")
{
    // A linker may merge "12CCSGameRules" into the tail of "P12CCSGameRules"; the class typeinfo then points one byte in.
    FakeItaniumModule module;
    const auto pointerName = module.AddName("P12CCSGameRules");
    module.AddTypeinfo(pointerName);
    const auto vtable = module.AddVtable(module.AddTypeinfo(pointerName + 1));

    const auto lookup = module.Find("CCSGameRules");
    CHECK(lookup.Found());
    CHECK(lookup.address == vtable);
}

TEST_CASE("A longer class name containing the decorated name does not match")
{
    FakeItaniumModule module;
    module.AddVtable(module.AddTypeinfo(module.AddName("15A12CCSGameRules")));

    const auto lookup = module.Find("CCSGameRules");
    CHECK_FALSE(lookup.Found());
    CHECK(lookup.matchCount == 0);
    CHECK(lookup.address == 0);
}

TEST_CASE("Two primary vtables for the class are reported as ambiguous, not resolved to the first")
{
    FakeItaniumModule module;
    const auto typeinfo = module.AddTypeinfo(module.AddName("17CGameEventManager"));
    module.AddVtable(typeinfo);
    module.AddVtable(typeinfo);

    const auto lookup = module.Find("CGameEventManager");
    CHECK_FALSE(lookup.Found());
    CHECK(lookup.Ambiguous());
    CHECK(lookup.matchCount == 2);
    CHECK(lookup.address == 0);
}

TEST_CASE("Secondary vtables of the class do not count as matches")
{
    FakeItaniumModule module;
    const auto typeinfo = module.AddTypeinfo(module.AddName("20CNavPhysicsInterface"));
    const auto primary = module.AddVtable(typeinfo);
    module.AddVtable(typeinfo, -8);

    const auto lookup = module.Find("CNavPhysicsInterface");
    CHECK(lookup.Found());
    CHECK(lookup.address == primary);
}

TEST_CASE("A class without a typeinfo name or without a vtable is not found")
{
    FakeItaniumModule module;
    module.AddTypeinfo(module.AddName("7CNoVtbl"));

    CHECK(module.Find("CNoVtbl").matchCount == 0);
    CHECK(module.Find("CMissing").matchCount == 0);
    CHECK(module.Find("").matchCount == 0);
}
