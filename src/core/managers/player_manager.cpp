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
#include "core/managers/con_command_manager.h"
#include "core/managers/voice_manager.h"

#include <public/eiface.h>
#include <public/inetchannelinfo.h>
#include <public/iserver.h>

#include "core/log.h"
#include "core/timer_system.h"
#include "scripting/callback_manager.h"
#include <iplayerinfo.h>
#include "player_manager.h"
#include <entity2/entitysystem.h>
#include "entity/dump.h"
#include <vprof.h>
// extern CEntitySystem *g_pEntitySystem;

namespace counterstrikesharp {

void PlayerManager::OnStartup() {}

void PlayerManager::OnAllInitialized()
{
    m_ClientConnect.Add(globals::serverGameClients);
    m_ClientPutInServer.Add(globals::serverGameClients);
    m_ClientDisconnect.Add(globals::serverGameClients);
    m_ClientCommand.Add(globals::serverGameClients);
    m_ClientVoice.Add(globals::serverGameClients);

    m_on_client_connect_callback = globals::callbackManager.CreateCallback("OnClientConnect");
    m_on_client_connected_callback = globals::callbackManager.CreateCallback("OnClientConnected");
    m_on_client_put_in_server_callback = globals::callbackManager.CreateCallback("OnClientPutInServer");
    m_on_client_disconnect_callback = globals::callbackManager.CreateCallback("OnClientDisconnect");
    m_on_client_disconnect_post_callback = globals::callbackManager.CreateCallback("OnClientDisconnectPost");
    m_on_client_voice_callback = globals::callbackManager.CreateCallback("OnClientVoice");
    m_on_client_authorized_callback = globals::callbackManager.CreateCallback("OnClientAuthorized");
    m_on_player_buttons_changed_callback = globals::callbackManager.CreateCallback("OnPlayerButtonsChanged");
}

void PlayerManager::OnShutdown()
{
    m_ClientConnect.Remove(globals::serverGameClients);
    m_ClientPutInServer.Remove(globals::serverGameClients);
    m_ClientDisconnect.Remove(globals::serverGameClients);
    m_ClientCommand.Remove(globals::serverGameClients);
    m_ClientVoice.Remove(globals::serverGameClients);

    globals::callbackManager.ReleaseCallback(m_on_client_connect_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_connected_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_put_in_server_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_disconnect_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_disconnect_post_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_authorized_callback);
    globals::callbackManager.ReleaseCallback(m_on_client_voice_callback);
    globals::callbackManager.ReleaseCallback(m_on_player_buttons_changed_callback);
}

KHook::Return<bool> PlayerManager::OnClientConnect(IServerGameClients* pGameClients,
                                                   CPlayerSlot slot,
                                                   const char* pszName,
                                                   uint64 xuid,
                                                   const char* pszNetworkID,
                                                   bool unk1,
                                                   CBufferString* pRejectReason)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientConnect] - {}, {}, {}", slot.Get(), pszName, pszNetworkID);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];

    if (pPlayer->IsConnected())
    {
        OnClientDisconnect(pGameClients, slot, ENetworkDisconnectionReason::NETWORK_DISCONNECT_INVALID, pszName, xuid, pszNetworkID);
        OnClientDisconnect_Post(pGameClients, slot, ENetworkDisconnectionReason::NETWORK_DISCONNECT_INVALID, pszName, xuid, pszNetworkID);
    }

    pPlayer->Initialize(pszName, pszNetworkID, slot);

    m_on_client_connect_callback->ScriptContext().Reset();
    m_on_client_connect_callback->ScriptContext().Push(client);
    m_on_client_connect_callback->ScriptContext().Push(pszName);
    m_on_client_connect_callback->ScriptContext().Push(pszNetworkID);
    m_on_client_connect_callback->Execute();

    if (m_on_client_connect_callback->GetFunctionCount() > 0)
    {
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
        //                return { KHook::Action::Supersede, false };
        //            }
        //        }
    }

    m_user_id_lookup[globals::engine->GetPlayerUserId(slot).Get()] = client;

    return { KHook::Action::Ignore, true };
}

KHook::Return<bool> PlayerManager::OnClientConnect_Post(IServerGameClients* pGameClients,
                                                        CPlayerSlot slot,
                                                        const char* pszName,
                                                        uint64 xuid,
                                                        const char* pszNetworkID,
                                                        bool unk1,
                                                        CBufferString* pRejectReason)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientConnect_Post] - {}, {}, {}", slot.Get(), pszName, pszNetworkID);

    int client = slot.Get();
    // KHook has no original return storage when another plugin blocks the call.
    auto* original = static_cast<bool*>(KHook::GetOriginalValuePtr());
    auto* overridden = static_cast<bool*>(KHook::GetOverrideValuePtr());
    bool orig_value = original ? *original : (overridden ? *overridden : false);
    CPlayer* pPlayer = &m_players[client];

    if (orig_value)
    {
        m_on_client_connected_callback->ScriptContext().Reset();
        m_on_client_connected_callback->ScriptContext().Push(pPlayer->m_slot.Get());
        m_on_client_connected_callback->Execute();

        if (!pPlayer->IsFakeClient() && m_is_listen_server && strncmp(pszNetworkID, "127.0.0.1", 9) == 0)
        {
            m_listen_client = client;
        }
    }
    else
    {
        InvalidatePlayer(pPlayer);
    }

    return { KHook::Action::Ignore, true };
}

KHook::Return<void>
PlayerManager::OnClientPutInServer(IServerGameClients* pGameClients, CPlayerSlot slot, char const* pszName, int type, uint64 xuid)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientPutInServer] - {}, {}, {}", slot.Get(), pszName, type);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];

    if (!pPlayer->IsConnected())
    {
        pPlayer->m_is_fake_client = true;

        if (!OnClientConnect(pGameClients, slot, pszName, 0, "127.0.0.1", false, new CBufferStringGrowable<255>()).ret)
        {
            /* :TODO: kick the bot if it's rejected */
            return { KHook::Action::Ignore };
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

    return { KHook::Action::Ignore };
}

KHook::Return<void> PlayerManager::OnClientDisconnect(IServerGameClients* pGameClients,
                                                      CPlayerSlot slot,
                                                      ENetworkDisconnectionReason reason,
                                                      const char* pszName,
                                                      uint64 xuid,
                                                      const char* pszNetworkID)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientDisconnect] - {}, {}, {}", slot.Get(), pszName, pszNetworkID);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];

    if (pPlayer->IsConnected())
    {
        m_on_client_disconnect_callback->ScriptContext().Reset();
        m_on_client_disconnect_callback->ScriptContext().Push(pPlayer->m_slot.Get());
        m_on_client_disconnect_callback->ScriptContext().Push(reason);
        m_on_client_disconnect_callback->Execute();
    }

    if (pPlayer->WasCountedAsInGame())
    {
        m_player_count--;
    }

    // globals::entityListener.HandleEntityDeleted(pPlayer->GetBaseEntity(), client);

    return { KHook::Action::Ignore };
}

KHook::Return<void> PlayerManager::OnClientDisconnect_Post(IServerGameClients* pGameClients,
                                                           CPlayerSlot slot,
                                                           ENetworkDisconnectionReason reason,
                                                           const char* pszName,
                                                           uint64 xuid,
                                                           const char* pszNetworkID)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientDisconnect_Post] - {}, {}, {}", slot.Get(), pszName, pszNetworkID);

    int client = slot.Get();
    CPlayer* pPlayer = &m_players[client];
    if (!pPlayer->IsConnected())
    {
        /* We don't care, prevent a double call */
        return { KHook::Action::Ignore };
    }

    InvalidatePlayer(pPlayer);

    m_on_client_disconnect_post_callback->ScriptContext().Reset();
    m_on_client_disconnect_post_callback->ScriptContext().Push(pPlayer->m_slot.Get());
    m_on_client_disconnect_post_callback->ScriptContext().Push(reason);
    m_on_client_disconnect_post_callback->Execute();

    return { KHook::Action::Ignore };
}

KHook::Return<void> PlayerManager::OnClientVoice(IServerGameClients* pGameClients, CPlayerSlot slot)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientVoice] - {}", slot.Get());

    m_on_client_voice_callback->ScriptContext().Reset();
    m_on_client_voice_callback->ScriptContext().Push(slot.Get());
    m_on_client_voice_callback->Execute();

    return { KHook::Action::Ignore };
}

void PlayerManager::OnLevelEnd()
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnLevelEnd]");

    for (int i = 0; i <= MaxClients(); i++)
    {
        if (m_players[i].IsConnected())
        {
            OnClientDisconnect(globals::serverGameClients, m_players[i].m_slot, ENetworkDisconnectionReason::NETWORK_DISCONNECT_INVALID,
                               m_players[i].GetName(), 0, m_players[i].GetIpAddress());
            OnClientDisconnect_Post(globals::serverGameClients, m_players[i].m_slot,
                                    ENetworkDisconnectionReason::NETWORK_DISCONNECT_INVALID, m_players[i].GetName(), 0,
                                    m_players[i].GetIpAddress());
        }
    }
    m_player_count = 0;
}

KHook::Return<void> PlayerManager::OnClientCommand(IServerGameClients* pGameClients, CPlayerSlot slot, const CCommand& args)
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnClientCommand] - {}, {}, {}", slot.Get(), args.Arg(0), (void*)&args);

    const char* cmd = args.Arg(0);

    globals::voiceManager.OnClientCommand(slot, args);

    auto result = globals::conCommandManager.ExecuteCommandCallbacks(cmd, CCommandContext(CommandTarget_t::CT_NO_TARGET, slot), args,
                                                                     HookMode::Pre, CommandCallingContext::Console);

    if (result >= HookResult::Handled)
    {
        return { KHook::Action::Supersede };
    }

    return { KHook::Action::Ignore };
}

int PlayerManager::ListenClient() const { return m_listen_client; }

int PlayerManager::NumPlayers() const { return m_player_count; }

int PlayerManager::MaxClients() const { return globals::getGlobalVars()->maxClients; }

CPlayer* PlayerManager::GetPlayerBySlot(int client) const
{
    if (client > MaxClients() || client < 0)
    {
        return nullptr;
    }

    return &m_players[client];
}

void PlayerManager::RunThink() const
{
    // VPROF_BUDGET("CS#::PlayerManager::RunThink", "CS# On Frame");

    for (int i = 0; i <= MaxClients(); i++)
    {
        auto player = GetPlayerBySlot(i);
        auto pController = (CCSPlayerController*)globals::entitySystem->GetEntityInstance(CEntityIndex(i + 1));

        if (!player || !pController || !pController->m_hPlayerPawn().IsValid())
        {
            continue;
        }

        if (((CBasePlayerController*)pController)->m_iConnected() != PlayerConnectedState::PlayerConnected) continue;

        auto pawn = pController->m_hPlayerPawn().Get();
        if (!pawn || !pawn->m_pMovementServices()) continue;

        auto buttonStates = pawn->m_pMovementServices()->m_nButtons().m_pButtonStates();
        const auto buttons = buttonStates[0];

        const auto lastButtons = player->m_buttonState;
        const auto buttonsChanged = buttons ^ lastButtons;

        const auto buttonsPressedThisFrame = (~lastButtons) & buttons;
        const auto buttonsReleasedThisFrame = lastButtons & (~buttons);

        if (buttonsPressedThisFrame || buttonsReleasedThisFrame)
        {
            auto callback = m_on_player_buttons_changed_callback;

            if (callback && callback->GetFunctionCount())
            {
                callback->ScriptContext().Reset();
                callback->ScriptContext().Push(pController);
                callback->ScriptContext().Push(buttonsPressedThisFrame);
                callback->ScriptContext().Push(buttonsReleasedThisFrame);
                callback->Execute();
            }
        }

        player->m_buttonState = buttons;
    }
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
    if (userid.Get() != -1) m_user_id_lookup[userid.Get()] = 0;

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
    if (!IsFakeClient())
    {
        return globals::engine->IsClientFullyAuthenticated(m_slot);
    }
    return false;
}

void CPlayer::Authorize() { m_is_authorized = true; }

void CPlayer::PrintToConsole(const char* message) const
{
    if (m_is_connected == false || m_is_fake_client == true)
    {
        return;
    }

    INetChannelInfo* pNetChan = globals::engine->GetPlayerNetInfo(m_slot);
    if (pNetChan == nullptr)
    {
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
    : m_ClientConnect(&IServerGameClients::ClientConnect, this, &PlayerManager::OnClientConnect, &PlayerManager::OnClientConnect_Post),
      m_ClientPutInServer(&IServerGameClients::ClientPutInServer, this, nullptr, &PlayerManager::OnClientPutInServer),
      m_ClientDisconnect(
          &IServerGameClients::ClientDisconnect, this, &PlayerManager::OnClientDisconnect, &PlayerManager::OnClientDisconnect_Post),
      m_ClientCommand(&IServerGameClients::ClientCommand, this, &PlayerManager::OnClientCommand, nullptr),
      m_ClientVoice(&IServerGameClients::ClientVoice, this, nullptr, &PlayerManager::OnClientVoice)
{
    m_players = new CPlayer[66];
    m_player_count = 0;
    m_user_id_lookup = new int[USHRT_MAX + 1];
    memset(m_user_id_lookup, 0, sizeof(int) * (USHRT_MAX + 1));
}

void PlayerManager::RunAuthChecks()
{
    if (globals::timerSystem.GetTickedTime() - m_last_auth_check_time < 0.5F)
    {
        return;
    }

    m_last_auth_check_time = globals::timerSystem.GetTickedTime();

    for (int i = 0; i <= MaxClients(); i++)
    {
        if (m_players[i].IsConnected())
        {
            if (m_players[i].IsAuthorized() || m_players[i].IsFakeClient()) continue;

            if (globals::engine->IsClientFullyAuthenticated(i))
            {
                m_players[i].Authorize();
                m_players[i].SetSteamId(globals::engine->GetClientSteamID(i));
                OnAuthorized(&m_players[i]);
            }
        }
    }
}

void PlayerManager::OnAuthorized(CPlayer* player) const
{
    CSSHARP_CORE_TRACE("[PlayerManager][OnAuthorized] - {} {}", player->GetName(), player->GetSteamId()->ConvertToUint64());

    m_on_client_authorized_callback->ScriptContext().Reset();
    m_on_client_authorized_callback->ScriptContext().Push(player->m_slot.Get());
    m_on_client_authorized_callback->ScriptContext().Push(player->GetSteamId()->ConvertToUint64());
    m_on_client_authorized_callback->Execute();
}

bool CPlayer::WasCountedAsInGame() const { return m_is_in_game; }

int CPlayer::GetUserId()
{
    if (m_user_id == -1)
    {
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

const char* CPlayer::GetKeyValue(const char* key) const { return globals::engine->GetClientConVarValue(m_slot, key); }

Vector CPlayer::GetMaxSize() const { return m_info->GetPlayerMaxs(); }

Vector CPlayer::GetMinSize() const { return m_info->GetPlayerMins(); }

int CPlayer::GetMaxHealth() const { return m_info->GetMaxHealth(); }

const char* CPlayer::GetIpAddress() const { return m_ip_address.c_str(); }

const char* CPlayer::GetModelName() const { return m_info->GetModelName(); }

int CPlayer::GetUserId() const { return m_user_id; }

float CPlayer::GetTimeConnected() const
{
    if (!IsConnected() || IsFakeClient())
    {
        return 0;
    }

    return GetNetInfo()->GetTimeConnected();
}

void CPlayer::SetListen(CPlayerSlot slot, ListenOverride listen) { m_listenMap[slot.Get()] = listen; }

void CPlayer::SetVoiceFlags(VoiceFlag_t flags) { m_voiceFlag = flags; }

VoiceFlag_t CPlayer::GetVoiceFlags() { return m_voiceFlag; }

ListenOverride CPlayer::GetListen(CPlayerSlot slot) const { return m_listenMap[slot.Get()]; }

void CPlayer::Connect()
{
    if (m_is_in_game)
    {
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
    m_selfMutes->ClearAll();
    memset(m_listenMap, 0, sizeof m_listenMap);
    m_voiceFlag = 0;
}

QAngle CPlayer::GetAbsAngles() const { return m_info->GetAbsAngles(); }

Vector CPlayer::GetAbsOrigin() const { return m_info->GetAbsOrigin(); }

bool CPlayer::IsAlive() const
{
    if (!IsInGame())
    {
        return false;
    }

    return !m_info->IsDead();
}
const CSteamID* CPlayer::GetSteamId() { return m_steamId; }
void CPlayer::SetSteamId(const CSteamID* steam_id) { m_steamId = steam_id; }

} // namespace counterstrikesharp
