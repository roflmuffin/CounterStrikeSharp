#include "core/engine_server_calls.h"

#include <string_view>

#include "core/globals.h"
#include "core/slot_calls.h"

namespace counterstrikesharp::engine_server {
namespace {

// Calls method on the engine server through Slot Binding key; fallback if the Binding failed.
template <class F, class R, class... Args> R CallOr(std::string_view key, F method, R fallback, Args... args)
{
    const auto index = SlotIndexOf(key, method);
    if (!index) return fallback;
    return CallAtSlot(*index, method, globals::engine, args...);
}

// As CallOr, for a method returning nothing.
template <class F, class... Args> void Call(std::string_view key, F method, Args... args)
{
    if (const auto index = SlotIndexOf(key, method)) CallAtSlot(*index, method, globals::engine, args...);
}

} // namespace

void ServerCommand(const char* command) { Call("IVEngineServer2::ServerCommand", &IVEngineServer2::ServerCommand, command); }

bool IsMapValid(const char* mapName) { return CallOr("IVEngineServer2::IsMapValid", &IVEngineServer2::IsMapValid, 0, mapName) != 0; }

void PrecacheGeneric(const char* name) { Call("IVEngineServer2::PrecacheGeneric", &IVEngineServer2::PrecacheGeneric, name, false); }

int GetPlayerUserId(CPlayerSlot slot)
{
    return CallOr("IVEngineServer2::GetPlayerUserId", &IVEngineServer2::GetPlayerUserId, CPlayerUserId(-1), slot).Get();
}

void ClientPrintf(CPlayerSlot slot, const char* message) { Call("IVEngineServer2::ClientPrintf", &IVEngineServer2::ClientPrintf, slot, message); }

const char* GetClientConVarValue(CPlayerSlot slot, const char* name)
{
    return CallOr("IVEngineServer2::GetClientConVarValue", &IVEngineServer2::GetClientConVarValue, static_cast<const char*>(nullptr), slot,
                name);
}

void SetFakeClientConVarValue(CPlayerSlot slot, const char* name, const char* value)
{
    Call("IVEngineServer2::SetFakeClientConVarValue", &IVEngineServer2::SetFakeClientConVarValue, slot, name, value);
}

// As before: the command is the format string.
void ClientCommand(CPlayerSlot slot, const char* command) { Call("IVEngineServer2::ClientCommand", &IVEngineServer2::ClientCommand, slot, command); }

void DisconnectClient(CPlayerSlot slot, ENetworkDisconnectionReason reason)
{
    Call("IVEngineServer2::DisconnectClient", &IVEngineServer2::DisconnectClient, slot, reason, static_cast<const char*>(nullptr));
}

bool IsClientFullyAuthenticated(CPlayerSlot slot)
{
    return CallOr("IVEngineServer2::IsClientFullyAuthenticated", &IVEngineServer2::IsClientFullyAuthenticated, false, slot);
}

const CSteamID* GetClientSteamID(CPlayerSlot slot)
{
    return CallOr("IVEngineServer2::GetClientSteamID", &IVEngineServer2::GetClientSteamID, static_cast<const CSteamID*>(nullptr), slot);
}

} // namespace counterstrikesharp::engine_server
