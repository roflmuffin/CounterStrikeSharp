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
#include "entitykeyvalues.h"

#include <ios>
#include <sstream>

#include "core/log.h"
#include "core/managers/entity_manager.h"
#include "core/managers/player_manager.h"
#include "core/memory.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {
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

CEntityInstance* GetEntityFromIndex(ScriptContext& script_context)
{
    if (!globals::entitySystem)
    {
        script_context.ThrowNativeError("Entity system is not yet initialized");
        return nullptr;
    }

    auto entityIndex = script_context.GetArgument<int>(0);

    return globals::entitySystem->GetEntityInstance(CEntityIndex(entityIndex));
}

int GetUserIdFromIndex(ScriptContext& scriptContext)
{
    auto entityIndex = scriptContext.GetArgument<int>(0);

    // CPlayerSlot is 1 less than index
    return globals::engine->GetPlayerUserId(CPlayerSlot(entityIndex - 1)).Get();
}

const char* GetDesignerName(ScriptContext& scriptContext)
{
    auto entity = scriptContext.GetArgument<CEntityInstance*>(0);
    return entity->GetClassname();
}

void* GetEntityPointerFromHandle(ScriptContext& scriptContext)
{
    if (!globals::entitySystem)
    {
        scriptContext.ThrowNativeError("Entity system is not yet initialized");
        return nullptr;
    }

    auto handle = scriptContext.GetArgument<CEntityHandle*>(0);

    if (!handle->IsValid())
    {
        return nullptr;
    }

    return globals::entitySystem->GetEntityInstance(*handle);
}

void* GetEntityPointerFromRef(ScriptContext& scriptContext)
{
    if (!globals::entitySystem)
    {
        scriptContext.ThrowNativeError("Entity system yet is not initialized");
        return nullptr;
    }

    auto ref = scriptContext.GetArgument<unsigned int>(0);

    if (ref == INVALID_EHANDLE_INDEX)
    {
        return nullptr;
    }

    CBaseHandle hndl(ref);

    return globals::entitySystem->GetEntityInstance(hndl);
}

unsigned int GetRefFromEntityPointer(ScriptContext& scriptContext)
{
    auto* pEntity = scriptContext.GetArgument<CEntityInstance*>(0);

    if (pEntity == nullptr)
    {
        return INVALID_EHANDLE_INDEX;
    }

    auto hndl = pEntity->GetRefEHandle();

    if (hndl == INVALID_EHANDLE_INDEX)
    {
        return INVALID_EHANDLE_INDEX;
    }

    return hndl.ToInt();
}

bool IsRefValidEntity(ScriptContext& scriptContext)
{
    if (!globals::entitySystem)
    {
        scriptContext.ThrowNativeError("Entity system yet is not initialized");
        return false;
    }

    auto ref = scriptContext.GetArgument<unsigned int>(0);

    if (ref == INVALID_EHANDLE_INDEX)
    {
        return false;
    }

    CBaseHandle hndl(ref);

    if (!hndl.IsValid())
    {
        return false;
    }

    return globals::entitySystem->GetEntityInstance(hndl) != nullptr;
}

void PrintToConsole(ScriptContext& scriptContext)
{
    auto index = scriptContext.GetArgument<int>(0);
    auto message = scriptContext.GetArgument<const char*>(1);

    globals::engine->ClientPrintf(CPlayerSlot{ index - 1 }, message);
}

CEntityIdentity* GetFirstActiveEntity(ScriptContext& script_context)
{
    if (!globals::entitySystem)
    {
        script_context.ThrowNativeError("Entity system yet is not initialized");
        return nullptr;
    }

    return globals::entitySystem->m_EntityList.m_pFirstActiveEntity;
}

void* GetConcreteEntityListPointer(ScriptContext& script_context)
{
    if (!globals::entitySystem)
    {
        script_context.ThrowNativeError("Entity system yet is not initialized");
        return nullptr;
    }

    return &globals::entitySystem->m_EntityList;
}

unsigned long GetPlayerAuthorizedSteamID(ScriptContext& script_context)
{
    auto iSlot = script_context.GetArgument<int>(0);

    auto pPlayer = globals::playerManager.GetPlayerBySlot(iSlot);
    if (pPlayer == nullptr || !pPlayer->m_is_authorized)
    {
        return -1;
    }

    auto pSteamId = pPlayer->GetSteamId();
    if (pSteamId == nullptr)
    {
        return -1;
    }

    return pSteamId->ConvertToUint64();
}

const char* GetPlayerIpAddress(ScriptContext& script_context)
{
    auto iSlot = script_context.GetArgument<int>(0);

    auto pPlayer = globals::playerManager.GetPlayerBySlot(iSlot);
    if (pPlayer == nullptr)
    {
        return nullptr;
    }

    return pPlayer->GetIpAddress();
}

void HookEntityOutput(ScriptContext& script_context)
{
    auto szClassname = script_context.GetArgument<const char*>(0);
    auto szOutput = script_context.GetArgument<const char*>(1);
    auto callback = script_context.GetArgument<CallbackT>(2);
    auto mode = script_context.GetArgument<HookMode>(3);
    globals::entityManager.HookEntityOutput(szClassname, szOutput, callback, mode);
}

void UnhookEntityOutput(ScriptContext& script_context)
{
    auto szClassname = script_context.GetArgument<const char*>(0);
    auto szOutput = script_context.GetArgument<const char*>(1);
    auto callback = script_context.GetArgument<CallbackT>(2);
    auto mode = script_context.GetArgument<HookMode>(3);
    globals::entityManager.UnhookEntityOutput(szClassname, szOutput, callback, mode);
}

void AcceptInput(ScriptContext& script_context)
{
    if (!CEntityInstance_AcceptInput)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CEntityInstance_AcceptInput\'");
        return;
    }

    CEntityInstance* pThis = script_context.GetArgument<CEntityInstance*>(0);
    const char* pInputName = script_context.GetArgument<const char*>(1);
    CEntityInstance* pActivator = script_context.GetArgument<CEntityInstance*>(2);
    CEntityInstance* pCaller = script_context.GetArgument<CEntityInstance*>(3);
    const char* value = script_context.GetArgument<const char*>(4);
    int outputID = script_context.GetArgument<int>(5);

    variant_t _value = variant_t(value);
    CEntityInstance_AcceptInput(pThis, pInputName, pActivator, pCaller, &_value, outputID);
}

void AddEntityIOEvent(ScriptContext& script_context)
{
    if (!CEntitySystem_AddEntityIOEvent)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CEntitySystem_AddEntityIOEvent\'");
        return;
    }

    CEntityInstance* pTarget = script_context.GetArgument<CEntityInstance*>(0);
    const char* pInputName = script_context.GetArgument<const char*>(1);
    CEntityInstance* pActivator = script_context.GetArgument<CEntityInstance*>(2);
    CEntityInstance* pCaller = script_context.GetArgument<CEntityInstance*>(3);
    const char* value = script_context.GetArgument<const char*>(4);
    float delay = script_context.GetArgument<float>(5);
    int outputID = script_context.GetArgument<int>(6);

    variant_t _value = variant_t(value);
    CEntitySystem_AddEntityIOEvent(GameEntitySystem(), pTarget, pInputName, pActivator, pCaller, &_value, delay, outputID);
}

void DispatchSpawn(ScriptContext& scriptContext)
{
    auto entity = scriptContext.GetArgument<void*>(0);
    auto keyValues = scriptContext.GetArgument<CEntityKeyValues*>(1);
    CBaseEntity_DispatchSpawn(entity, keyValues);
}

CEntityKeyValues* EntityKeyValuesNew(ScriptContext& script_context)
{
    return new CEntityKeyValues();
}

bool EntityKeyValuesHasValue(ScriptContext& script_context)
{
    CEntityKeyValues* keyValues = script_context.GetArgument<CEntityKeyValues*>(0);
    const char* key = script_context.GetArgument<const char*>(1);
    return keyValues->HasValue(key);
}

void EntityKeyValuesSetValue(ScriptContext& script_context)
{
    CEntityKeyValues* keyValues = script_context.GetArgument<CEntityKeyValues*>(0);
    const char* key = script_context.GetArgument<const char*>(1);
    KeyValuesType_t type = script_context.GetArgument<KeyValuesType_t>(2);

    int offset = 3;

    switch (type) {
        case counterstrikesharp::TYPE_BOOL:
            keyValues->SetBool(key, script_context.GetArgument<bool>(offset));
            break;

        case counterstrikesharp::TYPE_INT:
            keyValues->SetInt(key, script_context.GetArgument<int>(offset));
            break;

        case counterstrikesharp::TYPE_UINT:
            keyValues->SetUint(key, script_context.GetArgument<uint>(offset));
            break;

        case counterstrikesharp::TYPE_INT64:
            keyValues->SetInt64(key, script_context.GetArgument<int64>(offset));
            break;

        case counterstrikesharp::TYPE_UINT64:
            keyValues->SetUint64(key, script_context.GetArgument<uint64>(offset));
            break;

        case counterstrikesharp::TYPE_FLOAT:
            keyValues->SetFloat(key, script_context.GetArgument<float>(offset));
            break;

        case counterstrikesharp::TYPE_DOUBLE:
            keyValues->SetDouble(key, script_context.GetArgument<double>(offset));
            break;

        case counterstrikesharp::TYPE_STRING:
            keyValues->SetString(key, script_context.GetArgument<const char*>(offset));
            break;

        case counterstrikesharp::TYPE_POINTER:
            keyValues->SetPtr(key, script_context.GetArgument<void*>(offset));
            break;

        case counterstrikesharp::TYPE_STRING_TOKEN:
            keyValues->SetStringToken(key, CUtlStringToken(script_context.GetArgument<unsigned int>(offset)));
            break;

        case counterstrikesharp::TYPE_EHANDLE:
            keyValues->SetEHandle(key, CEntityHandle(script_context.GetArgument<unsigned int>(offset)));
            break;

        case counterstrikesharp::TYPE_COLOR:
        {
            char r = script_context.GetArgument<char>(offset);
            char g = script_context.GetArgument<char>(offset + 1);
            char b = script_context.GetArgument<char>(offset + 2);
            char a = script_context.GetArgument<char>(offset + 3);
            keyValues->SetColor(key, Color(r, g, b, a));
            break;
        }

        case counterstrikesharp::TYPE_VECTOR:
        {
            float x = script_context.GetArgument<float>(offset);
            float y = script_context.GetArgument<float>(offset + 1);
            float z = script_context.GetArgument<float>(offset + 2);
            keyValues->SetVector(key, Vector(x, y, z));
            break;
        }

        case counterstrikesharp::TYPE_VECTOR2D:
        {
            float x = script_context.GetArgument<float>(offset);
            float y = script_context.GetArgument<float>(offset + 1);
            keyValues->SetVector2D(key, Vector2D(x, y));
            break;
        }

        case counterstrikesharp::TYPE_VECTOR4D:
        {
            float x = script_context.GetArgument<float>(offset);
            float y = script_context.GetArgument<float>(offset + 1);
            float z = script_context.GetArgument<float>(offset + 2);
            float w = script_context.GetArgument<float>(offset + 3);
            keyValues->SetVector4D(key, Vector4D(x, y, z, w));
            break;
        }

        case counterstrikesharp::TYPE_QUATERNION:
        {
            float x = script_context.GetArgument<float>(offset);
            float y = script_context.GetArgument<float>(offset + 1);
            float z = script_context.GetArgument<float>(offset + 2);
            float w = script_context.GetArgument<float>(offset + 3);
            keyValues->SetQuaternion(key, Quaternion(x, y, z, w));
            break;
        }

        case counterstrikesharp::TYPE_QANGLE:
        {
            float x = script_context.GetArgument<float>(offset);
            float y = script_context.GetArgument<float>(offset + 1);
            float z = script_context.GetArgument<float>(offset + 2);
            keyValues->SetQAngle(key, QAngle(x, y, z));
            break;
        }

        case counterstrikesharp::TYPE_MATRIX3X4:
        {
            float m11 = script_context.GetArgument<float>(offset);
            float m12 = script_context.GetArgument<float>(offset + 1);
            float m13 = script_context.GetArgument<float>(offset + 2);
            float m14 = script_context.GetArgument<float>(offset + 3);

            float m21 = script_context.GetArgument<float>(offset + 4);
            float m22 = script_context.GetArgument<float>(offset + 5);
            float m23 = script_context.GetArgument<float>(offset + 6);
            float m24 = script_context.GetArgument<float>(offset + 7);

            float m31 = script_context.GetArgument<float>(offset + 8);
            float m32 = script_context.GetArgument<float>(offset + 9);
            float m33 = script_context.GetArgument<float>(offset + 10);
            float m34 = script_context.GetArgument<float>(offset + 11);

            keyValues->SetMatrix3x4(key, matrix3x4_t(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34));
            break;
        }

        default:
            script_context.ThrowNativeError("Invalid KeyValues Type! ({})", type);
            break;
    }
}

void EntityKeyValuesGetValue(ScriptContext& script_context)
{
    CEntityKeyValues* keyValues = script_context.GetArgument<CEntityKeyValues*>(0);
    const char* key = script_context.GetArgument<const char*>(1);
    KeyValuesType_t type = script_context.GetArgument<KeyValuesType_t>(2);

    switch (type) {
        case counterstrikesharp::TYPE_BOOL:
        {
            script_context.SetResult(keyValues->GetBool(key));
        } break;

        case counterstrikesharp::TYPE_INT:
        {
            script_context.SetResult(keyValues->GetInt(key));
        } break;

        case counterstrikesharp::TYPE_UINT:
        {
            script_context.SetResult(keyValues->GetUint(key));
        } break;

        case counterstrikesharp::TYPE_INT64:
        {
            script_context.SetResult(keyValues->GetInt64(key));
        } break;

        case counterstrikesharp::TYPE_UINT64:
        {
            script_context.SetResult(keyValues->GetUint64(key));
        } break;

        case counterstrikesharp::TYPE_FLOAT:
        {
            script_context.SetResult(keyValues->GetFloat(key));
        } break;

        case counterstrikesharp::TYPE_DOUBLE:
        {
            script_context.SetResult(keyValues->GetDouble(key));
        } break;

        case counterstrikesharp::TYPE_STRING:
        {
            script_context.SetResult(keyValues->GetString(key));
        } break;

        case counterstrikesharp::TYPE_POINTER:
        {
            script_context.SetResult(keyValues->GetPtr(key));
        } break;

        case counterstrikesharp::TYPE_STRING_TOKEN:
        {
            script_context.SetResult(keyValues->GetStringToken(key).GetHashCode());
        } break;

        case counterstrikesharp::TYPE_EHANDLE:
        {
            script_context.SetResult(keyValues->GetEHandle(key));
        } break;

        case counterstrikesharp::TYPE_COLOR:
        {
            script_context.SetResult(new Color(keyValues->GetColor(key)));
            break;
        }

        case counterstrikesharp::TYPE_VECTOR:
        {
            script_context.SetResult(new Vector(keyValues->GetVector(key)));
            break;
        }

        case counterstrikesharp::TYPE_VECTOR2D:
        {
            script_context.SetResult(new Vector2D(keyValues->GetVector2D(key)));
            break;
        }

        case counterstrikesharp::TYPE_VECTOR4D:
        {
            script_context.SetResult(new Vector4D(keyValues->GetVector4D(key)));
            break;
        }

        case counterstrikesharp::TYPE_QUATERNION:
        {
            script_context.SetResult(new Quaternion(keyValues->GetQuaternion(key)));
            break;
        }

        case counterstrikesharp::TYPE_QANGLE:
        {
            script_context.SetResult(new QAngle(keyValues->GetQAngle(key)));
            break;
        }

        case counterstrikesharp::TYPE_MATRIX3X4:
        {
            script_context.SetResult(new matrix3x4_t(keyValues->GetMatrix3x4(key)));
            break;
        }

        default:
        {
            script_context.ThrowNativeError("Invalid KeyValues Type! ({})", type);
        } break;
    }
}

REGISTER_NATIVES(entities, {
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_FROM_INDEX", GetEntityFromIndex);
    ScriptEngine::RegisterNativeHandler("GET_USERID_FROM_INDEX", GetUserIdFromIndex);
    ScriptEngine::RegisterNativeHandler("GET_DESIGNER_NAME", GetDesignerName);
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_POINTER_FROM_HANDLE", GetEntityPointerFromHandle);
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_POINTER_FROM_REF", GetEntityPointerFromRef);
    ScriptEngine::RegisterNativeHandler("GET_REF_FROM_ENTITY_POINTER", GetRefFromEntityPointer);
    ScriptEngine::RegisterNativeHandler("GET_CONCRETE_ENTITY_LIST_POINTER", GetConcreteEntityListPointer);
    ScriptEngine::RegisterNativeHandler("IS_REF_VALID_ENTITY", IsRefValidEntity);
    ScriptEngine::RegisterNativeHandler("PRINT_TO_CONSOLE", PrintToConsole);
    ScriptEngine::RegisterNativeHandler("GET_FIRST_ACTIVE_ENTITY", GetFirstActiveEntity);
    ScriptEngine::RegisterNativeHandler("GET_PLAYER_AUTHORIZED_STEAMID", GetPlayerAuthorizedSteamID);
    ScriptEngine::RegisterNativeHandler("GET_PLAYER_IP_ADDRESS", GetPlayerIpAddress);
    ScriptEngine::RegisterNativeHandler("HOOK_ENTITY_OUTPUT", HookEntityOutput);
    ScriptEngine::RegisterNativeHandler("UNHOOK_ENTITY_OUTPUT", UnhookEntityOutput);
    ScriptEngine::RegisterNativeHandler("ACCEPT_INPUT", AcceptInput);
    ScriptEngine::RegisterNativeHandler("ADD_ENTITY_IO_EVENT", AddEntityIOEvent);
    ScriptEngine::RegisterNativeHandler("DISPATCH_SPAWN", DispatchSpawn);
    ScriptEngine::RegisterNativeHandler("ENTITY_KEY_VALUES_NEW", EntityKeyValuesNew);
    ScriptEngine::RegisterNativeHandler("ENTITY_KEY_VALUES_GET_VALUE", EntityKeyValuesGetValue);
    ScriptEngine::RegisterNativeHandler("ENTITY_KEY_VALUES_SET_VALUE", EntityKeyValuesSetValue);
    ScriptEngine::RegisterNativeHandler("ENTITY_KEY_VALUES_HAS_VALUE", EntityKeyValuesHasValue);
})
} // namespace counterstrikesharp
