/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

#include "core/managers/server_manager.h"

#include "core/core_bindings.h"
#include "core/log.h"
#include "core/slot_calls.h"
#include "core/slot_binding_keys.h"
#include "core/slot_hooks.h"
#include "scripting/callback_manager.h"

#include "core/game_system.h"

namespace counterstrikesharp {

ServerManager::ServerManager() = default;

ServerManager::~ServerManager() = default;

void ServerManager::OnAllInitialized()
{
    // By the slot index the boot Vtable Check verified (ADR-0003). A disabled Engine Feature skips its hook; listeners
    // on it still register (their callbacks are created below) and never fire.
    using namespace bindings;
    if (ShouldRegisterHooks(features::HibernationEvents, "ServerHibernationUpdate hook"))
        HookSlot(m_hooks, SlotMethod{ bindings::slots::ISource2Server_ServerHibernationUpdate, &ISource2Server::ServerHibernationUpdate },
                 globals::server, this, nullptr, &ServerManager::ServerHibernationUpdate);
    if (ShouldRegisterHooks(features::SteamApiEvents, "Steam API hooks"))
    {
        HookSlot(m_hooks,
                 SlotMethod{ bindings::slots::ISource2Server_GameServerSteamAPIActivated, &ISource2Server::GameServerSteamAPIActivated },
                 globals::server, this, nullptr, &ServerManager::GameServerSteamAPIActivated);
        HookSlot(
            m_hooks,
            SlotMethod{ bindings::slots::ISource2Server_GameServerSteamAPIDeactivated, &ISource2Server::GameServerSteamAPIDeactivated },
            globals::server, this, nullptr, &ServerManager::GameServerSteamAPIDeactivated);
    }
    if (ShouldRegisterHooks(features::HostnameChanged, "OnHostNameChanged hook"))
        HookSlot(m_hooks, SlotMethod{ bindings::slots::ISource2Server_OnHostNameChanged, &ISource2Server::OnHostNameChanged },
                 globals::server, this, nullptr, &ServerManager::OnHostNameChanged);
    if (ShouldRegisterHooks(features::FatalShutdown, "PreFatalShutdown hook"))
        HookSlot(m_hooks, SlotMethod{ bindings::slots::ISource2Server_PreFatalShutdown, &ISource2Server::PreFatalShutdown },
                 globals::server, this, nullptr, &ServerManager::PreFatalShutdown);
    if (ShouldRegisterHooks(features::UpdateWhenNotInGame, "UpdateWhenNotInGame hook"))
        HookSlot(m_hooks, SlotMethod{ bindings::slots::ISource2Server_UpdateWhenNotInGame, &ISource2Server::UpdateWhenNotInGame },
                 globals::server, this, nullptr, &ServerManager::UpdateWhenNotInGame);
    if (ShouldRegisterHooks(features::PreWorldUpdate, "PreWorldUpdate hook"))
        HookSlot(m_hooks, SlotMethod{ bindings::slots::ISource2Server_PreWorldUpdate, &ISource2Server::PreWorldUpdate }, globals::server,
                 this, nullptr, &ServerManager::PreWorldUpdate);

    on_server_hibernation_update_callback = globals::callbackManager.CreateCallback("OnServerHibernationUpdate");
    on_server_steam_api_activated_callback = globals::callbackManager.CreateCallback("OnGameServerSteamAPIActivated");
    on_server_steam_api_deactivated_callback = globals::callbackManager.CreateCallback("OnGameServerSteamAPIDeactivated");
    on_server_hostname_changed_callback = globals::callbackManager.CreateCallback("OnHostNameChanged");
    on_server_pre_fatal_shutdown = globals::callbackManager.CreateCallback("OnPreFatalShutdown");
    on_server_update_when_not_in_game = globals::callbackManager.CreateCallback("OnUpdateWhenNotInGame");
    on_server_pre_world_update = globals::callbackManager.CreateCallback("OnServerPreWorldUpdate");
    on_server_pre_entity_think = globals::callbackManager.CreateCallback("OnServerPreEntityThink");
    on_server_post_entity_think = globals::callbackManager.CreateCallback("OnServerPostEntityThink");

    on_server_precache_resources = globals::callbackManager.CreateCallback("OnServerPrecacheResources");
}

void ServerManager::OnShutdown()
{
    m_hooks.Clear();

    globals::callbackManager.ReleaseCallback(on_server_hibernation_update_callback);
    globals::callbackManager.ReleaseCallback(on_server_steam_api_activated_callback);
    globals::callbackManager.ReleaseCallback(on_server_steam_api_deactivated_callback);
    globals::callbackManager.ReleaseCallback(on_server_hostname_changed_callback);
    globals::callbackManager.ReleaseCallback(on_server_pre_fatal_shutdown);
    globals::callbackManager.ReleaseCallback(on_server_update_when_not_in_game);
    globals::callbackManager.ReleaseCallback(on_server_pre_world_update);
    globals::callbackManager.ReleaseCallback(on_server_pre_entity_think);
    globals::callbackManager.ReleaseCallback(on_server_post_entity_think);

    globals::callbackManager.ReleaseCallback(on_server_precache_resources);
}

// Direct calls by verified slot index. Callers check the Engine Feature first (natives_server.cpp).
void* ServerManager::GetEconItemSystem()
{
    constexpr SlotMethod kGetEconItemSystem{ bindings::slots::ISource2Server_GetEconItemSystem, &ISource2Server::GetEconItemSystem };
    return CallSlot(kGetEconItemSystem, globals::server);
}

bool ServerManager::IsPaused()
{
    constexpr SlotMethod kIsPaused{ bindings::slots::ISource2Server_IsPaused, &ISource2Server::IsPaused };
    return CallSlot(kIsPaused, globals::server);
}

KHook::Return<void> ServerManager::ServerHibernationUpdate(ISource2Server* hookThis, bool bHibernating)
{
    CSSHARP_CORE_TRACE("Server hibernation update {0}", bHibernating);

    auto callback = globals::serverManager.on_server_hibernation_update_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(bHibernating);
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

KHook::Return<void> ServerManager::GameServerSteamAPIActivated(ISource2Server* hookThis)
{
    CSSHARP_CORE_TRACE("GameServerSteamAPIActivated");

    auto callback = globals::serverManager.on_server_steam_api_activated_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

KHook::Return<void> ServerManager::GameServerSteamAPIDeactivated(ISource2Server* hookThis)
{
    CSSHARP_CORE_TRACE("GameServerSteamAPIDeactivated");

    auto callback = globals::serverManager.on_server_steam_api_deactivated_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

KHook::Return<void> ServerManager::OnHostNameChanged(ISource2Server* hookThis, const char* pHostname)
{
    CSSHARP_CORE_TRACE("Server hostname changed {0}", pHostname);

    auto callback = globals::serverManager.on_server_hostname_changed_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pHostname);
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

KHook::Return<void> ServerManager::PreFatalShutdown(const ISource2Server* hookThis)
{
    CSSHARP_CORE_TRACE("Pre fatal shutdown");

    auto callback = globals::serverManager.on_server_pre_fatal_shutdown;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

KHook::Return<void> ServerManager::UpdateWhenNotInGame(ISource2Server* hookThis, float flFrameTime)
{
    CSSHARP_CORE_TRACE("Update when not in game {}", flFrameTime);

    auto callback = globals::serverManager.on_server_update_when_not_in_game;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(flFrameTime);
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

KHook::Return<void> ServerManager::PreWorldUpdate(ISource2Server* hookThis, bool bSimulating)
{
    auto callback = globals::serverManager.on_server_pre_world_update;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(bSimulating);
        callback->Execute();
    }
    return { KHook::Action::Ignore };
}

void ServerManager::OnPrecacheResources(IEntityResourceManifest* pResourceManifest)
{
    CSSHARP_CORE_TRACE("Precache resources");
    auto callback = globals::serverManager.on_server_precache_resources;
    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pResourceManifest);
        callback->Execute();
    }
}

} // namespace counterstrikesharp
