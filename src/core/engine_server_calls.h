#pragma once

// Every direct call CounterStrikeSharp makes on IVEngineServer2 (engine2's CEngineServer), each through its Slot Binding
// "IVEngineServer2::<Method>" at the slot index the boot Vtable Check verified or healed (core/slot_calls.h), never at
// the compiled header position. Callers gate on the call's Engine Feature first (bindings::features::Player*/Server*,
// see engine_features.cpp) and report a disabled one; a call whose Slot Binding failed anyway does nothing and returns
// the stated default.

#include <engine2/ivengineserver2.h>

namespace counterstrikesharp::engine_server {

// cssharp.server/execute-command
void ServerCommand(const char* command);
// cssharp.server/is-map-valid; false if disabled.
bool IsMapValid(const char* mapName);
// cssharp.server/precache-model
void PrecacheGeneric(const char* name);
// cssharp.player/user-id; -1 if disabled.
int GetPlayerUserId(CPlayerSlot slot);
// cssharp.player/print-to-console
void ClientPrintf(CPlayerSlot slot, const char* message);
// cssharp.player/get-convar-value; nullptr if disabled.
const char* GetClientConVarValue(CPlayerSlot slot, const char* name);
// cssharp.player/set-fake-client-convar
void SetFakeClientConVarValue(CPlayerSlot slot, const char* name, const char* value);
// cssharp.player/execute-client-command
void ClientCommand(CPlayerSlot slot, const char* command);
// cssharp.player/disconnect
void DisconnectClient(CPlayerSlot slot, ENetworkDisconnectionReason reason);
// cssharp.player/authorization; false and nullptr if disabled.
bool IsClientFullyAuthenticated(CPlayerSlot slot);
const CSteamID* GetClientSteamID(CPlayerSlot slot);

} // namespace counterstrikesharp::engine_server
