#include "core/engine_server_calls.h"

#include "core/core_bindings.h"
#include "core/globals.h"
#include "core/slot_calls.h"

namespace counterstrikesharp::engine_server {

// Each index is looked up once, on the first call (after boot; the Resolution Report is immutable then).

void ServerCommand(const char* command)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::ServerCommand");
    CallSlot(index, &IVEngineServer2::ServerCommand, globals::engine, command);
}

bool IsMapValid(const char* mapName)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::IsMapValid");
    return CallSlot(index, &IVEngineServer2::IsMapValid, globals::engine, mapName) != 0;
}

void PrecacheGeneric(const char* name)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::PrecacheGeneric");
    CallSlot(index, &IVEngineServer2::PrecacheGeneric, globals::engine, name, false);
}

int GetPlayerUserId(CPlayerSlot slot)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::GetPlayerUserId");
#ifndef _WIN32
    if (!index) return -1; // CPlayerUserId has no default value
#endif
    return CallSlot(index, &IVEngineServer2::GetPlayerUserId, globals::engine, slot).Get();
}

void ClientPrintf(CPlayerSlot slot, const char* message)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::ClientPrintf");
    CallSlot(index, &IVEngineServer2::ClientPrintf, globals::engine, slot, message);
}

const char* GetClientConVarValue(CPlayerSlot slot, const char* name)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::GetClientConVarValue");
    return CallSlot(index, &IVEngineServer2::GetClientConVarValue, globals::engine, slot, name);
}

void SetFakeClientConVarValue(CPlayerSlot slot, const char* name, const char* value)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::SetFakeClientConVarValue");
    CallSlot(index, &IVEngineServer2::SetFakeClientConVarValue, globals::engine, slot, name, value);
}

// As before: the command is the format string.
void ClientCommand(CPlayerSlot slot, const char* command)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::ClientCommand");
    CallSlot(index, &IVEngineServer2::ClientCommand, globals::engine, slot, command);
}

void DisconnectClient(CPlayerSlot slot, ENetworkDisconnectionReason reason)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::DisconnectClient");
    CallSlot(index, &IVEngineServer2::DisconnectClient, globals::engine, slot, reason, static_cast<const char*>(nullptr));
}

bool IsClientFullyAuthenticated(CPlayerSlot slot)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::IsClientFullyAuthenticated");
    return CallSlot(index, &IVEngineServer2::IsClientFullyAuthenticated, globals::engine, slot);
}

const CSteamID* GetClientSteamID(CPlayerSlot slot)
{
    static const auto index = bindings::GetBindingSlot("IVEngineServer2::GetClientSteamID");
    return CallSlot(index, &IVEngineServer2::GetClientSteamID, globals::engine, slot);
}

} // namespace counterstrikesharp::engine_server
