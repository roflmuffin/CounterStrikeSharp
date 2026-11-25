#pragma once

// Calling a method through its Slot Binding (ADR-0003): at the slot index the boot Vtable Check verified (or healed),
// not at the compiled header position. The caller gates on the method's Engine Feature first and looks the index up
// once (the Resolution Report is immutable after boot):
//   static const auto slot = bindings::GetBindingSlot("ISource2Server::IsPaused");
//   return CallSlot(slot, &ISource2Server::IsPaused, globals::server);

#include <cstdint>
#include <cstring>
#include <optional>
#include <stdexcept>
#include <type_traits>
#include <utility>

namespace counterstrikesharp {

namespace detail {
// The member function pointer of type F that calls address directly (non-virtual, no this adjustment): Itanium
// represents it as { address, 0 }, MSVC (single inheritance) as the address alone. Calling through it keeps the
// method's own calling convention (class-type returns, C varargs), which a cast to a plain function would not on MSVC.
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

template <class F, class C, class... Args> decltype(auto) CallAtIndex(int index, F method, C* instance, Args&&... args)
{
    (void)method; // only its type: the function called is the one at index
    auto** vtable = *reinterpret_cast<void***>(const_cast<std::remove_const_t<C>*>(instance));
    const auto function = MemberFunctionAt<F>(vtable[index]);
    return (instance->*function)(std::forward<Args>(args)...);
}

// What CallSlot returns on Linux without an index: R{}. A return type without a default value (e.g. CPlayerUserId)
// requires the caller to check the index first.
template <class R> R NoSlotResult()
{
    if constexpr (std::is_void_v<R> || std::is_default_constructible_v<R>) return R();
    else throw std::logic_error("CallSlot without a Slot Binding index for a return type without a default value");
}
} // namespace detail

// Calls method on instance at the Slot Binding's index. On Windows Slot Bindings are unverified: without a Gamedata
// index the compiled call is made, as before Slot Bindings. On Linux a missing index (Engine Feature not checked by
// the caller) returns R{} instead of calling anything.
template <class R, class C, class... Params, class... Args>
R CallSlot(std::optional<int> index, R (C::*method)(Params...), C* instance, Args&&... args)
{
    if (!index)
    {
#ifdef _WIN32
        return (instance->*method)(std::forward<Args>(args)...);
#else
        return detail::NoSlotResult<R>();
#endif
    }
    return detail::CallAtIndex(*index, method, instance, std::forward<Args>(args)...);
}

template <class R, class C, class... Params, class... Args>
R CallSlot(std::optional<int> index, R (C::*method)(Params...) const, const C* instance, Args&&... args)
{
    if (!index)
    {
#ifdef _WIN32
        return (instance->*method)(std::forward<Args>(args)...);
#else
        return detail::NoSlotResult<R>();
#endif
    }
    return detail::CallAtIndex(*index, method, instance, std::forward<Args>(args)...);
}

// A C-variadic method (printf-style); the variadic arguments are passed on unchanged.
template <class R, class C, class... Params, class... Args>
R CallSlot(std::optional<int> index, R (C::*method)(Params..., ...), C* instance, Args&&... args)
{
    if (!index)
    {
#ifdef _WIN32
        return (instance->*method)(std::forward<Args>(args)...);
#else
        return detail::NoSlotResult<R>();
#endif
    }
    return detail::CallAtIndex(*index, method, instance, std::forward<Args>(args)...);
}

} // namespace counterstrikesharp
