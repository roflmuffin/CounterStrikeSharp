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

#include <ios>
#include <sstream>

#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"
#include "schema.h"
#include "core/function.h"
#include "core/coreconfig.h"
#include "interfaces/cs2_interfaces.h"
#include <schemasystem/schemasystem.h>

namespace counterstrikesharp {

int16 GetSchemaOffset(ScriptContext& script_context)
{
    auto className = script_context.GetArgument<const char*>(0);
    auto memberName = script_context.GetArgument<const char*>(1);
    auto classKey = hash_32_fnv1a_const(className);
    auto memberKey = hash_32_fnv1a_const(memberName);

    const auto m_key = schema::GetOffset(className, classKey, memberName, memberKey);

    return m_key.offset;
}

// Offset of the field, or a negative status for a miss (see LOOKUP_SCHEMA_FIELD in natives_schema.yaml).
constexpr int kSchemaClassMissing = -1;
constexpr int kSchemaFieldMissing = -2;

int LookupSchemaField(ScriptContext& script_context)
{
    auto className = script_context.GetArgument<const char*>(0);
    auto memberName = script_context.GetArgument<const char*>(1);

    const auto lookup = schema::LookupField(className, hash_32_fnv1a_const(className), memberName, hash_32_fnv1a_const(memberName));
    switch (lookup.status)
    {
        case schema::LookupStatus::Found:
            return lookup.key.offset;
        case schema::LookupStatus::ClassMissing:
            return kSchemaClassMissing;
        case schema::LookupStatus::FieldMissing:
            break;
    }
    return kSchemaFieldMissing;
}

// Looks up a field for a by-name read or write; throws to managed instead of returning offset 0 for a miss.
static bool LookupSchemaFieldOrThrow(ScriptContext& script_context, const char* className, const char* memberName, SchemaKey& key)
{
    const auto lookup = schema::LookupField(className, hash_32_fnv1a_const(className), memberName, hash_32_fnv1a_const(memberName));
    if (lookup.status == schema::LookupStatus::Found)
    {
        key = lookup.key;
        return true;
    }

    script_context.ThrowNativeError("Schema field %s::%s not found (%s)", className, memberName,
                                    lookup.status == schema::LookupStatus::ClassMissing ? "class missing" : "field missing");
    return false;
}

bool IsSchemaFieldNetworked(ScriptContext& script_context)
{
    auto className = script_context.GetArgument<const char*>(0);
    auto memberName = script_context.GetArgument<const char*>(1);
    auto classKey = hash_32_fnv1a_const(className);
    auto memberKey = hash_32_fnv1a_const(memberName);

    const auto m_key = schema::GetOffset(className, classKey, memberName, memberKey);

    return m_key.networked;
}

int GetSchemaClassSize(ScriptContext& script_context)
{
    auto className = script_context.GetArgument<const char*>(0);

    CSchemaSystemTypeScope* pType = globals::schemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);
    if (!pType) return -1;

    SchemaClassInfoData_t* pClassInfo = pType->FindDeclaredClass(className).Get();
    if (!pClassInfo) return -1;

    return pClassInfo->m_nSizeOf;
}

void GetSchemaValueByName(ScriptContext& script_context)
{
    auto instancePointer = script_context.GetArgument<void*>(0);
    auto returnType = script_context.GetArgument<DataType_t>(1);
    auto className = script_context.GetArgument<const char*>(2);
    auto memberName = script_context.GetArgument<const char*>(3);

    SchemaKey m_key;
    if (!LookupSchemaFieldOrThrow(script_context, className, memberName, m_key)) return;

    switch (returnType)
    {
        case DATA_TYPE_BOOL:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<bool>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_CHAR:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<char>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_UCHAR:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned char>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_SHORT:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<short>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_USHORT:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned short>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_INT:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<int>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_UINT:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_LONG:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<long>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_ULONG:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned long>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_LONG_LONG:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<long long>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_ULONG_LONG:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<uint64_t>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_FLOAT:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<float>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_DOUBLE:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<double>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_POINTER:
            script_context.SetResult(reinterpret_cast<std::add_pointer_t<void>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_STRING:
            script_context.SetResult(reinterpret_cast<std::add_pointer_t<char>>((uintptr_t)(instancePointer) + m_key.offset));
            break;
        default:
            assert(!"Unknown function return type!");
            break;
    }
}

void SetSchemaValueByName(ScriptContext& script_context)
{
    auto instancePointer = script_context.GetArgument<void*>(0);
    auto dataType = script_context.GetArgument<DataType_t>(1);
    auto className = script_context.GetArgument<const char*>(2);
    auto memberName = script_context.GetArgument<const char*>(3);

    if (globals::coreConfig->FollowCS2ServerGuidelines &&
        std::find(schema::CS2BadList.begin(), schema::CS2BadList.end(), memberName) != schema::CS2BadList.end())
    {
        CSSHARP_CORE_ERROR("Cannot set '{}::{}' with \"FollowCS2ServerGuidelines\" option enabled.", className, memberName);
        return;
    }

    SchemaKey m_key;
    if (!LookupSchemaFieldOrThrow(script_context, className, memberName, m_key)) return;

    switch (dataType)
    {
        case DATA_TYPE_BOOL:
            *reinterpret_cast<std::add_pointer_t<bool>>((uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<bool>(4);
            break;
        case DATA_TYPE_CHAR:
            *reinterpret_cast<std::add_pointer_t<char>>((uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<char>(4);
            break;
        case DATA_TYPE_UCHAR:
            *reinterpret_cast<std::add_pointer_t<unsigned char>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<unsigned char>(4);
            break;
        case DATA_TYPE_SHORT:
            *reinterpret_cast<std::add_pointer_t<short>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<short>(4);
            break;
        case DATA_TYPE_USHORT:
            *reinterpret_cast<std::add_pointer_t<unsigned short>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<unsigned short>(4);
            break;
        case DATA_TYPE_INT:
            *reinterpret_cast<std::add_pointer_t<int>>((uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<int>(4);
            break;
        case DATA_TYPE_UINT:
            *reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<unsigned int>(4);
            break;
        case DATA_TYPE_LONG:
            *reinterpret_cast<std::add_pointer_t<long>>((uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<long>(4);
            break;
        case DATA_TYPE_ULONG:
            *reinterpret_cast<std::add_pointer_t<unsigned long>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<unsigned long>(4);
            break;
        case DATA_TYPE_LONG_LONG:
            *reinterpret_cast<std::add_pointer_t<long long>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<long long>(4);
            break;
        case DATA_TYPE_ULONG_LONG:
            *reinterpret_cast<std::add_pointer_t<uint64_t>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<uint64_t>(4);
            break;
        case DATA_TYPE_FLOAT:
            *reinterpret_cast<std::add_pointer_t<float>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<float>(4);
            break;
        case DATA_TYPE_DOUBLE:
            *reinterpret_cast<std::add_pointer_t<double>>((uintptr_t)(instancePointer) + m_key.offset) =
                script_context.GetArgument<double>(4);
            break;
        case DATA_TYPE_POINTER:
            *reinterpret_cast<void**>((uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<void*>(4);
            break;
        case DATA_TYPE_STRING:
        {
            auto duplicated = strdup(script_context.GetArgument<const char*>(4));
            *reinterpret_cast<char**>((uintptr_t)(instancePointer) + m_key.offset) = duplicated;
            break;
        }
        default:
            assert(!"Unknown function data type!");
            break;
    }
}

void SchemaSetStateChanged(ScriptContext& script_context)
{
    auto instancePointer = script_context.GetArgument<void*>(0);
    auto offset = script_context.GetArgument<uint32_t>(1);
    auto nArrayIndex = script_context.GetArgument<uint32_t>(2);
    auto nPathIndex = script_context.GetArgument<uint32_t>(3);

    SetStateChanged(reinterpret_cast<uintptr_t>(instancePointer), offset, nArrayIndex, nPathIndex);
}

void SchemaNetworkStateChanged(ScriptContext& script_context)
{
    auto instancePointer = script_context.GetArgument<void*>(0);
    auto offset = script_context.GetArgument<uint32_t>(1);
    auto nArrayIndex = script_context.GetArgument<uint32_t>(2);
    auto nPathIndex = script_context.GetArgument<uint32_t>(3);

    NetworkStateChanged(reinterpret_cast<uintptr_t>(instancePointer), offset, nArrayIndex, nPathIndex);

} // namespace counterstrikesharp

REGISTER_NATIVES(schema, {
    ScriptEngine::RegisterNativeHandler("GET_SCHEMA_OFFSET", GetSchemaOffset);
    ScriptEngine::RegisterNativeHandler("LOOKUP_SCHEMA_FIELD", LookupSchemaField);
    ScriptEngine::RegisterNativeHandler("IS_SCHEMA_FIELD_NETWORKED", IsSchemaFieldNetworked);
    ScriptEngine::RegisterNativeHandler("GET_SCHEMA_VALUE_BY_NAME", GetSchemaValueByName);
    ScriptEngine::RegisterNativeHandler("SET_SCHEMA_VALUE_BY_NAME", SetSchemaValueByName);
    ScriptEngine::RegisterNativeHandler("GET_SCHEMA_CLASS_SIZE", GetSchemaClassSize);
    ScriptEngine::RegisterNativeHandler("SCHEMA_SET_STATE_CHANGED", SchemaSetStateChanged);
    ScriptEngine::RegisterNativeHandler("SCHEMA_NETWORK_STATE_CHANGED", SchemaNetworkStateChanged);
})
} // namespace counterstrikesharp
