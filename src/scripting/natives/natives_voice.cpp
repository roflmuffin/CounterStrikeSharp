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

#include <public/entity2/entitysystem.h>

#include "core/managers/player_manager.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

void SetClientListening(ScriptContext& scriptContext)
{
    auto receiver = scriptContext.GetArgument<CEntityInstance*>(0);
    auto sender = scriptContext.GetArgument<CEntityInstance*>(1);
    auto listen = scriptContext.GetArgument<ListenOverride>(2);

    if (!receiver)
    {
        scriptContext.ThrowNativeError("Receiver is a null pointer");
        return;
    }

    if (!sender)
    {
        scriptContext.ThrowNativeError("Sender is a null pointer");
        return;
    }

    auto iSenderSlot = sender->GetEntityIndex().Get() - 1;

    if (iSenderSlot < 0 || iSenderSlot >= globals::getGlobalVars()->maxClients) scriptContext.ThrowNativeError("Invalid sender");

    auto pPlayer = globals::playerManager.GetPlayerBySlot(receiver->GetEntityIndex().Get() - 1);

    if (pPlayer == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid receiver");
        return;
    }

    pPlayer->SetListen(iSenderSlot, listen);
}

ListenOverride GetClientListening(ScriptContext& scriptContext)
{
    auto receiver = scriptContext.GetArgument<CEntityInstance*>(0);
    auto sender = scriptContext.GetArgument<CEntityInstance*>(1);

    if (!receiver)
    {
        scriptContext.ThrowNativeError("Receiver is a null pointer");
        return Listen_Default;
    }

    if (!sender)
    {
        scriptContext.ThrowNativeError("Sender is a null pointer");
        return Listen_Default;
    }

    auto iSenderSlot = sender->GetEntityIndex().Get() - 1;

    if (iSenderSlot < 0 || iSenderSlot >= globals::getGlobalVars()->maxClients) scriptContext.ThrowNativeError("Invalid sender");

    auto pPlayer = globals::playerManager.GetPlayerBySlot(receiver->GetEntityIndex().Get() - 1);

    if (pPlayer == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid receiver");
        return Listen_Default;
    }

    return pPlayer->GetListen(iSenderSlot);
}

void SetClientVoiceFlags(ScriptContext& scriptContext)
{
    auto client = scriptContext.GetArgument<CEntityInstance*>(0);
    auto flags = scriptContext.GetArgument<VoiceFlag_t>(1);

    if (!client)
    {
        scriptContext.ThrowNativeError("Receiver is a null pointer");
        return;
    }
    auto pPlayer = globals::playerManager.GetPlayerBySlot(client->GetEntityIndex().Get() - 1);

    if (pPlayer == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid receiver");
        return;
    }

    pPlayer->SetVoiceFlags(flags);
}

VoiceFlag_t GetClientVoiceFlags(ScriptContext& scriptContext)
{
    auto client = scriptContext.GetArgument<CEntityInstance*>(0);

    if (!client)
    {
        scriptContext.ThrowNativeError("Receiver is a null pointer");
        return VoiceFlag_t{};
    }

    auto pPlayer = globals::playerManager.GetPlayerBySlot(client->GetEntityIndex().Get() - 1);

    if (pPlayer == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid receiver");
    }

    return pPlayer->GetVoiceFlags();
}

REGISTER_NATIVES(voice, {
    ScriptEngine::RegisterNativeHandler("SET_CLIENT_LISTENING", SetClientListening);
    ScriptEngine::RegisterNativeHandler("GET_CLIENT_LISTENING", GetClientListening);
    ScriptEngine::RegisterNativeHandler("SET_CLIENT_VOICE_FLAGS", SetClientVoiceFlags);
    ScriptEngine::RegisterNativeHandler("GET_CLIENT_VOICE_FLAGS", GetClientVoiceFlags);
})
} // namespace counterstrikesharp
