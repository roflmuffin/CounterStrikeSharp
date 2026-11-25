#pragma once

// Hooking a method through its Slot Binding (ADR-0003): at the slot index the boot Vtable Check verified, not at the
// compiled header position, so the hook follows the Gamedata (and later a Heal) instead of the SDK headers.
//
// Pattern for every hook on a Slot Binding (gate first, so a disabled Engine Feature never reaches the hook):
//   constexpr SlotMethod kGameFrame{ bindings::slots::ISource2Server_GameFrame, &IServerGameDLL::GameFrame };
//   if (bindings::ShouldRegisterHooks(bindings::features::GameFrame, "GameFrame hook"))
//       HookSlot(m_hooks, kGameFrame, globals::server, this, nullptr, &CounterStrikeSharpMMPlugin::Hook_GameFrame);

#include <optional>

#include "core/core_bindings.h"
#include "core/hooks.h"
#include "core/slot_calls.h"

namespace counterstrikesharp {

// The index to hook slot at: the Slot Binding's index in the report; nullopt when the Binding is not usable. On Windows
// Slot Bindings are unverified: the Gamedata index if it has one, else the method's compiled header index, as before
// Slot Bindings.
template <class F> std::optional<int> HookIndex(const SlotMethod<F>& slot)
{
    auto index = bindings::GetBindingSlot(slot.key);
#ifdef _WIN32
    if (!index) index = KHook::GetVtableIndex(slot.method);
#endif
    return index;
}

// Hooks slot.method on instance at HookIndex(slot). Returns false (and hooks nothing) when the Binding is not usable.
template <class F, class Instance, class Context, class Pre, class Post>
bool HookSlot(HookSet& hooks, const SlotMethod<F>& slot, Instance* instance, Context* context, Pre pre, Post post)
{
    const auto index = HookIndex(slot);
    if (index) hooks.AddIndex(*index, slot.method, instance, context, pre, post);
    return index.has_value();
}

// As HookSlot, for every instance sharing vtable (e.g. a class's vtable found by RTTI).
template <class F, class Context, class Pre, class Post>
bool HookSlotVTable(HookSet& hooks, const SlotMethod<F>& slot, void** vtable, Context* context, Pre pre, Post post)
{
    const auto index = HookIndex(slot);
    if (index) hooks.AddVTableIndex(*index, slot.method, vtable, context, pre, post);
    return index.has_value();
}

} // namespace counterstrikesharp
