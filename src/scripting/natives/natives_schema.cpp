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

void GetSchemaValueByName(ScriptContext &script_context) {
    auto instancePointer = script_context.GetArgument<void *>(0);
    auto returnType = script_context.GetArgument<DataType_t>(1);
    auto className = script_context.GetArgument<const char *>(2);
    auto propName = script_context.GetArgument<const char *>(3);
    static auto datatable_hash = hash_32_fnv1a_const(className);
    static auto prop_hash = hash_32_fnv1a_const(propName);

    static const auto m_key = schema::GetOffset(className, datatable_hash, propName, prop_hash);

    CSSHARP_CORE_TRACE("Offset of {}:{}({}) is ({})", className, propName, instancePointer,
                       m_key.offset);

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
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<unsigned long long>>(
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
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<void *>>(
                (uintptr_t)(instancePointer) + m_key.offset));
            break;
        case DATA_TYPE_STRING:
            script_context.SetResult(*reinterpret_cast<std::add_pointer_t<const char *>>(
                (uintptr_t)(instancePointer) + m_key.offset));
            break;
        default:
            assert(!"Unknown function return type!");
            break;
    }
}

REGISTER_NATIVES(schema, {
    ScriptEngine::RegisterNativeHandler("GET_SCHEMA_VALUE_BY_NAME", GetSchemaValueByName);
})
}  // namespace counterstrikesharp