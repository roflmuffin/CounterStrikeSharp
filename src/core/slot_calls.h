#pragma once

// Calling a method through its Slot Binding (ADR-0003): at the slot index the boot Vtable Check verified (or healed),
// not at the compiled header position. The caller gates on the method's Engine Feature first and looks the index up
// once (the Resolution Report is immutable after boot):
//   static const auto slot = bindings::GetBindingSlot("ISource2Server::IsPaused");
//   return CallSlot(slot, &ISource2Server::IsPaused, globals::server);

#include <optional>
#include <type_traits>
#include <utility>

namespace counterstrikesharp {

namespace detail {
template <class R, class C, class... Params, class... Args> R CallAtIndex(int index, C* instance, Args&&... args)
{
    // Itanium and x64 MSVC alike: a virtual is a plain function taking this first.
    using Function = R (*)(C*, Params...);
    auto** vtable = *reinterpret_cast<void***>(const_cast<std::remove_const_t<C>*>(instance));
    return reinterpret_cast<Function>(vtable[index])(instance, std::forward<Args>(args)...);
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
        (void)method;
        return R();
#endif
    }
    return detail::CallAtIndex<R, C, Params...>(*index, instance, std::forward<Args>(args)...);
}

template <class R, class C, class... Params, class... Args>
R CallSlot(std::optional<int> index, R (C::*method)(Params...) const, const C* instance, Args&&... args)
{
    if (!index)
    {
#ifdef _WIN32
        return (instance->*method)(std::forward<Args>(args)...);
#else
        (void)method;
        return R();
#endif
    }
    return detail::CallAtIndex<R, const C, Params...>(*index, instance, std::forward<Args>(args)...);
}

} // namespace counterstrikesharp
