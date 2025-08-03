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

#include "igameeventsystem.h"

#include <IEngineSound.h>
#include <edict.h>
#include <eiface.h>
#include <filesystem.h>
#include <public/worldsize.h>

#include "mm_plugin.h"
#include "core/timer_system.h"
#include "core/utils.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"
#include "core/function.h"
#include "core/recipientfilters.h"
#include "core/managers/player_manager.h"
#include "core/managers/server_manager.h"
#include "core/tick_scheduler.h"
#include "networksystem/inetworkmessages.h"
#include "usermessages.pb.h"

#if _WIN32
#undef GetCurrentTime
#endif

namespace counterstrikesharp {

const char* GetMapName(ScriptContext& script_context)
{
    if (globals::getGlobalVars() == nullptr) return nullptr;

    return globals::getGlobalVars()->mapname.ToCStr();
}

const char* GetGameDirectory(ScriptContext& script_context) { return strdup(Plat_GetGameDirectory()); }

bool IsMapValid(ScriptContext& script_context)
{
    auto mapname = script_context.GetArgument<const char*>(0);
    return globals::engine->IsMapValid(mapname) != 0;
}

float GetTickInterval(ScriptContext& script_context) { return globals::engine_fixed_tick_interval; }

float GetCurrentTime(ScriptContext& script_context) { return globals::getGlobalVars()->curtime; }

int GetTickCount(ScriptContext& script_context) { return globals::getGlobalVars()->tickcount; }

float GetGameFrameTime(ScriptContext& script_context) { return 0; }

double GetEngineTime(ScriptContext& script_context) { return Plat_FloatTime(); }

int GetMaxClients(ScriptContext& script_context)
{
    auto globalVars = globals::getGlobalVars();
    if (globalVars == nullptr)
    {
        script_context.ThrowNativeError("Global Variables not initialized yet.");
        return -1;
    }

    return globalVars->maxClients;
}

void ServerCommand(ScriptContext& script_context)
{
    auto command = script_context.GetArgument<const char*>(0);

    auto clean_command = std::string(command);
    clean_command.append("\n\0");
    globals::engine->ServerCommand(clean_command.c_str());
}

void PrecacheModel(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    globals::engine->PrecacheGeneric(name);
}

bool PrecacheSound(ScriptContext& script_context)
{
    auto [name, preload] = script_context.GetArguments<const char*, bool>();

    return globals::engineSound->PrecacheSound(name, preload);
}

bool IsSoundPrecached(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);

    return globals::engineSound->IsSoundPrecached(name);
}

float GetSoundDuration(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);

    return globals::engineSound->GetSoundDuration(name);
}

// void EmitSound(ScriptContext& script_context)
//{
//    auto client = script_context.GetArgument<int>(0);
//    auto entitySource = script_context.GetArgument<int>(1);
//    auto channel = script_context.GetArgument<int>(2);
//    auto sound = script_context.GetArgument<const char*>(3);
//    auto volume = script_context.GetArgument<float>(4);
//    auto attenuation = script_context.GetArgument <float>(5);
//    auto flags = script_context.GetArgument<int>(6);
//    auto pitch = script_context.GetArgument<int>(7);
//    auto origin = script_context.GetArgument<Vector*>(8);
//    auto direction = script_context.GetArgument<Vector*>(9);
//
//    auto recipients = new CustomRecipientFilter();
//    recipients->AddPlayer(client);
//
//    globals::engineSound->EmitSound(static_cast<IRecipientFilter&>(*recipients),
//                                     entitySource,channel, sound, -1, sound, volume,
//                                     attenuation, 0, flags, pitch, origin, direction);
// }

double GetTickedTime(ScriptContext& script_context) { return globals::timerSystem.GetTickedTime(); }

void QueueTaskForNextFrame(ScriptContext& script_context)
{
    auto func = script_context.GetArgument<void*>(0);

    typedef void(voidfunc)(void);
    globals::mmPlugin->AddTaskForNextFrame([func]() {
        reinterpret_cast<voidfunc*>(func)();
    });
}

void QueueTaskForNextWorldUpdate(ScriptContext& script_context)
{
    auto func = script_context.GetArgument<void*>(0);

    typedef void(voidfunc)(void);
    globals::serverManager.AddTaskForNextWorldUpdate([func]() {
        reinterpret_cast<voidfunc*>(func)();
    });
}

void QueueTaskForFrame(ScriptContext& script_context)
{
    auto tick = script_context.GetArgument<int>(0);
    auto func = script_context.GetArgument<void*>(1);

    typedef void(voidfunc)(void);
    globals::tickScheduler.schedule(tick, reinterpret_cast<voidfunc*>(func));
}

enum InterfaceType
{
    Engine,
    Server
};

void* GetValveInterface(ScriptContext& scriptContext)
{
    auto [interfaceType, interfaceName] = scriptContext.GetArguments<InterfaceType, const char*>();

    CreateInterfaceFn factoryFn;
    if (interfaceType == Server)
    {
        factoryFn = globals::ismm->GetServerFactory();
    }
    else if (interfaceType == Engine)
    {
        factoryFn = globals::ismm->GetEngineFactory();
    }

    auto foundInterface = globals::ismm->VInterfaceMatch(factoryFn, interfaceName);

    if (foundInterface == nullptr)
    {
        scriptContext.ThrowNativeError("Could not find interface");
    }

    return foundInterface;
}

void GetCommandParamValue(ScriptContext& scriptContext)
{
    auto paramName = scriptContext.GetArgument<const char*>(0);
    auto paramType = scriptContext.GetArgument<DataType_t>(1);

    int iContextIndex = 2;
    switch (paramType)
    {
        case DATA_TYPE_STRING:
            scriptContext.SetResult(CommandLine()->ParmValue(paramName, scriptContext.GetArgument<const char*>(iContextIndex)));
            return;
        case DATA_TYPE_INT:
            scriptContext.SetResult(CommandLine()->ParmValue(paramName, scriptContext.GetArgument<int>(iContextIndex)));
            return;
        case DATA_TYPE_FLOAT:
            scriptContext.SetResult(CommandLine()->ParmValue(paramName, scriptContext.GetArgument<float>(iContextIndex)));
            return;
    }

    scriptContext.ThrowNativeError("Invalid param type");
}

void PrintToServerConsole(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<const char*>(0);

    META_CONPRINT(message);
}

void DisconnectClient(ScriptContext& scriptContext)
{
    auto slot = scriptContext.GetArgument<int>(0);
    auto disconnectReason = scriptContext.GetArgument<ENetworkDisconnectionReason>(1);

    if (!ENetworkDisconnectionReason_IsValid(disconnectReason))
    {
        scriptContext.ThrowNativeError("Invalid disconnect reason");
        return;
    }

    globals::engineServer2->DisconnectClient(slot, disconnectReason);
}

void ClientPrint(ScriptContext& scriptContext)
{
    auto slot = scriptContext.GetArgument<int>(0);
    auto hudDestination = scriptContext.GetArgument<int>(1);
    auto message = scriptContext.GetArgument<const char*>(2);

    INetworkMessageInternal* pNetMsg = globals::networkMessages->FindNetworkMessagePartial("TextMsg");
    auto data = pNetMsg->AllocateMessage()->ToPB<CUserMessageTextMsg>();

    data->set_dest(hudDestination);
    data->add_param(message);

    CPlayerBitVec recipients;
    recipients.Set(slot);

    globals::gameEventSystem->PostEventAbstract(CSplitScreenSlot(-1), false, ABSOLUTE_PLAYER_LIMIT,
                                                reinterpret_cast<const uint64*>(recipients.Base()), pNetMsg, data, 0,
                                                NetChannelBufType_t::BUF_RELIABLE);

    delete data;
}

REGISTER_NATIVES(engine, {
    ScriptEngine::RegisterNativeHandler("GET_GAME_DIRECTORY", GetGameDirectory);
    ScriptEngine::RegisterNativeHandler("GET_MAP_NAME", GetMapName);
    ScriptEngine::RegisterNativeHandler("IS_MAP_VALID", IsMapValid);
    ScriptEngine::RegisterNativeHandler("GET_TICK_INTERVAL", GetTickInterval);
    ScriptEngine::RegisterNativeHandler("GET_TICK_COUNT", GetTickCount);
    ScriptEngine::RegisterNativeHandler("GET_CURRENT_TIME", GetCurrentTime);
    ScriptEngine::RegisterNativeHandler("GET_GAME_FRAME_TIME", GetGameFrameTime);
    ScriptEngine::RegisterNativeHandler("GET_ENGINE_TIME", GetEngineTime);
    ScriptEngine::RegisterNativeHandler("GET_MAX_CLIENTS", GetMaxClients);
    ScriptEngine::RegisterNativeHandler("ISSUE_SERVER_COMMAND", ServerCommand);
    ScriptEngine::RegisterNativeHandler("PRECACHE_MODEL", PrecacheModel);
    ScriptEngine::RegisterNativeHandler("PRECACHE_SOUND", PrecacheSound);
    ScriptEngine::RegisterNativeHandler("IS_SOUND_PRECACHED", IsSoundPrecached);
    ScriptEngine::RegisterNativeHandler("GET_SOUND_DURATION", GetSoundDuration);
    // ScriptEngine::RegisterNativeHandler("EMIT_SOUND", EmitSound);

    ScriptEngine::RegisterNativeHandler("GET_TICKED_TIME", GetTickedTime);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_NEXT_FRAME", QueueTaskForNextFrame);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_NEXT_WORLD_UPDATE", QueueTaskForNextWorldUpdate);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_FRAME", QueueTaskForFrame);
    ScriptEngine::RegisterNativeHandler("GET_VALVE_INTERFACE", GetValveInterface);
    ScriptEngine::RegisterNativeHandler("GET_COMMAND_PARAM_VALUE", GetCommandParamValue);
    ScriptEngine::RegisterNativeHandler("PRINT_TO_SERVER_CONSOLE", PrintToServerConsole);
    ScriptEngine::RegisterNativeHandler("DISCONNECT_CLIENT", DisconnectClient);
    ScriptEngine::RegisterNativeHandler("CLIENT_PRINT", ClientPrint);
})
} // namespace counterstrikesharp
