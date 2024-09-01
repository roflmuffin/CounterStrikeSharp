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

#include <ios>
#include <sstream>

#include "core/log.h"
#include "core/memory.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

_fieldtypes GetVariantType(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return _fieldtypes::FIELD_TYPEUNKNOWN;
    }
    return pVariant->m_type;
}

static int GetVariantInt(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return 0;
    }
    return pVariant->m_int;
}

static uint GetVariantUInt(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return 0;
    }
    return pVariant->m_uint;
}

static float GetVariantFloat(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return 0;
    }
    return pVariant->m_float;
}

static const char* GetVariantString(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return nullptr;
    }
    return pVariant->m_pszString;
}

static bool GetVariantBool(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return false;
    }
    return pVariant->m_bool;
}

REGISTER_NATIVES(cvariant, {
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_TYPE", GetVariantType);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_INT", GetVariantInt);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_UINT", GetVariantUInt);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_FLOAT", GetVariantFloat);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_STRING", GetVariantString);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_BOOL", GetVariantBool);
})
} // namespace counterstrikesharp
