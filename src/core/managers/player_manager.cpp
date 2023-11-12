/**
 * =============================================================================
 * SourceMod
 * Copyright (C) 2004-2016 AlliedModders LLC.  All rights reserved.
 * =============================================================================
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * As a special exception, AlliedModders LLC gives you permission to link the
 * code of this program (as well as its derivative works) to "Half-Life 2," the
 * "Source Engine," the "SourcePawn JIT," and any Game MODs that run on software
 * by the Valve Corporation.  You must obey the GNU General Public License in
 * all respects for all other code used.  Additionally, AlliedModders LLC grants
 * this exception to all derivative works.  AlliedModders LLC defines further
 * exceptions, found in LICENSE.txt (as of this writing, version JULY-31-2007),
 * or <http://www.sourcemod.net/license.php>.
 *
 * This file has been modified from its original form, under the GNU General
 * Public License, version 3.0.
 */

#include "core/managers/player_manager.h"
#include "core/managers/client_command_manager.h"

#include <public/eiface.h>
#include <public/inetchannelinfo.h>
#include <public/iserver.h>
#include <sourcehook/sourcehook.h>

#include "core/log.h"
#include "scripting/callback_manager.h"
#include <iplayerinfo.h>
// extern CEntitySystem *g_pEntitySystem;

SH_DECL_HOOK4_void(IServerGameClients, ClientActive, SH_NOATTRIB, 0, CPlayerSlot, bool, const char*,
                   uint64);
SH_DECL_HOOK5_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, CPlayerSlot, int,
                   const char*, uint64, const char*);
SH_DECL_HOOK4_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, CPlayerSlot, char const*,
                   int, uint64);
SH_DECL_HOOK1_void(IServerGameClients, ClientSettingsChanged, SH_NOATTRIB, 0, CPlayerSlot);
SH_DECL_HOOK6_void(IServerGameClients, OnClientConnected, SH_NOATTRIB, 0, CPlayerSlot, const char*,
                   uint64, const char*, const char*, bool);
SH_DECL_HOOK6(IServerGameClients, ClientConnect, SH_NOATTRIB, 0, bool, CPlayerSlot, const char*,
              uint64, const char*, bool, CBufferString*);

SH_DECL_HOOK2_void(IServerGameClients, ClientCommand, SH_NOATTRIB, 0, CPlayerSlot, const CCommand&);

namespace counterstrikesharp {

void PlayerManager::OnStartup() {}

void PlayerManager::OnAllInitialized()
{
    SH_ADD_HOOK(IServerGameClients, ClientConnect, globals::serverGameClients,
                SH_MEMBER(this, &PlayerManager::OnClientConnect), false);
    SH_ADD_HOOK(IServerGameClients, ClientConnect, globals::serverGameClients,
                SH_MEMBER(this, &PlayerManager::OnClientConnect_Post), true);
    SH_ADD_HOOK(IServerGameClients, ClientPutInServer, globals::serverGameClients,
                SH_MEMBER(this, &PlayerManager::OnClientPutInServer), true);
    SH_ADD_HOOK(IServerGameClients, ClientDisconnect, globals::serverGameClients,
                SH_MEMBER(this, &PlayerManager::OnClientDisconnect), false);
    SH_ADD_HOOK(IServerGameClients, ClientDisconnect, globals::serverGameClients,
                SH_MEMBER(this, &PlayerManager::OnClientDisconnect_Post), true);
    SH_ADD_HOOK(IServerGameClients, ClientCommand, globals::serverGameClients,
                SH_MEMBER(this, &PlayerManager::OnClientCommand), false);

    m_on_client_connect_callback = globals::callbackManager.CreateCallback("OnClientConnect");
    m_on_client_connected_callback = globals::callbackManager.CreateCallback("OnClientConnected");
    m_on_client_put_in_server_callback =
        globals::callbackManager.CreateCallback("OnClientPutInServer");
    m_on_client_disconnect_callback = globals::callbackManager.CreateCallback("OnClientDisconnect");
    m_on_client_disconnect_post_callback =
        globals::callbackManager.CreateCallback("OnClientDisconnectPost");
    m_on_client_authorized_callback = globals::callbackManager.CreateCallback("OnClientAuthorized");
}

void PlayerManager::OnShutdown()
{
    SH_REMOVE_HOOK(IServerGameClients, ClientConnect, globals::serverGameClients,
                   SH_MEMBER(this, &PlayerManager::OnClientConnect), false);
    SH_REMOVE_HOOK(IServerGameClients, ClientConnect, globals::serverGameClients,
                   SH_MEMBER(this, &PlayerManager::OnClientConnect_Post), true);
    SH_REMOVE_HOOK(IServerGameClients, ClientPutInServer, globals::serverGameClients,
                   SH_MEMBER(this, &PlayerManager::OnClientPutInServer), true);
    SH_REMOVE_HOOK(IServerGameClients, ClientDisconnect, globals::serverGameClients,
                   SH_MEMBER(this, &PlayerManager::OnClientDisconnect), false);
    SH_REMOVE_HOOK(IServerGameClients, ClientDisconnect, globals::serverGameClients,
                   SH_MEMBER(this, &PlayerManager::OnClientDisconnect_Post), true);
    SH_REMOVE_HOOK(IServerGameClients, ClientCommand, globals::serverGameClients,
                   SH_MEMBER(this, &PlayerManager::OnClientCommand), false);

    globals::callbackManager.ReleaseCallback(m_on_client_connect_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_connected_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_put_in_server_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_disconnect_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_disconnect_post_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_authorized_callback);
}

bool PlayerManager::OnClientConnect(CPlayerSlot slot, const char* pszName, uint64 xuid,
                                    const char* pszNetworkID, bool unk1,
                                    CBufferString* pRejectReason)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientConnect] - {}, {}, {}", slot.Get(), pszName,
                       pszNetworkID);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];

    if (pPlayer->IsConnected()) {
        OnClientDisconnect(slot, 0, pszName, xuid, pszNetworkID);
        OnClientDisconnect_Post(slot, 0, pszName, xuid, pszNetworkID);
    }

    pPlayer->Initialize(pszName, pszNetworkID, slot);

    m_on_client_connect_callback->ScriptContext().Reset();
    m_on_client_connect_callback->ScriptContext().Push(client);
    m_on_client_connect_callback->ScriptContext().Push(pszName);
    m_on_client_connect_callback->ScriptContext().Push(pszNetworkID);
    m_on_client_connect_callback->Execute();

    if (m_on_client_connect_callback->GetFunctionCount() > 0) {
        //        auto cancel = m_on_client_connect_callback->ScriptContext().GetArgument<bool>(0);
        //        auto cancelReason =
        //        m_on_client_connect_callback->ScriptContext().GetArgument<const char *>(1);
        //
        //        CSSHARP_CORE_TRACE("On Client Connect Callback Results: {}, {}", cancel,
        //        cancelReason);
        //
        //        if (cancel)
        //        {
        //            pRejectReason->AppendFormat("%s", cancelReason);
        //
        //            if (!pPlayer->IsFakeClient())
        //            {
        //                RETURN_META_VALUE(MRES_SUPERCEDE, false);
        //            }
        //        }
    }

    m_user_id_lookup[globals::engine->GetPlayerUserId(slot).Get()] = client;

    return true;
}

bool PlayerManager::OnClientConnect_Post(CPlayerSlot slot, const char* pszName, uint64 xuid,
                                         const char* pszNetworkID, bool unk1,
                                         CBufferString* pRejectReason)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientConnect_Post] - {}, {}, {}", slot.Get(), pszName,
                       pszNetworkID);

    int client = slot.Get();
    bool orig_value = META_RESULT_ORIG_RET(bool);
    CPlayer* pPlayer = &m_players[client];

    if (orig_value) {
        m_on_client_connected_callback->ScriptContext().Reset();
        m_on_client_connected_callback->ScriptContext().Push(pPlayer->m_slot.Get());
        m_on_client_connected_callback->Execute();

        if (!pPlayer->IsFakeClient() && m_is_listen_server &&
            strncmp(pszNetworkID, "127.0.0.1", 9) == 0) {
            m_listen_client = client;
        }
    } else {
        InvalidatePlayer(pPlayer);
    }

    return true;
}

void PlayerManager::OnClientPutInServer(CPlayerSlot slot, char const* pszName, int type,
                                        uint64 xuid)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientPutInServer] - {}, {}, {}", slot.Get(), pszName,
                       type);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];

    if (!pPlayer->IsConnected()) {
        pPlayer->m_is_fake_client = true;

        if (!OnClientConnect(slot, pszName, 0, "127.0.0.1", false,
                             new CBufferStringGrowable<255>())) {
            /* :TODO: kick the bot if it's rejected */
            return;
        }

        m_on_client_connected_callback->ScriptContext().Reset();
        m_on_client_connected_callback->ScriptContext().Push(pPlayer->m_slot.Get());
        m_on_client_connected_callback->Execute();
    }

    //    if (globals::playerinfoManager != nullptr)
    //    {
    //        pPlayer->m_info = globals::playerinfoManager->GetPlayerInfo(m_slot);
    //    }

    pPlayer->Connect();
    m_player_count++;

    //    globals::entityListener.HandleEntityCreated(pPlayer->GetBaseEntity(), client);
    //    globals::entityListener.HandleEntitySpawned(pPlayer->GetBaseEntity(), client);

    m_on_client_put_in_server_callback->ScriptContext().Reset();
    m_on_client_put_in_server_callback->ScriptContext().Push(pPlayer->m_slot.Get());
    m_on_client_put_in_server_callback->Execute();
}

void PlayerManager::OnClientDisconnect(CPlayerSlot slot,
                                       /* ENetworkDisconnectionReason */ int reason,
                                       const char* pszName, uint64 xuid, const char* pszNetworkID)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientDisconnect] - {}, {}, {}", slot.Get(), pszName,
                       pszNetworkID);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];

    if (pPlayer->IsConnected()) {
        m_on_client_disconnect_callback->ScriptContext().Reset();
        m_on_client_disconnect_callback->ScriptContext().Push(pPlayer->m_slot.Get());
        m_on_client_disconnect_callback->Execute();
    }

    if (pPlayer->WasCountedAsInGame()) {
        m_player_count--;
    }

    // globals::entityListener.HandleEntityDeleted(pPlayer->GetBaseEntity(), client);
}

void PlayerManager::OnClientDisconnect_Post(CPlayerSlot slot,
                                            /* ENetworkDisconnectionReason */ int reason,
                                            const char* pszName, uint64 xuid,
                                            const char* pszNetworkID) const
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientDisconnect_Post] - {}, {}, {}", slot.Get(), pszName,
                       pszNetworkID);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];
    if (!pPlayer->IsConnected()) {
        /* We don't care, prevent a double call */
        return;
    }

    InvalidatePlayer(pPlayer);

    m_on_client_disconnect_post_callback->ScriptContext().Reset();
    m_on_client_disconnect_post_callback->ScriptContext().Push(pPlayer->m_slot.Get());
    m_on_client_disconnect_post_callback->Execute();
}

void PlayerManager::OnLevelEnd()
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnLevelEnd]");

    for (int i = 0; i <= m_max_clients; i++) {
        if (m_players[i].IsConnected()) {
            OnClientDisconnect(m_players[i].m_slot, 0, m_players[i].GetName(), 0,
                               m_players[i].GetIpAddress());
            OnClientDisconnect_Post(m_players[i].m_slot, 0, m_players[i].GetName(), 0,
                                    m_players[i].GetIpAddress());
        }
    }
    m_player_count = 0;
}

void PlayerManager::OnClientCommand(CPlayerSlot slot, const CCommand& args) const
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientCommand] - {}, {}, {}", slot.Get(), args.Arg(0), (void*)&args);

    const char* cmd = args.Arg(0);

    bool response = globals::clientCommandManager.DispatchClientCommand(slot, cmd, &args);
    if (response) {
        RETURN_META(MRES_SUPERCEDE);
    }
}

int PlayerManager::ListenClient() const { return m_listen_client; }

int PlayerManager::NumPlayers() const { return m_player_count; }

int PlayerManager::MaxClients() const { return m_max_clients; }

CPlayer* PlayerManager::GetPlayerByIndex(int client) const
{
    if (client > m_max_clients || client < 1) {
        return nullptr;
    }

    return &m_players[client];
}

// CPlayer *PlayerManager::GetClientOfUserId(int user_id) const
//{
//     if (user_id < 0 || user_id > USHRT_MAX)
//     {
//         return nullptr;
//     }
//
//     int client = m_user_id_lookup[user_id];
//
//     /* Verify the userid.  The cache can get messed up with older
//      * Valve engines.  :TODO: If this gets fixed, do an old engine
//      * check before invoking this backwards compat code.
//      */
//     if (client)
//     {
//         CPlayer *player = GetPlayerByIndex(client);
//         if (player && player->IsConnected())
//         {
//             int realUserId = ExcUseridFromEdict(player->GetEdict());
//             if (realUserId == user_id)
//             {
//                 return player;
//             }
//         }
//     }
//
//     /* If we can't verify the userid, we have to do a manual loop */
//     CPlayer *player;
//     auto index = ExcIndexFromUserid(user_id);
//     player = GetPlayerByIndex(index);
//     if (player && player->IsConnected())
//     {
//         m_user_id_lookup[user_id] = index;
//         return player;
//     }
//
//     return nullptr;
// }

void PlayerManager::InvalidatePlayer(CPlayer* pPlayer) const
{
    auto userid = globals::engine->GetPlayerUserId(pPlayer->m_slot);
    if (userid.Get() != -1)
        m_user_id_lookup[userid.Get()] = 0;

    pPlayer->Disconnect();
}

CPlayer::CPlayer() {}

void CPlayer::Initialize(const char* name, const char* ip, CPlayerSlot slot)
{
    m_is_connected = true;
    m_slot = slot;
    m_name = std::string(name);
    m_ip_address = std::string(ip);
}

IPlayerInfo* CPlayer::GetPlayerInfo() const { return m_info; }

const char* CPlayer::GetName() const { return strdup(m_name.c_str()); }

bool CPlayer::IsConnected() const { return m_is_connected; }

bool CPlayer::IsFakeClient() const { return m_is_fake_client; }

bool CPlayer::IsAuthorized() const { return m_is_authorized; }

bool CPlayer::IsAuthStringValidated() const
{
    if (!IsFakeClient()) {
        return globals::engine->IsClientFullyAuthenticated(m_slot);
    }
    return false;
}

void CPlayer::Authorize() { m_is_authorized = true; }

void CPlayer::PrintToConsole(const char* message) const
{
    if (m_is_connected == false || m_is_fake_client == true) {
        return;
    }

    INetChannelInfo* pNetChan = globals::engine->GetPlayerNetInfo(m_slot);
    if (pNetChan == nullptr) {
        return;
    }

    globals::engine->ClientPrintf(m_slot, message);
}

// void CPlayer::PrintToChat(const char *message)
//{
//     globals::user_message_manager.SendMessageToChat(m_i_index, message);
// }
//
// void CPlayer::PrintToHint(const char *message)
//{
//     globals::user_message_manager.SendHintMessage(m_i_index, message);
// }
//
// void CPlayer::PrintToCenter(const char *message)
//{
//     globals::user_message_manager.SendCenterMessage(m_i_index, message);
// }

void CPlayer::SetName(const char* name) { m_name = strdup(name); }

INetChannelInfo* CPlayer::GetNetInfo() const { return globals::engine->GetPlayerNetInfo(m_slot); }

PlayerManager::PlayerManager()
{
    m_max_clients = 64;
    m_players = new CPlayer[66];
    m_player_count = 0;
    m_user_id_lookup = new int[USHRT_MAX + 1];
    memset(m_user_id_lookup, 0, sizeof(int) * (USHRT_MAX + 1));
}

void PlayerManager::RunAuthChecks()
{
    if (globals::getGlobalVars()->curtime - m_last_auth_check_time < 0.5F) {
        return;
    }

    m_last_auth_check_time = globals::getGlobalVars()->curtime;

    for (int i = 0; i <= m_max_clients; i++) {
        if (m_players[i].IsConnected()) {
            if (m_players[i].IsAuthorized() || m_players[i].IsFakeClient())
                continue;

            if (globals::engine->IsClientFullyAuthenticated(i)) {
                m_players[i].Authorize();
                m_players[i].SetSteamId(globals::engine->GetClientSteamID(i));
                OnAuthorized(&m_players[i]);
            }
        }
    }
}

void PlayerManager::OnAuthorized(CPlayer* player) const
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnAuthorized] - {} {}", player->GetName(),
                       player->GetSteamId()->ConvertToUint64());

    m_on_client_authorized_callback->ScriptContext().Reset();
    m_on_client_authorized_callback->ScriptContext().Push(player->m_slot.Get());
    m_on_client_authorized_callback->ScriptContext().Push(player->GetSteamId()->ConvertToUint64());
    m_on_client_authorized_callback->Execute();
}

bool CPlayer::WasCountedAsInGame() const { return m_is_in_game; }

int CPlayer::GetUserId()
{
    if (m_user_id == -1) {
        m_user_id = globals::engine->GetPlayerUserId(m_slot).Get();
    }

    return m_user_id;
}

bool CPlayer::IsInGame() const
{
    return m_is_in_game; // && (m_p_edict->GetUnknown() != nullptr);
}

void CPlayer::Kick(const char* kickReason)
{
    char buffer[255];
    sprintf(buffer, "kickid %d %s\n", GetUserId(), kickReason);
    globals::engine->ServerCommand(buffer);
}

const char* CPlayer::GetWeaponName() const { return m_info->GetWeaponName(); }

void CPlayer::ChangeTeam(int team) const { m_info->ChangeTeam(team); }

int CPlayer::GetTeam() const { return m_info->GetTeamIndex(); }

int CPlayer::GetArmor() const { return m_info->GetArmorValue(); }

int CPlayer::GetFrags() const { return m_info->GetFragCount(); }

int CPlayer::GetDeaths() const { return m_info->GetDeathCount(); }

const char* CPlayer::GetKeyValue(const char* key) const
{
    return globals::engine->GetClientConVarValue(m_slot, key);
}

Vector CPlayer::GetMaxSize() const { return m_info->GetPlayerMaxs(); }

Vector CPlayer::GetMinSize() const { return m_info->GetPlayerMins(); }

int CPlayer::GetMaxHealth() const { return m_info->GetMaxHealth(); }

const char* CPlayer::GetIpAddress() const { return m_ip_address.c_str(); }

const char* CPlayer::GetModelName() const { return m_info->GetModelName(); }

int CPlayer::GetUserId() const { return m_user_id; }

float CPlayer::GetTimeConnected() const
{
    if (!IsConnected() || IsFakeClient()) {
        return 0;
    }

    return GetNetInfo()->GetTimeConnected();
}

float CPlayer::GetLatency() const
{
    return GetNetInfo()->GetLatency(FLOW_INCOMING) + GetNetInfo()->GetLatency(FLOW_OUTGOING);
}

void CPlayer::Connect()
{
    if (m_is_in_game) {
        return;
    }

    m_is_in_game = true;
}

void CPlayer::Disconnect()
{
    m_is_connected = false;
    m_is_in_game = false;
    m_name.clear();
    m_info = nullptr;
    m_is_fake_client = false;
    m_user_id = -1;
    m_is_authorized = false;
    m_ip_address.clear();
}

QAngle CPlayer::GetAbsAngles() const { return m_info->GetAbsAngles(); }

Vector CPlayer::GetAbsOrigin() const { return m_info->GetAbsOrigin(); }

bool CPlayer::IsAlive() const
{
    if (!IsInGame()) {
        return false;
    }

    return !m_info->IsDead();
}
const CSteamID* CPlayer::GetSteamId() { return m_steamId; }
void CPlayer::SetSteamId(const CSteamID* steam_id) { m_steamId = steam_id; }

} // namespace counterstrikesharp