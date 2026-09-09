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

#include "core/log.h"
#include "scripting/callback_manager.h"

#include "core/game_system.h"

namespace counterstrikesharp {

ServerManager::ServerManager()
    : m_ServerHibernationUpdate(&ISource2Server::ServerHibernationUpdate, this, nullptr, &ServerManager::ServerHibernationUpdate),
      m_GameServerSteamAPIActivated(
          &ISource2Server::GameServerSteamAPIActivated, this, nullptr, &ServerManager::GameServerSteamAPIActivated),
      m_GameServerSteamAPIDeactivated(
          &ISource2Server::GameServerSteamAPIDeactivated, this, nullptr, &ServerManager::GameServerSteamAPIDeactivated),
      m_OnHostNameChanged(&ISource2Server::OnHostNameChanged, this, nullptr, &ServerManager::OnHostNameChanged),
      m_PreFatalShutdown(&ISource2Server::PreFatalShutdown, this, nullptr, &ServerManager::PreFatalShutdown),
      m_UpdateWhenNotInGame(&ISource2Server::UpdateWhenNotInGame, this, nullptr, &ServerManager::UpdateWhenNotInGame),
      m_PreWorldUpdate(&ISource2Server::PreWorldUpdate, this, nullptr, &ServerManager::PreWorldUpdate)
{
}

ServerManager::~ServerManager() = default;

void ServerManager::OnAllInitialized()
{
    m_ServerHibernationUpdate.Add(globals::server);
    m_GameServerSteamAPIActivated.Add(globals::server);
    m_GameServerSteamAPIDeactivated.Add(globals::server);
    m_OnHostNameChanged.Add(globals::server);
    m_PreFatalShutdown.Add(globals::server);
    m_UpdateWhenNotInGame.Add(globals::server);
    m_PreWorldUpdate.Add(globals::server);

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
    m_ServerHibernationUpdate.Remove(globals::server);
    m_GameServerSteamAPIActivated.Remove(globals::server);
    m_GameServerSteamAPIDeactivated.Remove(globals::server);
    m_OnHostNameChanged.Remove(globals::server);
    m_PreFatalShutdown.Remove(globals::server);
    m_UpdateWhenNotInGame.Remove(globals::server);
    m_PreWorldUpdate.Remove(globals::server);

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

void* ServerManager::GetEconItemSystem() { return globals::server->GetEconItemSystem(); }

bool ServerManager::IsPaused() { return globals::server->IsPaused(); }

KHook::Return<void> ServerManager::ServerHibernationUpdate(ISource2Server*, bool bHibernating)
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

KHook::Return<void> ServerManager::GameServerSteamAPIActivated(ISource2Server*)
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

KHook::Return<void> ServerManager::GameServerSteamAPIDeactivated(ISource2Server*)
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

KHook::Return<void> ServerManager::OnHostNameChanged(ISource2Server*, const char* pHostname)
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

KHook::Return<void> ServerManager::PreFatalShutdown(const ISource2Server*)
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

KHook::Return<void> ServerManager::UpdateWhenNotInGame(ISource2Server*, float flFrameTime)
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

KHook::Return<void> ServerManager::PreWorldUpdate(ISource2Server*, bool bSimulating)
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
