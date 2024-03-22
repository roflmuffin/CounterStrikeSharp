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

#include <IEngineSound.h>
#include <edict.h>
#include <eiface.h>
#include <filesystem.h>
#include <public/worldsize.h>

#include "entitykeyvalues.h"

// clang-format off
#include "mm_plugin.h"
#include "core/engine_trace.h"
#include "core/timer_system.h"
#include "core/utils.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"
#include "core/function.h"
#include "core/managers/player_manager.h"
#include "core/managers/server_manager.h"
#include "core/tick_scheduler.h"
// clang-format on

#if _WIN32
#undef GetCurrentTime
#endif

namespace counterstrikesharp {

const char* GetMapName(ScriptContext& script_context)
{
    if (globals::getGlobalVars() == nullptr)
        return nullptr;

    return globals::getGlobalVars()->mapname.ToCStr();
}

const char* GetGameDirectory(ScriptContext& script_context)
{
    return strdup(Plat_GetGameDirectory());
}

bool IsMapValid(ScriptContext& script_context)
{
    auto mapname = script_context.GetArgument<const char*>(0);
    return globals::engine->IsMapValid(mapname) != 0;
}

float GetTickInterval(ScriptContext& script_context)
{
    return globals::engine_fixed_tick_interval;
}

float GetCurrentTime(ScriptContext& script_context) { return globals::getGlobalVars()->curtime; }

int GetTickCount(ScriptContext& script_context) { return globals::getGlobalVars()->tickcount; }

float GetGameFrameTime(ScriptContext& script_context)
{
    return globals::getGlobalVars()->frametime;
}

double GetEngineTime(ScriptContext& script_context) { return Plat_FloatTime(); }

int GetMaxClients(ScriptContext& script_context)
{
    auto globalVars = globals::getGlobalVars();
    if (globalVars == nullptr) {
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

Ray_t* CreateRay1(ScriptContext& script_context)
{
    auto ray_type = script_context.GetArgument<RayType>(0);
    auto vec1 = script_context.GetArgument<Vector*>(1);
    auto vec2 = script_context.GetArgument<Vector*>(2);

    Ray_t* pRay = new Ray_t;

    if (ray_type == RayType_EndPoint) {
        pRay->Init(*vec1, *vec2);
        return pRay;
    } else if (ray_type == RayType_Infinite) {
        QAngle angles;
        Vector endVec;
        angles.Init(vec2->x, vec2->y, vec2->z);
        AngleVectors(angles, &endVec);

        endVec.NormalizeInPlace();
        endVec = *vec1 + endVec * MAX_TRACE_LENGTH;

        pRay->Init(*vec1, endVec);
        return pRay;
    }

    return nullptr;
}

Ray_t* CreateRay2(ScriptContext& script_context)
{
    auto vec1 = script_context.GetArgument<Vector*>(0);
    auto vec2 = script_context.GetArgument<Vector*>(1);
    auto vec3 = script_context.GetArgument<Vector*>(2);
    auto vec4 = script_context.GetArgument<Vector*>(3);

    Ray_t* pRay = new Ray_t;
    pRay->Init(*vec1, *vec2, *vec3, *vec4);
    return pRay;
}

void TraceRay(ScriptContext& script_context)
{
    auto ray = script_context.GetArgument<Ray_t*>(0);
    auto pTrace = script_context.GetArgument<CGameTrace*>(1);
    auto trace_filter = script_context.GetArgument<ITraceFilter*>(2);
    auto flags = script_context.GetArgument<uint32_t>(3);

    globals::engineTrace->TraceRay(*ray, flags, trace_filter, pTrace);
}

CSimpleTraceFilter* NewSimpleTraceFilter(ScriptContext& script_context)
{
    auto index_to_ignore = script_context.GetArgument<int>(0);

    return new CSimpleTraceFilter(index_to_ignore);
}

TraceFilterProxy* NewTraceFilterProxy(ScriptContext& script_context)
{
    return new TraceFilterProxy();
}

void TraceFilterProxySetTraceTypeCallback(ScriptContext& script_context)
{
    auto trace_filter = script_context.GetArgument<TraceFilterProxy*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);

    trace_filter->SetGetTraceTypeCallback(callback);
}

void TraceFilterProxySetShouldHitEntityCallback(ScriptContext& script_context)
{
    auto [trace_filter, callback] = script_context.GetArguments<TraceFilterProxy*, CallbackT>();
    trace_filter->SetShouldHitEntityCallback(callback);
}

CGameTrace* NewTraceResult(ScriptContext& script_context) { return new CGameTrace(); }

double GetTickedTime(ScriptContext& script_context) { return globals::timerSystem.GetTickedTime(); }

void QueueTaskForNextFrame(ScriptContext& script_context)
{
    auto func = script_context.GetArgument<void*>(0);

    typedef void(voidfunc)(void);
    globals::mmPlugin->AddTaskForNextFrame([func]() { reinterpret_cast<voidfunc*>(func)(); });
}

void QueueTaskForNextWorldUpdate(ScriptContext& script_context)
{
    auto func = script_context.GetArgument<void*>(0);

    typedef void(voidfunc)(void);
    globals::serverManager.AddTaskForNextWorldUpdate([func]() { reinterpret_cast<voidfunc*>(func)(); });
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
    if (interfaceType == Server) {
        factoryFn = globals::ismm->GetServerFactory();
    } else if (interfaceType == Engine) {
        factoryFn = globals::ismm->GetEngineFactory();
    }

    auto foundInterface = globals::ismm->VInterfaceMatch(factoryFn, interfaceName);

    if (foundInterface == nullptr) {
        scriptContext.ThrowNativeError("Could not find interface");
    }

    return foundInterface;
}

void GetCommandParamValue(ScriptContext& scriptContext)
{
    auto paramName = scriptContext.GetArgument<const char*>(0);
    auto paramType = scriptContext.GetArgument<DataType_t>(1);

    int iContextIndex = 2;
    switch (paramType) {
    case DATA_TYPE_STRING:
        scriptContext.SetResult(CommandLine()->ParmValue(
            paramName, scriptContext.GetArgument<const char*>(iContextIndex)));
        return;
    case DATA_TYPE_INT:
        scriptContext.SetResult(
            CommandLine()->ParmValue(paramName, scriptContext.GetArgument<int>(iContextIndex)));
        return;
    case DATA_TYPE_FLOAT:
        scriptContext.SetResult(
            CommandLine()->ParmValue(paramName, scriptContext.GetArgument<float>(iContextIndex)));
        return;
    }

    scriptContext.ThrowNativeError("Invalid param type");
}

void PrintToServerConsole(ScriptContext& scriptContext) {
    auto message = scriptContext.GetArgument<const char*>(0);

    META_CONPRINT(message);
}

enum KeyValuesType_t : unsigned int
{
    TYPE_BOOL,
    TYPE_INT,
    TYPE_UINT,
    TYPE_INT64,
    TYPE_UINT64,
    TYPE_FLOAT,
    TYPE_DOUBLE,
    TYPE_STRING,
    TYPE_POINTER,
    TYPE_STRING_TOKEN,
    TYPE_EHANDLE,
    TYPE_COLOR,
    TYPE_VECTOR,
    TYPE_VECTOR2D,
    TYPE_VECTOR4D,
    TYPE_QUATERNION,
    TYPE_QANGLE,
    TYPE_MATRIX3X4
};

CEntityKeyValues* CreateEntityKeyValues(ScriptContext& scriptContext)
{
    auto count = scriptContext.GetArgument<int>(0);
    if (count == 0) {
        return nullptr;
    }

    CEntityKeyValues* pKeyValues = new CEntityKeyValues();
    pKeyValues->EnableLogging(true);

    int offset = 1;
    for (int i = 0; i < count; ++i) {
        const char* key = scriptContext.GetArgument<const char*>(offset);
        KeyValuesType_t _type = scriptContext.GetArgument<KeyValuesType_t>(offset + 1);
        switch (_type) {
        case counterstrikesharp::TYPE_BOOL:
            pKeyValues->SetBool(key, scriptContext.GetArgument<bool>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_INT:
            pKeyValues->SetInt(key, scriptContext.GetArgument<int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_UINT:
            pKeyValues->SetUint(key, scriptContext.GetArgument<unsigned int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_INT64:
            pKeyValues->SetInt64(key, scriptContext.GetArgument<long>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_UINT64:
            pKeyValues->SetUint64(key, scriptContext.GetArgument<unsigned long>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_FLOAT:
            pKeyValues->SetFloat(key, scriptContext.GetArgument<float>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_DOUBLE:
            pKeyValues->SetDouble(key, scriptContext.GetArgument<double>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_STRING:
            pKeyValues->SetString(key, scriptContext.GetArgument<const char*>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_POINTER:
            pKeyValues->SetPtr(key, scriptContext.GetArgument<void*>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_STRING_TOKEN:
            pKeyValues->SetStringToken(key, scriptContext.GetArgument<unsigned int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_EHANDLE:
            pKeyValues->SetEHandle(key, scriptContext.GetArgument<unsigned int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_COLOR: {
            char r = scriptContext.GetArgument<char>(offset + 2);
            char g = scriptContext.GetArgument<char>(offset + 3);
            char b = scriptContext.GetArgument<char>(offset + 4);
            char a = scriptContext.GetArgument<char>(offset + 5);

            pKeyValues->SetColor(key, Color(r, g, b, a));
            offset += 6;
            break;
        }
        case counterstrikesharp::TYPE_VECTOR: {
            float x = scriptContext.GetArgument<float>(offset + 2);
            float y = scriptContext.GetArgument<float>(offset + 3);
            float z = scriptContext.GetArgument<float>(offset + 4);

            pKeyValues->SetVector(key, Vector(x, y, z));
            offset += 5;
            break;
        }
        case counterstrikesharp::TYPE_VECTOR2D: {
            float x = scriptContext.GetArgument<float>(offset + 2);
            float y = scriptContext.GetArgument<float>(offset + 3);

            pKeyValues->SetVector2D(key, Vector2D(x, y));
            offset += 4;
            break;
        }
        case counterstrikesharp::TYPE_VECTOR4D: {
            float x = scriptContext.GetArgument<float>(offset + 2);
            float y = scriptContext.GetArgument<float>(offset + 3);
            float z = scriptContext.GetArgument<float>(offset + 4);
            float w = scriptContext.GetArgument<float>(offset + 5);

            pKeyValues->SetVector4D(key, Vector4D(x, y, z, w));
            offset += 6;
            break;
        }
        case counterstrikesharp::TYPE_QUATERNION: {
            float x = scriptContext.GetArgument<float>(offset + 2);
            float y = scriptContext.GetArgument<float>(offset + 3);
            float z = scriptContext.GetArgument<float>(offset + 4);
            float w = scriptContext.GetArgument<float>(offset + 5);

            pKeyValues->SetQuaternion(key, Quaternion(x, y, z, w));
            offset += 6;
            break;
        }
        case counterstrikesharp::TYPE_QANGLE: {
            float x = scriptContext.GetArgument<float>(offset + 2);
            float y = scriptContext.GetArgument<float>(offset + 3);
            float z = scriptContext.GetArgument<float>(offset + 4);

            pKeyValues->SetQAngle(key, QAngle(x, y, z));
            offset += 5;
            break;
        }
        case counterstrikesharp::TYPE_MATRIX3X4: {
            float m11 = scriptContext.GetArgument<float>(offset + 2);
            float m12 = scriptContext.GetArgument<float>(offset + 3);
            float m13 = scriptContext.GetArgument<float>(offset + 4);
            float m14 = scriptContext.GetArgument<float>(offset + 5);

            float m21 = scriptContext.GetArgument<float>(offset + 6);
            float m22 = scriptContext.GetArgument<float>(offset + 7);
            float m23 = scriptContext.GetArgument<float>(offset + 8);
            float m24 = scriptContext.GetArgument<float>(offset + 9);

            float m31 = scriptContext.GetArgument<float>(offset + 10);
            float m32 = scriptContext.GetArgument<float>(offset + 11);
            float m33 = scriptContext.GetArgument<float>(offset + 12);
            float m34 = scriptContext.GetArgument<float>(offset + 13);

            pKeyValues->SetMatrix3x4(
                key, matrix3x4_t(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34));
            offset += 14;
            break;
        }
        default:
            break;
        }
    }

    return pKeyValues;
}

CREATE_GETTER_FUNCTION(Trace, bool, DidHit, CGameTrace*, obj->DidHit());
CREATE_GETTER_FUNCTION(TraceResult, CBaseEntity*, Entity, CGameTrace*, obj->m_pEnt);

REGISTER_NATIVES(engine, {
    ScriptEngine::RegisterNativeHandler("GET_GAME_DIRECTORY", GetGameDirectory);
    ScriptEngine::RegisterNativeHandler("GET_MAP_NAME", GetMapName);
    ScriptEngine::RegisterNativeHandler("IS_MAP_VALID", IsMapValid);
    ScriptEngine::RegisterNativeHandler("GET_TICK_INTERVAL", GetTickInterval);
    ScriptEngine::RegisterNativeHandler("GET_TICK_COUNT", GetTickCount);
    ScriptEngine::RegisterNativeHandler("GET_CURRENT_TIME", GetCurrentTime);
    ScriptEngine::RegisterNativeHandler("GET_GAMEFRAME_TIME", GetGameFrameTime);
    ScriptEngine::RegisterNativeHandler("GET_ENGINE_TIME", GetEngineTime);
    ScriptEngine::RegisterNativeHandler("GET_MAX_CLIENTS", GetMaxClients);
    ScriptEngine::RegisterNativeHandler("ISSUE_SERVER_COMMAND", ServerCommand);
    ScriptEngine::RegisterNativeHandler("PRECACHE_MODEL", PrecacheModel);
    ScriptEngine::RegisterNativeHandler("PRECACHE_SOUND", PrecacheSound);
    ScriptEngine::RegisterNativeHandler("IS_SOUND_PRECACHED", IsSoundPrecached);
    ScriptEngine::RegisterNativeHandler("GET_SOUND_DURATION", GetSoundDuration);
    // ScriptEngine::RegisterNativeHandler("EMIT_SOUND", EmitSound);

    ScriptEngine::RegisterNativeHandler("NEW_SIMPLE_TRACE_FILTER", NewSimpleTraceFilter);
    ScriptEngine::RegisterNativeHandler("NEW_TRACE_RESULT", NewTraceResult);
    ScriptEngine::RegisterNativeHandler("TRACE_DID_HIT", TraceGetDidHit);
    ScriptEngine::RegisterNativeHandler("TRACE_RESULT_ENTITY", TraceResultGetEntity);

    ScriptEngine::RegisterNativeHandler("NEW_TRACE_FILTER_PROXY", NewTraceFilterProxy);
    ScriptEngine::RegisterNativeHandler("TRACE_FILTER_PROXY_SET_TRACE_TYPE_CALLBACK",
                                        TraceFilterProxySetTraceTypeCallback);
    ScriptEngine::RegisterNativeHandler("TRACE_FILTER_PROXY_SET_SHOULD_HIT_ENTITY_CALLBACK",
                                        TraceFilterProxySetShouldHitEntityCallback);

    ScriptEngine::RegisterNativeHandler("CREATE_RAY_1", CreateRay1);
    ScriptEngine::RegisterNativeHandler("CREATE_RAY_2", CreateRay2);
    ScriptEngine::RegisterNativeHandler("TRACE_RAY", TraceRay);
    ScriptEngine::RegisterNativeHandler("GET_TICKED_TIME", GetTickedTime);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_NEXT_FRAME", QueueTaskForNextFrame);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_NEXT_WORLD_UPDATE", QueueTaskForNextWorldUpdate);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_FRAME", QueueTaskForFrame);
    ScriptEngine::RegisterNativeHandler("GET_VALVE_INTERFACE", GetValveInterface);
    ScriptEngine::RegisterNativeHandler("GET_COMMAND_PARAM_VALUE", GetCommandParamValue);
    ScriptEngine::RegisterNativeHandler("PRINT_TO_SERVER_CONSOLE", PrintToServerConsole);
    ScriptEngine::RegisterNativeHandler("CREATE_ENTITY_KEYVALUES", CreateEntityKeyValues);
})
} // namespace counterstrikesharp
