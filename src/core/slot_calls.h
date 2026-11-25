#pragma once

// Calling a virtual method through a slot index (a verified Slot Binding, ADR-0003) instead of its compiled header
// position, so a direct call follows the Gamedata (and a Heal) the way hooks do through core/slot_hooks.h.
//
// Itanium C++ ABI only (Linux): a pointer to a virtual member function is { 1 + byte offset in the vtable, this
// adjustment }. Windows keeps its compiled calls (no Vtable Check there).

#include <cstddef>
#include <cstdint>
#include <cstring>
#include <utility>

namespace counterstrikesharp {

#ifndef _WIN32
// A pointer to member function of type F that dispatches through vtable slot index. F names a virtual method of the
// class whose primary vtable holds the slot (no this adjustment).
template <class F> F MethodAtSlot(int index)
{
    struct ItaniumMemberFunction
    {
        std::uintptr_t pointer;
        std::ptrdiff_t adjustment;
    };
    static_assert(sizeof(F) == sizeof(ItaniumMemberFunction), "Itanium pointer to member function expected");
    const ItaniumMemberFunction raw{ static_cast<std::uintptr_t>(index) * sizeof(void*) + 1, 0 };
    F method;
    std::memcpy(&method, &raw, sizeof(method));
    return method;
}

// Calls instance's virtual at slot index with method's signature; method itself is not called.
template <class F, class Instance, class... Args> decltype(auto) CallSlot(int index, F method, Instance* instance, Args&&... args)
{
    (void)method;
    return (instance->*MethodAtSlot<F>(index))(std::forward<Args>(args)...);
}
#endif

} // namespace counterstrikesharp
