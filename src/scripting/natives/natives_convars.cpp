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

#define private   public
#define protected public

#include "core/log.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

#include <eiface.h>
#include <convar.h>
#undef private
#undef protected

namespace counterstrikesharp {

static void SetConvarFlags(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    auto flags = script_context.GetArgument<uint64_t>(1);
    ref.GetConVarData()->m_nFlags = flags;
}

static void GetConvarFlags(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    script_context.SetResult(ref.GetConVarData()->m_nFlags);
}

static void GetConvarType(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    script_context.SetResult(ref.GetConVarData()->GetType());
}

static void GetConvarName(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    script_context.SetResult(ref.GetConVarData()->GetName());
}

static void GetConvarHelpText(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (!ref.GetConVarData()->HasHelpText())
    {
        return;
    }

    script_context.SetResult(ref.GetConVarData()->GetHelpText());
}

static void GetConvarAccessIndexByName(ScriptContext& script_context)
{
    auto convarName = script_context.GetArgument<const char*>(0);
    ConVarRef ref(convarName);

    if (!ref.IsValidRef())
    {
        return;
    }

    script_context.SetResult(ref.GetAccessIndex());
}

static void GetConvarValue(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto cvar = ConVarRefAbstract(convarAccessIndex);
    CSplitScreenSlot server(0);

    if (!cvar.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (!cvar.IsConVarDataValid()) return;

    switch (cvar.GetType())
    {
        case EConVarType_Int16:
        {
            script_context.SetResult(cvar.GetAs<int16>(server));
            break;
        }
        case EConVarType_UInt16:
        {
            script_context.SetResult(cvar.GetAs<uint16>(server));
            break;
        }
        case EConVarType_UInt32:
        {
            script_context.SetResult(cvar.GetAs<uint32>(server));
            break;
        }
        case EConVarType_Int32:
        {
            script_context.SetResult(cvar.GetAs<int32>(server));
            break;
        }
        case EConVarType_UInt64:
        {
            script_context.SetResult(cvar.GetAs<uint64>(server));
            break;
        }
        case EConVarType_Int64:
        {
            script_context.SetResult(cvar.GetAs<int64>(server));
            break;
        }
        case EConVarType_Bool:
        {
            script_context.SetResult(cvar.GetAs<bool>(server));
            break;
        }
        case EConVarType_Float32:
        {
            script_context.SetResult((float)cvar.GetAs<float32>(server));
            break;
        }
        case EConVarType_Float64:
        {
            script_context.SetResult((double)cvar.GetAs<float64>(server));
            break;
        }
        case EConVarType_String:
        {
            script_context.SetResult(cvar.GetString(server).String());
            break;
        }
        case EConVarType_Color:
        {
            script_context.SetResult(&(cvar.GetConVarData()->ValueOrDefault(server)->m_clrValue));
            break;
        }
        case EConVarType_Vector2:
        {
            script_context.SetResult(&(cvar.GetConVarData()->ValueOrDefault(server)->m_vec2Value));
            break;
        }
        case EConVarType_Vector3:
        {
            script_context.SetResult(&(cvar.GetConVarData()->ValueOrDefault(server)->m_vec3Value));
            break;
        }
        case EConVarType_Vector4:
        {
            script_context.SetResult(&(cvar.GetConVarData()->ValueOrDefault(server)->m_vec4Value));
            break;
        }
        case EConVarType_Qangle:
        {
            script_context.SetResult(&(cvar.GetConVarData()->ValueOrDefault(server)->m_angValue));
            break;
        }
        default:
        {
            script_context.SetResult(nullptr);
        }
    }
}
static void SetConvarValue(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto cvar = ConVarRefAbstract(convarAccessIndex);
    CSplitScreenSlot server(0);

    if (!cvar.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (!cvar.IsConVarDataValid()) return;

    switch (cvar.GetType())
    {
        case EConVarType_Int16:
        {
            cvar.SetAs<int16>(script_context.GetArgument<int16>(1), server);
            break;
        }
        case EConVarType_UInt16:
        {
            cvar.SetAs<uint16>(script_context.GetArgument<uint16>(1), server);
            break;
        }
        case EConVarType_UInt32:
        {
            cvar.SetAs<uint32>(script_context.GetArgument<uint32>(1), server);
            break;
        }
        case EConVarType_Int32:
        {
            cvar.SetAs<int32>(script_context.GetArgument<int32>(1), server);
            break;
        }
        case EConVarType_UInt64:
        {
            cvar.SetAs<uint64>(script_context.GetArgument<uint64>(1), server);
            break;
        }
        case EConVarType_Int64:
        {
            cvar.SetAs<int64>(script_context.GetArgument<int64>(1), server);
            break;
        }
        case EConVarType_Bool:
        {
            cvar.SetAs<bool>(script_context.GetArgument<bool>(1), server);
            break;
        }
        case EConVarType_Float32:
        {
            cvar.SetAs<float32>(script_context.GetArgument<float32>(1), server);
            break;
        }
        case EConVarType_Float64:
        {
            cvar.SetAs<float64>(script_context.GetArgument<float64>(1), server);
            break;
        }
        case EConVarType_String:
        {
            cvar.SetString(script_context.GetArgument<const char*>(1), server);
            break;
        }
        case EConVarType_Color:
        {
            cvar.SetAs<Color>(*script_context.GetArgument<Color*>(1), server);
            break;
        }
        case EConVarType_Vector2:
        {
            cvar.SetAs<Vector2D>(*script_context.GetArgument<Vector2D*>(1), server);
            break;
        }
        case EConVarType_Vector3:
        {
            cvar.SetAs<Vector>(*script_context.GetArgument<Vector*>(1), server);
            break;
        }
        case EConVarType_Vector4:
        {
            cvar.SetAs<Vector4D>(*script_context.GetArgument<Vector4D*>(1), server);
            break;
        }
        case EConVarType_Qangle:
        {
            cvar.SetAs<QAngle>(*script_context.GetArgument<QAngle*>(1), server);
            break;
        }
        default:
        {
            script_context.ThrowNativeError("Unsupported convar type: %d", cvar.GetType());
        }
    }
}

REGISTER_NATIVES(convars, {
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_FLAGS", SetConvarFlags);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_FLAGS", GetConvarFlags);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_TYPE", GetConvarType);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_NAME", GetConvarName);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_HELP_TEXT", GetConvarHelpText);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_ACCESS_INDEX_BY_NAME", GetConvarAccessIndexByName);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_VALUE", GetConvarValue);
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_VALUE", SetConvarValue);
})
} // namespace counterstrikesharp
