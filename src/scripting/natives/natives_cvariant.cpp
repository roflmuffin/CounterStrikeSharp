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
    if (pVariant->m_type != _fieldtypes::FIELD_INT32)
    {
        script_context.ThrowNativeError("Variant type is not int");
        return 0.0f;
    }
    return pVariant->m_int32;
}

static uint GetVariantUInt(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return 0;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_UINT32)
    {
        script_context.ThrowNativeError("Variant type is not uint");
        return 0.0f;
    }
    return pVariant->m_uint32;
}

static float GetVariantFloat(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return 0;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_FLOAT32)
    {
        script_context.ThrowNativeError("Variant type is not float");
        return 0.0f;
    }
    return pVariant->m_float32;
}

static const char* GetVariantString(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return "";
    }
    if (pVariant->m_type != _fieldtypes::FIELD_STRING)
    {
        script_context.ThrowNativeError("Variant type is not string");
        return "";
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
    if (pVariant->m_type != _fieldtypes::FIELD_BOOLEAN)
    {
        script_context.ThrowNativeError("Variant type is not boolean");
        return 0.0f;
    }
    return pVariant->m_bool;
}

static void SetVariantInt(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_INT32)
    {
        script_context.ThrowNativeError("Variant type is not int");
        return;
    }

    int value = script_context.GetArgument<int>(1);
    pVariant->m_int32 = value;
}

static void SetVariantUInt(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_UINT32)
    {
        script_context.ThrowNativeError("Variant type is not uint");
        return;
    }

    uint value = script_context.GetArgument<uint>(1);
    pVariant->m_uint32 = value;
}

static void SetVariantFloat(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_FLOAT32)
    {
        script_context.ThrowNativeError("Variant type is not float");
        return;
    }

    float value = script_context.GetArgument<float>(1);
    pVariant->m_float32 = value;
}

static void SetVariantString(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_STRING)
    {
        script_context.ThrowNativeError("Variant type is not string");
        return;
    }

    const char* value = script_context.GetArgument<const char*>(1);
    pVariant->m_pszString = value;
}

static void SetVariantBool(ScriptContext& script_context)
{
    variant_t* pVariant = script_context.GetArgument<variant_t*>(0);
    if (!pVariant)
    {
        script_context.ThrowNativeError("Invalid variant pointer");
        return;
    }
    if (pVariant->m_type != _fieldtypes::FIELD_BOOLEAN)
    {
        script_context.ThrowNativeError("Variant type is not boolean");
        return;
    }

    bool value = script_context.GetArgument<bool>(1);
    pVariant->m_bool = value;
}

REGISTER_NATIVES(cvariant, {
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_TYPE", GetVariantType);

    ScriptEngine::RegisterNativeHandler("GET_VARIANT_INT", GetVariantInt);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_UINT", GetVariantUInt);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_FLOAT", GetVariantFloat);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_STRING", GetVariantString);
    ScriptEngine::RegisterNativeHandler("GET_VARIANT_BOOL", GetVariantBool);

    ScriptEngine::RegisterNativeHandler("SET_VARIANT_INT", SetVariantInt);
    ScriptEngine::RegisterNativeHandler("SET_VARIANT_UINT", SetVariantUInt);
    ScriptEngine::RegisterNativeHandler("SET_VARIANT_FLOAT", SetVariantFloat);
    ScriptEngine::RegisterNativeHandler("SET_VARIANT_STRING", SetVariantString);
    ScriptEngine::RegisterNativeHandler("SET_VARIANT_BOOL", SetVariantBool);
})
} // namespace counterstrikesharp
