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

SH_DECL_HOOK1_void(ISource2Server, ServerHibernationUpdate, SH_NOATTRIB, 0, bool);
SH_DECL_HOOK0_void(ISource2Server, GameServerSteamAPIActivated, SH_NOATTRIB, 0);
SH_DECL_HOOK0_void(ISource2Server, GameServerSteamAPIDeactivated, SH_NOATTRIB, 0);
SH_DECL_HOOK1_void(ISource2Server, OnHostNameChanged, SH_NOATTRIB, 0, const char*);
SH_DECL_HOOK0_void(ISource2Server, PreFatalShutdown, const, 0);
SH_DECL_HOOK1_void(ISource2Server, UpdateWhenNotInGame, SH_NOATTRIB, 0, float);
SH_DECL_HOOK1_void(ISource2Server, PreWorldUpdate, SH_NOATTRIB, 0, bool);

namespace counterstrikesharp {

ServerManager::ServerManager() = default;

ServerManager::~ServerManager() = default;

void ServerManager::OnAllInitialized() {
    SH_ADD_HOOK(ISource2Server, ServerHibernationUpdate, globals::server,
                SH_MEMBER(this, &ServerManager::ServerHibernationUpdate), true);
    SH_ADD_HOOK(ISource2Server, GameServerSteamAPIActivated, globals::server,
                SH_MEMBER(this, &ServerManager::GameServerSteamAPIActivated), true);
    SH_ADD_HOOK(ISource2Server, GameServerSteamAPIDeactivated, globals::server,
                SH_MEMBER(this, &ServerManager::GameServerSteamAPIDeactivated), true);
    SH_ADD_HOOK(ISource2Server, OnHostNameChanged, globals::server,
                SH_MEMBER(this, &ServerManager::OnHostNameChanged), true);
    SH_ADD_HOOK(ISource2Server, PreFatalShutdown, globals::server,
                SH_MEMBER(this, &ServerManager::PreFatalShutdown), true);
    SH_ADD_HOOK(ISource2Server, UpdateWhenNotInGame, globals::server,
                SH_MEMBER(this, &ServerManager::UpdateWhenNotInGame), true);
    SH_ADD_HOOK(ISource2Server, PreWorldUpdate, globals::server,
                SH_MEMBER(this, &ServerManager::PreWorldUpdate), true);

    on_server_hibernation_update_callback = globals::callbackManager.CreateCallback("OnServerHibernationUpdate");
    on_server_steam_api_activated_callback = globals::callbackManager.CreateCallback("OnGameServerSteamAPIActivated");
    on_server_steam_api_deactivated_callback = globals::callbackManager.CreateCallback("OnGameServerSteamAPIDeactivated");
    on_server_hostname_changed_callback = globals::callbackManager.CreateCallback("OnHostNameChanged");
    on_server_pre_fatal_shutdown = globals::callbackManager.CreateCallback("OnPreFatalShutdown");
    on_server_update_when_not_in_game = globals::callbackManager.CreateCallback("OnUpdateWhenNotInGame");
    on_server_pre_world_update = globals::callbackManager.CreateCallback("OnServerPreWorldUpdate");
}

void ServerManager::OnShutdown() {
    SH_REMOVE_HOOK(ISource2Server, ServerHibernationUpdate, globals::server,
                SH_MEMBER(this, &ServerManager::ServerHibernationUpdate), true);
    SH_REMOVE_HOOK(ISource2Server, GameServerSteamAPIActivated, globals::server,
                SH_MEMBER(this, &ServerManager::GameServerSteamAPIActivated), true);
    SH_REMOVE_HOOK(ISource2Server, GameServerSteamAPIDeactivated, globals::server,
                SH_MEMBER(this, &ServerManager::GameServerSteamAPIDeactivated), true);
    SH_REMOVE_HOOK(ISource2Server, OnHostNameChanged, globals::server,
                SH_MEMBER(this, &ServerManager::OnHostNameChanged), true);
    SH_REMOVE_HOOK(ISource2Server, PreFatalShutdown, globals::server,
                SH_MEMBER(this, &ServerManager::PreFatalShutdown), true);
    SH_REMOVE_HOOK(ISource2Server, UpdateWhenNotInGame, globals::server,
                SH_MEMBER(this, &ServerManager::UpdateWhenNotInGame), true);
    SH_REMOVE_HOOK(ISource2Server, PreWorldUpdate, globals::server,
                SH_MEMBER(this, &ServerManager::PreWorldUpdate), true);

    globals::callbackManager.ReleaseCallback(on_server_hibernation_update_callback);
    globals::callbackManager.ReleaseCallback(on_server_steam_api_activated_callback);
    globals::callbackManager.ReleaseCallback(on_server_steam_api_deactivated_callback);
    globals::callbackManager.ReleaseCallback(on_server_hostname_changed_callback);
    globals::callbackManager.ReleaseCallback(on_server_pre_fatal_shutdown);
    globals::callbackManager.ReleaseCallback(on_server_update_when_not_in_game);
    globals::callbackManager.ReleaseCallback(on_server_pre_world_update);
}

void* ServerManager::GetEconItemSystem()
{
    return globals::server->GetEconItemSystem();
}

bool ServerManager::IsPaused()
{
    return globals::server->IsPaused();
}

void ServerManager::ServerHibernationUpdate(bool bHibernating)
{
    CSSHARP_CORE_TRACE("Server hibernation update {0}", bHibernating);

    auto callback = globals::serverManager.on_server_hibernation_update_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(bHibernating);
        callback->Execute();
    }
}

void ServerManager::GameServerSteamAPIActivated()
{
    CSSHARP_CORE_TRACE("GameServerSteamAPIActivated");

    auto callback = globals::serverManager.on_server_steam_api_activated_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->Execute();
    }
}

void ServerManager::GameServerSteamAPIDeactivated()
{
    CSSHARP_CORE_TRACE("GameServerSteamAPIDeactivated");

    auto callback = globals::serverManager.on_server_steam_api_deactivated_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->Execute();
    }
}

void ServerManager::OnHostNameChanged(const char *pHostname)
{
    CSSHARP_CORE_TRACE("Server hostname changed {0}", pHostname);

    auto callback = globals::serverManager.on_server_hostname_changed_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pHostname);
        callback->Execute();
    }
}

void ServerManager::PreFatalShutdown()
{
    CSSHARP_CORE_TRACE("Pre fatal shutdown");

    auto callback = globals::serverManager.on_server_pre_fatal_shutdown;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->Execute();
    }
}

void ServerManager::UpdateWhenNotInGame(float flFrameTime)
{
    CSSHARP_CORE_TRACE("Update when not in game {}", flFrameTime);

    auto callback = globals::serverManager.on_server_update_when_not_in_game;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(flFrameTime);
        callback->Execute();
    }
}

void ServerManager::PreWorldUpdate(bool bSimulating)
{
    std::lock_guard<std::mutex> lock(m_nextWorldUpdateTasksLock);

    if (m_nextWorldUpdateTasks.empty())
        return;

    CSSHARP_CORE_TRACE("Executing queued tasks of size: {0} at time {1}", m_nextWorldUpdateTasks.size(),
                       globals::getGlobalVars()->curtime);

    for (size_t i = 0; i < m_nextWorldUpdateTasks.size(); i++) {
        m_nextWorldUpdateTasks[i]();
    }

    m_nextWorldUpdateTasks.clear();

    auto callback = globals::serverManager.on_server_pre_world_update;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(bSimulating);
        callback->Execute();
    }
}

void ServerManager::AddTaskForNextWorldUpdate(std::function<void()>&& task)
{
    std::lock_guard<std::mutex> lock(m_nextWorldUpdateTasksLock);
    m_nextWorldUpdateTasks.push_back(std::forward<decltype(task)>(task));
}
}  // namespace counterstrikesharp