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
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>.
 */

#include "core/log.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

// ---- Flag setter compatible with various SDKs ----
template <typename T>
concept HasAddClear = requires(T* t, uint64_t f) {
    t->AddFlags(f);
    t->ClearFlags(f);
};

template <typename T>
concept HasAddRemove = requires(T* t, uint64_t f) {
    t->AddFlags(f);
    t->RemoveFlags(f);
};

template <typename T>
concept HasSetFlagBit = requires(T* t, uint64_t f) {
    t->SetFlag(f, true);
    t->SetFlag(f, false);
};

template <typename T> void SetAllFlagsCompat(T* data, uint64_t desired)
{
    uint64_t cur = data->GetFlags();
    uint64_t add = desired & ~cur;
    uint64_t rem = cur & ~desired;

    if constexpr (HasAddClear<T>)
    {
        if (add) data->AddFlags(add);
        if (rem) data->ClearFlags(rem);
    }
    else if constexpr (HasAddRemove<T>)
    {
        if (add) data->AddFlags(add);
        if (rem) data->RemoveFlags(rem);
    }
    else if constexpr (HasSetFlagBit<T>)
    {
        // Fallback: set/clear bitwise
        for (int i = 0; i < 64; i)
        {
            uint64_t bit = (1ULL << i);
            bool want = (desired & bit) != 0;
            data->SetFlag(bit, want);
        }
    }
    else
    {
        static_assert(sizeof(T) == 0, "ConVarData hat keine passende Flags-API (Add/Clear/Remove/SetFlag).");
    }
}
// ------------------------------------------------------

// First STL/SPDLOG, then SDK with the hack – and clean up immediately afterwards
#ifdef private
#undef private
#endif
#ifdef protected
#undef protected
#endif
#define private public
#include <eiface.h>
#include <convar.h>
#undef private
#ifdef protected
#undef protected
#endif

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
    SetAllFlagsCompat(ref.GetConVarData(), flags);
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

    script_context.SetResult(ref.GetConVarData()->GetFlags());
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
        script_context.SetResult(0);
        return;
    }

    script_context.SetResult(ref.GetAccessIndex());
}

static void GetConvarValueAsString(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);
    CSplitScreenSlot server(0);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (!ref.IsConVarDataValid())
    {
        script_context.ThrowNativeError("Convar data is not valid for access index %d.", convarAccessIndex);
        return;
    }

    CBufferString buf;
    ref.GetValueAsString(buf, server);

    std::string result = buf.Get();
    script_context.SetResult(result.c_str());
}

static void SetConvarValueAsString(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);
    CSplitScreenSlot server(0);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (!ref.IsConVarDataValid())
    {
        script_context.ThrowNativeError("Convar data is not valid for access index %d.", convarAccessIndex);
        return;
    }

    auto value = script_context.GetArgument<const char*>(1);
    if (!ref.SetString(value, server))
    {
        script_context.ThrowNativeError("Failed to set value for convar %s.", ref.GetName());
        return;
    }
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

    if (!cvar.IsConVarDataValid())
    {
        script_context.ThrowNativeError("Convar data is not valid for access index %d.", convarAccessIndex);
        return;
    }

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

static void GetConvarValueAddress(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto cvar = ConVarRefAbstract(convarAccessIndex);
    CSplitScreenSlot server(0);

    if (!cvar.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (!cvar.IsConVarDataValid())
    {
        script_context.ThrowNativeError("Convar data is not valid for access index %d.", convarAccessIndex);
        return;
    }

    script_context.SetResult(cvar.GetConVarData()->ValueOrDefault(server));
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

    if (!cvar.IsConVarDataValid())
    {
        script_context.ThrowNativeError("Convar data is not valid for access index %d.", convarAccessIndex);
        return;
    }

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

#define CREATE_CVAR(type)                                                                                                     \
    auto createdConVar = new CConVar<type>(name, flags, helpText, script_context.GetArgument<type>(6), hasMin,                \
                                           script_context.GetArgument<type>(7), hasMax, script_context.GetArgument<type>(8)); \
    createdConVarPtr = (void*)createdConVar;                                                                                  \
    createdConVarAccessIndex = createdConVar->GetAccessIndex();

#define CREATE_CVAR_PTR(type)                                                                                                     \
    auto createdConVar = new CConVar<type>(name, flags, helpText, *script_context.GetArgument<type*>(6), hasMin,                  \
                                           *script_context.GetArgument<type*>(7), hasMax, *script_context.GetArgument<type*>(8)); \
    createdConVarPtr = (void*)createdConVar;                                                                                      \
    createdConVarAccessIndex = createdConVar->GetAccessIndex();

static void CreateConVar(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto type = script_context.GetArgument<EConVarType>(1);
    auto helpText = script_context.GetArgument<const char*>(2);
    auto flags = script_context.GetArgument<uint64_t>(3);
    auto hasMin = script_context.GetArgument<bool>(4);
    auto hasMax = script_context.GetArgument<bool>(5);

    ConVarRefAbstract cvar(name);
    if (cvar.IsValidRef())
    {
        script_context.ThrowNativeError("Convar with name '%s' already exists.", name);
        return;
    }

    uint16 createdConVarAccessIndex = 0;
    void* createdConVarPtr = nullptr;

    switch (type)
    {
        case EConVarType_Int16:
        {
            CREATE_CVAR(int16);
            break;
        }
        case EConVarType_UInt16:
        {
            CREATE_CVAR(uint16);
            break;
        }
        case EConVarType_UInt32:
        {
            CREATE_CVAR(uint32);
            break;
        }
        case EConVarType_Int32:
        {
            CREATE_CVAR(int32);
            break;
        }
        case EConVarType_UInt64:
        {
            CREATE_CVAR(uint64);
            break;
        }
        case EConVarType_Int64:
        {
            CREATE_CVAR(int64);
            break;
        }
        case EConVarType_Bool:
        {
            CREATE_CVAR(bool);
            break;
        }
        case EConVarType_Float32:
        {
            CREATE_CVAR(float32);
            break;
        }
        case EConVarType_Float64:
        {
            CREATE_CVAR(float64);
            break;
        }
        case EConVarType_String:
        {
            auto createdConVar =
                new CConVar<CUtlString>(name, flags, helpText, script_context.GetArgument<const char*>(6), hasMin,
                                        script_context.GetArgument<const char*>(7), hasMax, script_context.GetArgument<const char*>(8));
            createdConVarAccessIndex = createdConVar->GetAccessIndex();
            break;
        }
        case EConVarType_Vector2:
        {
            CREATE_CVAR_PTR(Vector2D);
            break;
        }
        case EConVarType_Vector3:
        {
            CREATE_CVAR_PTR(Vector);
            break;
        }
        case EConVarType_Vector4:
        {
            CREATE_CVAR_PTR(Vector4D);
            break;
        }
        case EConVarType_Qangle:
        {
            CREATE_CVAR_PTR(QAngle);
            break;
        }
        default:
        {
            script_context.ThrowNativeError("Unsupported convar type: %d", type);
            return;
        }
    }

    script_context.SetResult(createdConVarAccessIndex);
}

static void DeleteConVar(ScriptContext& script_context)
{
    auto convarAccessIndex = script_context.GetArgument<uint16>(0);
    auto ref = ConVarRefAbstract(convarAccessIndex);

    if (!ref.IsValidRef())
    {
        script_context.ThrowNativeError("Invalid convar access index.");
        return;
    }

    if (ref.GetConVarData() == nullptr)
    {
        script_context.ThrowNativeError("Convar data is null.");
        return;
    }

    ref.GetConVarData()->Invalidate();
}

REGISTER_NATIVES(convars, {
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_FLAGS", SetConvarFlags);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_FLAGS", GetConvarFlags);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_TYPE", GetConvarType);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_NAME", GetConvarName);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_HELP_TEXT", GetConvarHelpText);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_ACCESS_INDEX_BY_NAME", GetConvarAccessIndexByName);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_VALUE", GetConvarValue);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_VALUE_ADDRESS", GetConvarValueAddress);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_VALUE_AS_STRING", GetConvarValueAsString);
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_VALUE_AS_STRING", SetConvarValueAsString);
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_VALUE", SetConvarValue);
    ScriptEngine::RegisterNativeHandler("CREATE_CONVAR", CreateConVar);
    ScriptEngine::RegisterNativeHandler("DELETE_CONVAR", DeleteConVar);
})
} // namespace counterstrikesharp
