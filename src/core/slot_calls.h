#pragma once

// Calling a method through its Slot Binding (ADR-0003): at the slot index the boot Vtable Check verified (or healed),
// not at the compiled header position, so a direct call follows the Gamedata (and a Heal) instead of the SDK headers.
// The counterpart of core/slot_hooks.h for calls.
//
// Pattern (gate on the Engine Feature first, so a disabled one is reported instead of called):
//   if (const auto index = SlotIndexOf("IVEngineServer2::IsMapValid", &IVEngineServer2::IsMapValid))
//       return CallAtSlot(*index, &IVEngineServer2::IsMapValid, globals::engine, mapName);

#include <cstdint>
#include <cstring>
#include <optional>
#include <string_view>
#include <type_traits>
#include <utility>

#include "core/core_bindings.h"
#ifdef _WIN32
#include "core/hooks.h"
#endif

namespace counterstrikesharp {

// The member function pointer of type F that calls address directly (non-virtual, no this adjustment). Itanium
// represents it as { address, 0 }, MSVC (single inheritance) as the address alone.
template <class F> F MemberFunctionAt(void* address)
{
    static_assert(std::is_member_function_pointer_v<F>, "F must be a member function pointer");
    struct Representation
    {
        void* address;
        std::intptr_t rest[3];
    } representation{ address, {} };
    static_assert(sizeof(F) <= sizeof(Representation), "unsupported member function pointer representation");
    F function;
    std::memcpy(&function, &representation, sizeof(F));
    return function;
}

// Calls the function instance's vtable holds at slot index, with method's signature. method only supplies the
// signature; its compiled position is not used.
template <class F, class Instance, class... Args>
decltype(auto) CallAtSlot(int index, [[maybe_unused]] F method, Instance* instance, Args&&... args)
{
    void** vtable = *reinterpret_cast<void***>(instance);
    const auto function = MemberFunctionAt<F>(vtable[index]);
    return (instance->*function)(std::forward<Args>(args)...);
}

// The slot to call method at for Slot Binding key: its verified (or healed) index; nullopt if the Binding failed or is
// not declared. On Windows Slot Bindings are unverified: the Gamedata index if it has one, else method's compiled
// header index, as before Slot Bindings.
template <class F> std::optional<int> SlotIndexOf(std::string_view key, [[maybe_unused]] F method)
{
    auto index = bindings::GetBindingSlot(key);
#ifdef _WIN32
    if (!index) index = KHook::GetVtableIndex(method);
#endif
    return index;
}

} // namespace counterstrikesharp
