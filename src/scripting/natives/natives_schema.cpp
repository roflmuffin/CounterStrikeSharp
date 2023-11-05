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
#include <dlfcn.h>

#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"
#include "schema.h"
#include "core/function.h"

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

void GetSchemaValueByName(ScriptContext& script_context)
{
    auto instancePointer = script_context.GetArgument<void*>(0);
    auto returnType = script_context.GetArgument<DataType_t>(1);
    auto className = script_context.GetArgument<const char*>(2);
    auto memberName = script_context.GetArgument<const char*>(3);
    auto classKey = hash_32_fnv1a_const(className);
    auto memberKey = hash_32_fnv1a_const(memberName);

    const auto m_key = schema::GetOffset(className, classKey, memberName, memberKey);

    switch (returnType) {
    case DATA_TYPE_BOOL:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<bool>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_CHAR:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<char>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_UCHAR:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned char>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_SHORT:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<short>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_USHORT:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned short>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_INT:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<int>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_UINT:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned int>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_LONG:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<long>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_ULONG:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned long>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_LONG_LONG:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<long long>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_ULONG_LONG:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<uint64_t>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_FLOAT:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<float>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_DOUBLE:
        script_context.SetResult(*reinterpret_cast<std::add_pointer_t<double>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_POINTER:
        script_context.SetResult(reinterpret_cast<std::add_pointer_t<void>>(
            (uintptr_t)(instancePointer) + m_key.offset));
        break;
    case DATA_TYPE_STRING:
        script_context.SetResult(reinterpret_cast<std::add_pointer_t<char>>(
            (uintptr_t)(instancePointer) + m_key.offset));
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
    auto classKey = hash_32_fnv1a_const(className);
    auto memberKey = hash_32_fnv1a_const(memberName);

    const auto m_key = schema::GetOffset(className, classKey, memberName, memberKey);
    const auto m_chain = schema::FindChainOffset(className);


    // todo network updates
    //    if (m_chain != 0 && m_key.networked) {
    //        addresses::NetworkStateChanged((uintptr_t)(instancePointer) + m_chain, m_key.offset,
    //                                       0xFFFFFFFF);
    //    } else if (m_key.networked) { /* WIP: Works fine for most props, but inlined classes in
    //    the
    //                                     middle of a class will need to have their this pointer
    //                                     corrected by the offset .*/
    //        CALL_VIRTUAL(void, 1, instancePointer, m_key.offset, 0xFFFFFFFF, 0xFFFF);
    //    }

    switch (dataType) {
    case DATA_TYPE_BOOL:
        *reinterpret_cast<std::add_pointer_t<bool>>((uintptr_t)(instancePointer) + m_key.offset) =
            script_context.GetArgument<bool>(4);
        break;
    case DATA_TYPE_CHAR:
        *reinterpret_cast<std::add_pointer_t<char>>((uintptr_t)(instancePointer) + m_key.offset) =
            script_context.GetArgument<char>(4);
        break;
    case DATA_TYPE_UCHAR:
        *reinterpret_cast<std::add_pointer_t<unsigned char>>((uintptr_t)(instancePointer) +
                                                             m_key.offset) =
            script_context.GetArgument<unsigned char>(4);
        break;
    case DATA_TYPE_SHORT:
        *reinterpret_cast<std::add_pointer_t<short>>((uintptr_t)(instancePointer) + m_key.offset) =
            script_context.GetArgument<short>(4);
        break;
    case DATA_TYPE_USHORT:
        *reinterpret_cast<std::add_pointer_t<unsigned short>>((uintptr_t)(instancePointer) +
                                                              m_key.offset) =
            script_context.GetArgument<unsigned short>(4);
        break;
    case DATA_TYPE_INT:
        *reinterpret_cast<std::add_pointer_t<int>>((uintptr_t)(instancePointer) + m_key.offset) =
            script_context.GetArgument<int>(4);
        break;
    case DATA_TYPE_UINT:
        *reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(instancePointer) +
                                                            m_key.offset) =
            script_context.GetArgument<unsigned int>(4);
        break;
    case DATA_TYPE_LONG:
        *reinterpret_cast<std::add_pointer_t<long>>((uintptr_t)(instancePointer) + m_key.offset) =
            script_context.GetArgument<long>(4);
        break;
    case DATA_TYPE_ULONG:
        *reinterpret_cast<std::add_pointer_t<unsigned long>>((uintptr_t)(instancePointer) +
                                                             m_key.offset) =
            script_context.GetArgument<unsigned long>(4);
        break;
    case DATA_TYPE_LONG_LONG:
        *reinterpret_cast<std::add_pointer_t<long long>>(
            (uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<long long>(4);
        break;
    case DATA_TYPE_ULONG_LONG:
        *reinterpret_cast<std::add_pointer_t<uint64_t>>(
            (uintptr_t)(instancePointer) + m_key.offset) = script_context.GetArgument<uint64_t>(4);
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
        *reinterpret_cast<void**>((uintptr_t)(instancePointer) + m_key.offset) =
            script_context.GetArgument<void*>(4);
        break;
    case DATA_TYPE_STRING: {
        auto duplicated = strdup(script_context.GetArgument<const char*>(4));
        *reinterpret_cast<char**>((uintptr_t)(instancePointer) + m_key.offset) =
            duplicated;
        break;
    }
    default:
        assert(!"Unknown function data type!");
        break;
    }
}

REGISTER_NATIVES(schema, {
    ScriptEngine::RegisterNativeHandler("GET_SCHEMA_OFFSET", GetSchemaOffset);
    ScriptEngine::RegisterNativeHandler("GET_SCHEMA_VALUE_BY_NAME", GetSchemaValueByName);
    ScriptEngine::RegisterNativeHandler("SET_SCHEMA_VALUE_BY_NAME", SetSchemaValueByName);
})
} // namespace counterstrikesharp