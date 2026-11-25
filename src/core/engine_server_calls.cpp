#include "core/engine_server_calls.h"

#include "core/core_bindings.h"
#include "core/globals.h"
#include "core/slot_binding_keys.h"
#include "core/slot_calls.h"

namespace counterstrikesharp::engine_server {

namespace {
namespace slots = bindings::slots;

constexpr SlotMethod kServerCommand{ slots::IVEngineServer2_ServerCommand, &IVEngineServer2::ServerCommand };
constexpr SlotMethod kIsMapValid{ slots::IVEngineServer2_IsMapValid, &IVEngineServer2::IsMapValid };
constexpr SlotMethod kPrecacheGeneric{ slots::IVEngineServer2_PrecacheGeneric, &IVEngineServer2::PrecacheGeneric };
constexpr SlotMethod kGetPlayerUserId{ slots::IVEngineServer2_GetPlayerUserId, &IVEngineServer2::GetPlayerUserId };
constexpr SlotMethod kClientPrintf{ slots::IVEngineServer2_ClientPrintf, &IVEngineServer2::ClientPrintf };
constexpr SlotMethod kGetClientConVarValue{ slots::IVEngineServer2_GetClientConVarValue, &IVEngineServer2::GetClientConVarValue };
constexpr SlotMethod kSetFakeClientConVarValue{ slots::IVEngineServer2_SetFakeClientConVarValue,
                                                &IVEngineServer2::SetFakeClientConVarValue };
constexpr SlotMethod kClientCommand{ slots::IVEngineServer2_ClientCommand, &IVEngineServer2::ClientCommand };
constexpr SlotMethod kDisconnectClient{ slots::IVEngineServer2_DisconnectClient, &IVEngineServer2::DisconnectClient };
constexpr SlotMethod kIsClientFullyAuthenticated{ slots::IVEngineServer2_IsClientFullyAuthenticated,
                                                  &IVEngineServer2::IsClientFullyAuthenticated };
constexpr SlotMethod kGetClientSteamID{ slots::IVEngineServer2_GetClientSteamID, &IVEngineServer2::GetClientSteamID };
} // namespace

void ServerCommand(const char* command) { CallSlot(kServerCommand, globals::engine, command); }

bool IsMapValid(const char* mapName) { return CallSlot(kIsMapValid, globals::engine, mapName) != 0; }

void PrecacheGeneric(const char* name) { CallSlot(kPrecacheGeneric, globals::engine, name, false); }

int GetPlayerUserId(CPlayerSlot slot)
{
#ifndef _WIN32
    if (!bindings::GetBindingSlot(kGetPlayerUserId.key)) return -1; // CPlayerUserId has no default value
#endif
    return CallSlot(kGetPlayerUserId, globals::engine, slot).Get();
}

void ClientPrintf(CPlayerSlot slot, const char* message) { CallSlot(kClientPrintf, globals::engine, slot, message); }

const char* GetClientConVarValue(CPlayerSlot slot, const char* name)
{
    return CallSlot(kGetClientConVarValue, globals::engine, slot, name);
}

void SetFakeClientConVarValue(CPlayerSlot slot, const char* name, const char* value)
{
    CallSlot(kSetFakeClientConVarValue, globals::engine, slot, name, value);
}

// As before: the command is the format string.
void ClientCommand(CPlayerSlot slot, const char* command) { CallSlot(kClientCommand, globals::engine, slot, command); }

void DisconnectClient(CPlayerSlot slot, ENetworkDisconnectionReason reason)
{
    CallSlot(kDisconnectClient, globals::engine, slot, reason, static_cast<const char*>(nullptr));
}

bool IsClientFullyAuthenticated(CPlayerSlot slot) { return CallSlot(kIsClientFullyAuthenticated, globals::engine, slot); }

const CSteamID* GetClientSteamID(CPlayerSlot slot) { return CallSlot(kGetClientSteamID, globals::engine, slot); }

} // namespace counterstrikesharp::engine_server
