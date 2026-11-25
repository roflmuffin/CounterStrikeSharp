#pragma once

// Hooking a method through its Slot Binding (ADR-0003): at the slot index the boot Vtable Check verified, not at the
// compiled header position, so the hook follows the Gamedata (and later a Heal) instead of the SDK headers.
//
// Pattern for every hook on a Slot Binding (gate first, so a disabled Engine Feature never reaches the hook):
//   if (bindings::ShouldRegisterHooks(bindings::features::GameFrame, "GameFrame hook"))
//       HookSlot(m_hooks, "ISource2Server::GameFrame", &IServerGameDLL::GameFrame, globals::server, this, nullptr,
//                &CounterStrikeSharpMMPlugin::Hook_GameFrame);

#include <string_view>

#include "core/core_bindings.h"
#include "core/hooks.h"

namespace counterstrikesharp {

// Hooks method on instance at the Slot Binding key's index. Returns false (and hooks nothing) when the Binding is not
// usable. On Windows Slot Bindings are unverified: the Gamedata index if it has one, else method's compiled header
// index, as before Slot Bindings.
template <class F, class Instance, class Context, class Pre, class Post>
bool HookSlot(HookSet& hooks, std::string_view key, F method, Instance* instance, Context* context, Pre pre, Post post)
{
    auto index = bindings::GetBindingSlot(key);
#ifdef _WIN32
    if (!index) index = KHook::GetVtableIndex(method);
#endif
    if (!index) return false;
    hooks.AddIndex(*index, method, instance, context, pre, post);
    return true;
}

} // namespace counterstrikesharp
