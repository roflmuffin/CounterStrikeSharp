#pragma once

// The Gamedata key of every Slot Binding native code hooks or calls, named once: the Engine Feature catalog
// (engine_features.cpp) and the call or hook site use the same constant. Pure data, no SDK. Keys are permanent.

#include <string_view>

namespace counterstrikesharp::bindings::slots {

// ISource2Server (CSource2Server, server)
inline constexpr std::string_view ISource2Server_GameFrame = "ISource2Server::GameFrame";
inline constexpr std::string_view ISource2Server_ServerHibernationUpdate = "ISource2Server::ServerHibernationUpdate";
inline constexpr std::string_view ISource2Server_GameServerSteamAPIActivated = "ISource2Server::GameServerSteamAPIActivated";
inline constexpr std::string_view ISource2Server_GameServerSteamAPIDeactivated = "ISource2Server::GameServerSteamAPIDeactivated";
inline constexpr std::string_view ISource2Server_OnHostNameChanged = "ISource2Server::OnHostNameChanged";
inline constexpr std::string_view ISource2Server_PreFatalShutdown = "ISource2Server::PreFatalShutdown";
inline constexpr std::string_view ISource2Server_UpdateWhenNotInGame = "ISource2Server::UpdateWhenNotInGame";
inline constexpr std::string_view ISource2Server_PreWorldUpdate = "ISource2Server::PreWorldUpdate";
inline constexpr std::string_view ISource2Server_GetEconItemSystem = "ISource2Server::GetEconItemSystem";
inline constexpr std::string_view ISource2Server_IsPaused = "ISource2Server::IsPaused";

// IServerGameClients (CSource2GameClients, server)
inline constexpr std::string_view IServerGameClients_ClientConnect = "IServerGameClients::ClientConnect";
inline constexpr std::string_view IServerGameClients_ClientPutInServer = "IServerGameClients::ClientPutInServer";
inline constexpr std::string_view IServerGameClients_ClientDisconnect = "IServerGameClients::ClientDisconnect";
inline constexpr std::string_view IServerGameClients_ClientCommand = "IServerGameClients::ClientCommand";
inline constexpr std::string_view IServerGameClients_ClientVoice = "IServerGameClients::ClientVoice";
inline constexpr std::string_view IServerGameClients_ClientSvcUserMessage = "IServerGameClients::ClientSvcUserMessage";

// IVEngineServer2 (CEngineServer, engine2)
inline constexpr std::string_view IVEngineServer2_SetClientListening = "IVEngineServer2::SetClientListening";
inline constexpr std::string_view IVEngineServer2_ServerCommand = "IVEngineServer2::ServerCommand";
inline constexpr std::string_view IVEngineServer2_IsMapValid = "IVEngineServer2::IsMapValid";
inline constexpr std::string_view IVEngineServer2_PrecacheGeneric = "IVEngineServer2::PrecacheGeneric";
inline constexpr std::string_view IVEngineServer2_GetPlayerUserId = "IVEngineServer2::GetPlayerUserId";
inline constexpr std::string_view IVEngineServer2_ClientPrintf = "IVEngineServer2::ClientPrintf";
inline constexpr std::string_view IVEngineServer2_GetClientConVarValue = "IVEngineServer2::GetClientConVarValue";
inline constexpr std::string_view IVEngineServer2_SetFakeClientConVarValue = "IVEngineServer2::SetFakeClientConVarValue";
inline constexpr std::string_view IVEngineServer2_ClientCommand = "IVEngineServer2::ClientCommand";
inline constexpr std::string_view IVEngineServer2_DisconnectClient = "IVEngineServer2::DisconnectClient";
inline constexpr std::string_view IVEngineServer2_IsClientFullyAuthenticated = "IVEngineServer2::IsClientFullyAuthenticated";
inline constexpr std::string_view IVEngineServer2_GetClientSteamID = "IVEngineServer2::GetClientSteamID";

// ICvar (CCvar, tier0)
inline constexpr std::string_view ICvar_FindConVar = "ICvar::FindConVar";
inline constexpr std::string_view ICvar_CallChangeCallback = "ICvar::CallChangeCallback";
inline constexpr std::string_view ICvar_CallFilterCallback = "ICvar::CallFilterCallback";
inline constexpr std::string_view ICvar_FindConCommand = "ICvar::FindConCommand";
inline constexpr std::string_view ICvar_DispatchConCommand = "ICvar::DispatchConCommand";
inline constexpr std::string_view ICvar_InstallGlobalChangeCallback = "ICvar::InstallGlobalChangeCallback";
inline constexpr std::string_view ICvar_RemoveGlobalChangeCallback = "ICvar::RemoveGlobalChangeCallback";
inline constexpr std::string_view ICvar_CallGlobalChangeCallbacks = "ICvar::CallGlobalChangeCallbacks";
inline constexpr std::string_view ICvar_GetCharacterSet = "ICvar::GetCharacterSet";
inline constexpr std::string_view ICvar_RegisterConVar = "ICvar::RegisterConVar";
inline constexpr std::string_view ICvar_UnregisterConVarCallbacks = "ICvar::UnregisterConVarCallbacks";
inline constexpr std::string_view ICvar_GetConVarData = "ICvar::GetConVarData";
inline constexpr std::string_view ICvar_RegisterConCommand = "ICvar::RegisterConCommand";
inline constexpr std::string_view ICvar_UnregisterConCommandCallbacks = "ICvar::UnregisterConCommandCallbacks";
inline constexpr std::string_view ICvar_GetConCommandData = "ICvar::GetConCommandData";
inline constexpr std::string_view ICvar_QueueThreadSetValue = "ICvar::QueueThreadSetValue";

// Interfaces that never shifted a slot we use (hooked by verified index; direct calls compiled, verified)
inline constexpr std::string_view IGameEventManager2_LoadEventsFromFile = "IGameEventManager2::LoadEventsFromFile";
inline constexpr std::string_view IGameEventManager2_AddListener = "IGameEventManager2::AddListener";
inline constexpr std::string_view IGameEventManager2_FindListener = "IGameEventManager2::FindListener";
inline constexpr std::string_view IGameEventManager2_RemoveListener = "IGameEventManager2::RemoveListener";
inline constexpr std::string_view IGameEventManager2_CreateEvent = "IGameEventManager2::CreateEvent";
inline constexpr std::string_view IGameEventManager2_FireEvent = "IGameEventManager2::FireEvent";
inline constexpr std::string_view IGameEventManager2_FreeEvent = "IGameEventManager2::FreeEvent";
inline constexpr std::string_view INetworkServerService_StartupServer = "INetworkServerService::StartupServer";
inline constexpr std::string_view INetworkServerService_GetIGameServer = "INetworkServerService::GetIGameServer";
inline constexpr std::string_view INetworkGameServer_GetGlobals = "INetworkGameServer::GetGlobals";
inline constexpr std::string_view IEngineServiceMgr_RegisterLoopMode = "IEngineServiceMgr::RegisterLoopMode";
inline constexpr std::string_view IEngineServiceMgr_FindService = "IEngineServiceMgr::FindService";
inline constexpr std::string_view IGameEventSystem_PostEventAbstract = "IGameEventSystem::PostEventAbstract";
inline constexpr std::string_view IGameEventSystem_PostEventAbstract_Filter = "IGameEventSystem::PostEventAbstract_Filter";

// Modules not fingerprinted yet (network messages, schema system): compiled calls, unverified
inline constexpr std::string_view INetworkMessages_FindNetworkMessagePartial = "INetworkMessages::FindNetworkMessagePartial";
inline constexpr std::string_view INetworkMessages_FindNetworkMessageById = "INetworkMessages::FindNetworkMessageById";
inline constexpr std::string_view CSchemaSystem_GlobalTypeScope = "CSchemaSystem::GlobalTypeScope";
inline constexpr std::string_view CSchemaSystem_FindTypeScopeForModule = "CSchemaSystem::FindTypeScopeForModule";

// Game-class virtuals native code calls or hooks through their Gamedata index
inline constexpr std::string_view CheckTransmit = "ISource2GameEntities::CheckTransmit";
inline constexpr std::string_view CBaseEntity_IsPlayerPawn = "CBaseEntity_IsPlayerPawn";
inline constexpr std::string_view SetStateChanged = "SetStateChanged"; // CEntityInstance::NetworkStateChanged

} // namespace counterstrikesharp::bindings::slots
