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

#include "mm_plugin.h"
#include "core/timer_system.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/function.h"
#include "pch.h"
#include "dynohook/core.h"
#include "dynohook/manager.h"

namespace counterstrikesharp {

enum class DHookRegister : int {
    EAX_RAX = 0,
    EBX_RBX = 1,
    ECX_RCX = 2,
    EDX_RDX = 3,
    ESI_RSI = 4,
    EDI_RDI = 5,
    EBP_RBP = 6,
    ESP_RSP = 7,

    R8 = 8,
    R9 = 9,
    R10 = 10,
    R11 = 11,
    R12 = 12,
    R13 = 13,
    R14 = 14,
    R15 = 15,

    XMM0 = 16,
    XMM1 = 17,
    XMM2 = 18,
    XMM3 = 19,
    XMM4 = 20,
    XMM5 = 21,
    XMM6 = 22,
    XMM7 = 23,
    XMM8 = 24,
    XMM9 = 25,
    XMM10 = 26,
    XMM11 = 27,
    XMM12 = 28,
    XMM13 = 29,
    XMM14 = 30,
    XMM15 = 31
};

void DHookGetRegister(ScriptContext& script_context)
{
    auto hook = script_context.GetArgument<dyno::Hook*>(0);
    auto registerIdInt = script_context.GetArgument<int>(1);
    auto dataType = script_context.GetArgument<DataType_t>(2);

    if (hook == nullptr)
    {
        script_context.ThrowNativeError("Invalid hook object provided to DHookGetRegister.");
        return;
    }

    const dyno::Registers& regs = hook->getRegisters();
    auto requestedRegId = static_cast<DHookRegister>(registerIdInt);
    dyno::RegisterType dynoRegType = dyno::RegisterType::NONE;

    switch (requestedRegId)
    {
        case DHookRegister::EAX_RAX:
            dynoRegType = dyno::RegisterType::RAX;
            break;
        case DHookRegister::EBX_RBX:
            dynoRegType = dyno::RegisterType::RBX;
            break;
        case DHookRegister::ECX_RCX:
            dynoRegType = dyno::RegisterType::RCX;
            break;
        case DHookRegister::EDX_RDX:
            dynoRegType = dyno::RegisterType::RDX;
            break;
        case DHookRegister::ESI_RSI:
            dynoRegType = dyno::RegisterType::RSI;
            break;
        case DHookRegister::EDI_RDI:
            dynoRegType = dyno::RegisterType::RDI;
            break;
        case DHookRegister::EBP_RBP:
            dynoRegType = dyno::RegisterType::RBP;
            break;
        case DHookRegister::ESP_RSP:
            dynoRegType = dyno::RegisterType::RSP;
            break;
        case DHookRegister::R8:
            dynoRegType = dyno::RegisterType::R8;
            break;
        case DHookRegister::R9:
            dynoRegType = dyno::RegisterType::R9;
            break;
        case DHookRegister::R10:
            dynoRegType = dyno::RegisterType::R10;
            break;
        case DHookRegister::R11:
            dynoRegType = dyno::RegisterType::R11;
            break;
        case DHookRegister::R12:
            dynoRegType = dyno::RegisterType::R12;
            break;
        case DHookRegister::R13:
            dynoRegType = dyno::RegisterType::R13;
            break;
        case DHookRegister::R14:
            dynoRegType = dyno::RegisterType::R14;
            break;
        case DHookRegister::R15:
            dynoRegType = dyno::RegisterType::R15;
            break;
        case DHookRegister::XMM0:
            dynoRegType = dyno::RegisterType::XMM0;
            break;
        case DHookRegister::XMM1:
            dynoRegType = dyno::RegisterType::XMM1;
            break;
        case DHookRegister::XMM2:
            dynoRegType = dyno::RegisterType::XMM2;
            break;
        case DHookRegister::XMM3:
            dynoRegType = dyno::RegisterType::XMM3;
            break;
        case DHookRegister::XMM4:
            dynoRegType = dyno::RegisterType::XMM4;
            break;
        case DHookRegister::XMM5:
            dynoRegType = dyno::RegisterType::XMM5;
            break;
        case DHookRegister::XMM6:
            dynoRegType = dyno::RegisterType::XMM6;
            break;
        case DHookRegister::XMM7:
            dynoRegType = dyno::RegisterType::XMM7;
            break;
        case DHookRegister::XMM8:
            dynoRegType = dyno::RegisterType::XMM8;
            break;
        case DHookRegister::XMM9:
            dynoRegType = dyno::RegisterType::XMM9;
            break;
        case DHookRegister::XMM10:
            dynoRegType = dyno::RegisterType::XMM10;
            break;
        case DHookRegister::XMM11:
            dynoRegType = dyno::RegisterType::XMM11;
            break;
        case DHookRegister::XMM12:
            dynoRegType = dyno::RegisterType::XMM12;
            break;
        case DHookRegister::XMM13:
            dynoRegType = dyno::RegisterType::XMM13;
            break;
        case DHookRegister::XMM14:
            dynoRegType = dyno::RegisterType::XMM14;
            break;
        case DHookRegister::XMM15:
            dynoRegType = dyno::RegisterType::XMM15;
            break;
        default:
            script_context.ThrowNativeError("Unsupported or invalid register ID %d for x64.", static_cast<int>(requestedRegId));
            return;
    }

    if (dynoRegType == dyno::RegisterType::NONE)
    {
        script_context.ThrowNativeError("Failed to map DHookRegister to dyno::RegisterType.");
        return;
    }

    const dyno::Register& reg = regs.at(dynoRegType);

    if (reg.getType() == dyno::RegisterType::NONE)
    {
        script_context.ThrowNativeError("Register ID %d (DynoType %d) is not available or not captured in the current hook context.", static_cast<int>(requestedRegId), static_cast<int>(dynoRegType));
        return;
    }

    if (dataType == DATA_TYPE_M128A_POINTER)
    {
        if (dynoRegType >= dyno::RegisterType::XMM0 && dynoRegType <= dyno::RegisterType::XMM15)
        {
            void* xmm_data_ptr = *reg;
            if (xmm_data_ptr == nullptr)
            {
                script_context.ThrowNativeError("Internal XMM register data pointer is null for register ID %d.", static_cast<int>(requestedRegId));
                return;
            }
            script_context.SetResult(xmm_data_ptr);
        }
        else
        {
            script_context.ThrowNativeError("DATA_TYPE_M128A_POINTER is only valid for XMM registers (XMM0-XMM15). Requested DHookRegister ID: %d, mapped DynoType: %d", static_cast<int>(requestedRegId), static_cast<int>(dynoRegType));
        }
    }
    else
    {
        uintptr_t regValue = reg.getValue<uintptr_t>();
        switch (dataType)
        {
            case DATA_TYPE_BOOL:
                script_context.SetResult(static_cast<bool>(regValue));
                break;
            case DATA_TYPE_CHAR:
                script_context.SetResult(static_cast<char>(regValue));
                break;
            case DATA_TYPE_UCHAR:
                script_context.SetResult(static_cast<unsigned char>(regValue));
                break;
            case DATA_TYPE_SHORT:
                script_context.SetResult(static_cast<short>(regValue));
                break;
            case DATA_TYPE_USHORT:
                script_context.SetResult(static_cast<unsigned short>(regValue));
                break;
            case DATA_TYPE_INT:
                script_context.SetResult(static_cast<int>(regValue));
                break;
            case DATA_TYPE_UINT:
                script_context.SetResult(static_cast<unsigned int>(regValue));
                break;
            case DATA_TYPE_LONG:
                script_context.SetResult(static_cast<long>(regValue));
                break;
            case DATA_TYPE_ULONG:
                script_context.SetResult(static_cast<unsigned long>(regValue));
                break;
            case DATA_TYPE_LONG_LONG:
                script_context.SetResult(static_cast<long long>(regValue));
                break;
            case DATA_TYPE_ULONG_LONG:
                script_context.SetResult(static_cast<unsigned long long>(regValue));
                break;
            case DATA_TYPE_FLOAT:
                uint32_t val32 = static_cast<uint32_t>(regValue);
                script_context.SetResult(*reinterpret_cast<float*>(&val32));
                break;
            case DATA_TYPE_STRING:
                script_context.SetResult(reinterpret_cast<const char*>(regValue));
                break;
            case DATA_TYPE_POINTER:
                script_context.SetResult(reinterpret_cast<void*>(regValue));
                break;
            default:
                script_context.ThrowNativeError("Unsupported data type %d in DHookGetRegister.", static_cast<int>(dataType));
                break;
        }
    }
}

void DHookGetReturn(ScriptContext& script_context)
{
    auto hook = script_context.GetArgument<dyno::Hook*>(0);
    auto dataType = script_context.GetArgument<DataType_t>(1);
    if (hook == nullptr)
    {
        script_context.ThrowNativeError("Invalid hook");
    }

    switch (dataType)
    {
        case DATA_TYPE_BOOL:
            script_context.SetResult(hook->getReturnValue<bool>());
            break;
        case DATA_TYPE_CHAR:
            script_context.SetResult(hook->getReturnValue<char>());
            break;
        case DATA_TYPE_UCHAR:
            script_context.SetResult(hook->getReturnValue<unsigned char>());
            break;
        case DATA_TYPE_SHORT:
            script_context.SetResult(hook->getReturnValue<short>());
            break;
        case DATA_TYPE_USHORT:
            script_context.SetResult(hook->getReturnValue<unsigned short>());
            break;
        case DATA_TYPE_INT:
            script_context.SetResult(hook->getReturnValue<int>());
            break;
        case DATA_TYPE_UINT:
            script_context.SetResult(hook->getReturnValue<unsigned int>());
            break;
        case DATA_TYPE_LONG:
            script_context.SetResult(hook->getReturnValue<long>());
            break;
        case DATA_TYPE_ULONG:
            script_context.SetResult(hook->getReturnValue<unsigned long>());
            break;
        case DATA_TYPE_LONG_LONG:
            script_context.SetResult(hook->getReturnValue<long long>());
            break;
        case DATA_TYPE_ULONG_LONG:
            script_context.SetResult(hook->getReturnValue<unsigned long long>());
            break;
        case DATA_TYPE_FLOAT:
            script_context.SetResult(hook->getReturnValue<float>());
            break;
        case DATA_TYPE_DOUBLE:
            script_context.SetResult(hook->getReturnValue<double>());
            break;
        case DATA_TYPE_POINTER:
            script_context.SetResult(hook->getReturnValue<void*>());
            break;
        case DATA_TYPE_STRING:
            script_context.SetResult(hook->getReturnValue<const char*>());
            break;
        default:
            assert(!"Unknown function parameter type!");
            break;
    }
}

void DHookSetReturn(ScriptContext& script_context)
{
    auto hook = script_context.GetArgument<dyno::Hook*>(0);
    auto dataType = script_context.GetArgument<DataType_t>(1);
    if (hook == nullptr)
    {
        script_context.ThrowNativeError("Invalid hook");
    }

    auto valueIndex = 2;

    switch (dataType)
    {
        case DATA_TYPE_BOOL:
            hook->setReturnValue(script_context.GetArgument<bool>(valueIndex));
            break;
        case DATA_TYPE_CHAR:
            hook->setReturnValue(script_context.GetArgument<char>(valueIndex));
            break;
        case DATA_TYPE_UCHAR:
            hook->setReturnValue(script_context.GetArgument<unsigned char>(valueIndex));
            break;
        case DATA_TYPE_SHORT:
            hook->setReturnValue(script_context.GetArgument<short>(valueIndex));
            break;
        case DATA_TYPE_USHORT:
            hook->setReturnValue(script_context.GetArgument<unsigned short>(valueIndex));
            break;
        case DATA_TYPE_INT:
            hook->setReturnValue(script_context.GetArgument<int>(valueIndex));
            break;
        case DATA_TYPE_UINT:
            hook->setReturnValue(script_context.GetArgument<unsigned int>(valueIndex));
            break;
        case DATA_TYPE_LONG:
            hook->setReturnValue(script_context.GetArgument<long>(valueIndex));
            break;
        case DATA_TYPE_ULONG:
            hook->setReturnValue(script_context.GetArgument<unsigned long>(valueIndex));
            break;
        case DATA_TYPE_LONG_LONG:
            hook->setReturnValue(script_context.GetArgument<long long>(valueIndex));
            break;
        case DATA_TYPE_ULONG_LONG:
            hook->setReturnValue(script_context.GetArgument<unsigned long long>(valueIndex));
            break;
        case DATA_TYPE_FLOAT:
            hook->setReturnValue(script_context.GetArgument<float>(valueIndex));
            break;
        case DATA_TYPE_DOUBLE:
            hook->setReturnValue(script_context.GetArgument<double>(valueIndex));
            break;
        case DATA_TYPE_POINTER:
            hook->setReturnValue(script_context.GetArgument<void*>(valueIndex));
            break;
        case DATA_TYPE_STRING:
            hook->setReturnValue(script_context.GetArgument<const char*>(valueIndex));
            break;
        default:
            assert(!"Unknown function parameter type!");
            break;
    }
}

void DHookGetParam(ScriptContext& script_context)
{
    auto hook = script_context.GetArgument<dyno::Hook*>(0);
    auto dataType = script_context.GetArgument<DataType_t>(1);
    auto paramIndex = script_context.GetArgument<int>(2);
    if (hook == nullptr)
    {
        script_context.ThrowNativeError("Invalid hook");
    }

    switch (dataType)
    {
        case DATA_TYPE_BOOL:
            script_context.SetResult(hook->getArgument<bool>(paramIndex));
            break;
        case DATA_TYPE_CHAR:
            script_context.SetResult(hook->getArgument<char>(paramIndex));
            break;
        case DATA_TYPE_UCHAR:
            script_context.SetResult(hook->getArgument<unsigned char>(paramIndex));
            break;
        case DATA_TYPE_SHORT:
            script_context.SetResult(hook->getArgument<short>(paramIndex));
            break;
        case DATA_TYPE_USHORT:
            script_context.SetResult(hook->getArgument<unsigned short>(paramIndex));
            break;
        case DATA_TYPE_INT:
            script_context.SetResult(hook->getArgument<int>(paramIndex));
            break;
        case DATA_TYPE_UINT:
            script_context.SetResult(hook->getArgument<unsigned int>(paramIndex));
            break;
        case DATA_TYPE_LONG:
            script_context.SetResult(hook->getArgument<long>(paramIndex));
            break;
        case DATA_TYPE_ULONG:
            script_context.SetResult(hook->getArgument<unsigned long>(paramIndex));
            break;
        case DATA_TYPE_LONG_LONG:
            script_context.SetResult(hook->getArgument<long long>(paramIndex));
            break;
        case DATA_TYPE_ULONG_LONG:
            script_context.SetResult(hook->getArgument<unsigned long long>(paramIndex));
            break;
        case DATA_TYPE_FLOAT:
            script_context.SetResult(hook->getArgument<float>(paramIndex));
            break;
        case DATA_TYPE_DOUBLE:
            script_context.SetResult(hook->getArgument<double>(paramIndex));
            break;
        case DATA_TYPE_POINTER:
            script_context.SetResult(hook->getArgument<void*>(paramIndex));
            break;
        case DATA_TYPE_STRING:
            script_context.SetResult(hook->getArgument<const char*>(paramIndex));
            break;
        default:
            assert(!"Unknown function parameter type!");
            break;
    }
}

void DHookSetParam(ScriptContext& script_context)
{
    auto hook = script_context.GetArgument<dyno::Hook*>(0);
    auto dataType = script_context.GetArgument<DataType_t>(1);
    auto paramIndex = script_context.GetArgument<int>(2);
    if (hook == nullptr)
    {
        script_context.ThrowNativeError("Invalid hook");
    }

    auto valueIndex = 3;

    switch (dataType)
    {
        case DATA_TYPE_BOOL:
            hook->setArgument(paramIndex, script_context.GetArgument<bool>(valueIndex));
            break;
        case DATA_TYPE_CHAR:
            hook->setArgument(paramIndex, script_context.GetArgument<char>(valueIndex));
            break;
        case DATA_TYPE_UCHAR:
            hook->setArgument(paramIndex, script_context.GetArgument<unsigned char>(valueIndex));
            break;
        case DATA_TYPE_SHORT:
            hook->setArgument(paramIndex, script_context.GetArgument<short>(valueIndex));
            break;
        case DATA_TYPE_USHORT:
            hook->setArgument(paramIndex, script_context.GetArgument<unsigned short>(valueIndex));
            break;
        case DATA_TYPE_INT:
            hook->setArgument(paramIndex, script_context.GetArgument<int>(valueIndex));
            break;
        case DATA_TYPE_UINT:
            hook->setArgument(paramIndex, script_context.GetArgument<unsigned int>(valueIndex));
            break;
        case DATA_TYPE_LONG:
            hook->setArgument(paramIndex, script_context.GetArgument<long>(valueIndex));
            break;
        case DATA_TYPE_ULONG:
            hook->setArgument(paramIndex, script_context.GetArgument<unsigned long>(valueIndex));
            break;
        case DATA_TYPE_LONG_LONG:
            hook->setArgument(paramIndex, script_context.GetArgument<long long>(valueIndex));
            break;
        case DATA_TYPE_ULONG_LONG:
            hook->setArgument(paramIndex, script_context.GetArgument<unsigned long long>(valueIndex));
            break;
        case DATA_TYPE_FLOAT:
            hook->setArgument(paramIndex, script_context.GetArgument<float>(valueIndex));
            break;
        case DATA_TYPE_DOUBLE:
            hook->setArgument(paramIndex, script_context.GetArgument<double>(valueIndex));
            break;
        case DATA_TYPE_POINTER:
            hook->setArgument(paramIndex, script_context.GetArgument<void*>(valueIndex));
            break;
        case DATA_TYPE_STRING:
            hook->setArgument(paramIndex, script_context.GetArgument<const char*>(valueIndex));
            break;
        default:
            assert(!"Unknown function parameter type!");
            break;
    }
}

REGISTER_NATIVES(dynamichooks, {
    ScriptEngine::RegisterNativeHandler("DYNAMIC_HOOK_GET_REGISTER", DHookGetRegister);
    ScriptEngine::RegisterNativeHandler("DYNAMIC_HOOK_GET_RETURN", DHookGetReturn);
    ScriptEngine::RegisterNativeHandler("DYNAMIC_HOOK_SET_RETURN", DHookSetReturn);
    ScriptEngine::RegisterNativeHandler("DYNAMIC_HOOK_GET_PARAM", DHookGetParam);
    ScriptEngine::RegisterNativeHandler("DYNAMIC_HOOK_SET_PARAM", DHookSetParam);
})
} // namespace counterstrikesharp
