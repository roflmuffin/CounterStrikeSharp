/**
 * =============================================================================
 * Source Python
 * Copyright (C) 2012-2015 Source Python Development Team.  All rights reserved.
 * =============================================================================
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * As a special exception, the Source Python Team gives you permission
 * to link the code of this program (as well as its derivative works) to
 * "Half-Life 2," the "Source Engine," and any Game MODs that run on software
 * by the Valve Corporation.  You must obey the GNU General Public License in
 * all respects for all other code used.  Additionally, the Source.Python
 * Development Team grants this exception to all derivative works.
 *
 * This file has been modified from its original form, under the terms of GNU
 * General Public License, version 3.0.
 */

#include "core/function.h"

#include "core/log.h"
#include "dyncall/dyncall/dyncall.h"

#include "pch.h"
#include "dynohook/core.h"
#include "dynohook/manager.h"
#include "dynohook/conventions/x64/x64SystemVcall.h"

namespace counterstrikesharp {

DCCallVM* g_pCallVM = dcNewCallVM(4096);
std::map<dyno::Hook*, ValveFunction*> g_HookMap;

// ============================================================================
// >> GetDynCallConvention
// ============================================================================
int GetDynCallConvention(Convention_t eConv)
{
    switch (eConv) {
    case CONV_CUSTOM:
        return -1;
    case CONV_CDECL:
        return DC_CALL_C_DEFAULT;
    case CONV_THISCALL:
#ifdef _WIN32
        return DC_CALL_C_X86_WIN32_THIS_MS;
#else
        return DC_CALL_C_X86_WIN32_THIS_GNU;
#endif
#ifdef _WIN32
    case CONV_STDCALL:
        return DC_CALL_C_X86_WIN32_STD;
    case CONV_FASTCALL:
        return DC_CALL_C_X86_WIN32_FAST_MS;
#endif
    }

    return -1;
}

ValveFunction::ValveFunction(void* ulAddr, Convention_t callingConvention,
                             std::vector<DataType_t> args, DataType_t returnType)
    : m_ulAddr(ulAddr)
{
    m_Args = args;

    m_eReturnType = returnType;

    m_eCallingConvention = callingConvention;

    m_iCallingConvention = GetDynCallConvention(m_eCallingConvention);
}

ValveFunction::ValveFunction(void* ulAddr, Convention_t callingConvention, DataType_t* args,
                             int argCount, DataType_t returnType)
    : m_ulAddr(ulAddr)

{
    m_Args = std::vector<DataType_t>(args, args + argCount);
    m_eReturnType = returnType;

    m_eCallingConvention = callingConvention;
    m_iCallingConvention = GetDynCallConvention(m_eCallingConvention);
}

ValveFunction::~ValveFunction() {}

bool ValveFunction::IsCallable()
{
    return (m_eCallingConvention != CONV_CUSTOM) && (m_iCallingConvention != -1);
}

template <class ReturnType, class Function>
ReturnType CallHelper(Function func, DCCallVM* vm, void* addr)
{
    ReturnType result;
    result = (ReturnType)func(vm, (void*)addr);
    return result;
}

void CallHelperVoid(DCCallVM* vm, void* addr) { dcCallVoid(vm, (void*)addr); }

void ValveFunction::Call(ScriptContext& script_context, int offset)
{
    if (!IsCallable())
        return;

    dcReset(g_pCallVM);
    dcMode(g_pCallVM, m_iCallingConvention);

    for (size_t i = 0; i < m_Args.size(); i++) {
        int contextIndex = i + offset;
        switch (m_Args[i]) {
        case DATA_TYPE_BOOL:
            dcArgBool(g_pCallVM, script_context.GetArgument<bool>(contextIndex));
            break;
        case DATA_TYPE_CHAR:
            dcArgChar(g_pCallVM, script_context.GetArgument<char>(contextIndex));
            break;
        case DATA_TYPE_UCHAR:
            dcArgChar(g_pCallVM, script_context.GetArgument<unsigned char>(contextIndex));
            break;
        case DATA_TYPE_SHORT:
            dcArgShort(g_pCallVM, script_context.GetArgument<short>(contextIndex));
            break;
        case DATA_TYPE_USHORT:
            dcArgShort(g_pCallVM, script_context.GetArgument<unsigned short>(contextIndex));
            break;
        case DATA_TYPE_INT:
            dcArgInt(g_pCallVM, script_context.GetArgument<int>(contextIndex));
            break;
        case DATA_TYPE_UINT:
            dcArgInt(g_pCallVM, script_context.GetArgument<unsigned int>(contextIndex));
            break;
        case DATA_TYPE_LONG:
            dcArgLong(g_pCallVM, script_context.GetArgument<long>(contextIndex));
            break;
        case DATA_TYPE_ULONG:
            dcArgLong(g_pCallVM, script_context.GetArgument<unsigned long>(contextIndex));
            break;
        case DATA_TYPE_LONG_LONG:
            dcArgLongLong(g_pCallVM, script_context.GetArgument<long long>(contextIndex));
            break;
        case DATA_TYPE_ULONG_LONG:
            dcArgLongLong(g_pCallVM, script_context.GetArgument<unsigned long long>(contextIndex));
            break;
        case DATA_TYPE_FLOAT:
            dcArgFloat(g_pCallVM, script_context.GetArgument<float>(contextIndex));
            break;
        case DATA_TYPE_DOUBLE:
            dcArgDouble(g_pCallVM, script_context.GetArgument<double>(contextIndex));
            break;
        case DATA_TYPE_POINTER:
            dcArgPointer(g_pCallVM, script_context.GetArgument<void*>(contextIndex));
            break;
        case DATA_TYPE_STRING:
            dcArgPointer(g_pCallVM, (void*)script_context.GetArgument<const char*>(contextIndex));
            break;
        default:
            assert(!"Unknown function parameter type!");
            break;
        }
    }

    switch (m_eReturnType) {
    case DATA_TYPE_VOID:
        CallHelperVoid(g_pCallVM, m_ulAddr);
        break;
    case DATA_TYPE_BOOL:
        script_context.SetResult(CallHelper<bool>(dcCallBool, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_CHAR:
        script_context.SetResult(CallHelper<char>(dcCallChar, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_UCHAR:
        script_context.SetResult(CallHelper<unsigned char>(dcCallChar, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_SHORT:
        script_context.SetResult(CallHelper<short>(dcCallShort, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_USHORT:
        script_context.SetResult(CallHelper<unsigned short>(dcCallShort, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_INT:
        script_context.SetResult(CallHelper<int>(dcCallInt, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_UINT:
        script_context.SetResult(CallHelper<unsigned int>(dcCallInt, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_LONG:
        script_context.SetResult(CallHelper<long>(dcCallLong, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_ULONG:
        script_context.SetResult(CallHelper<unsigned long>(dcCallLong, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_LONG_LONG:
        script_context.SetResult(CallHelper<long long>(dcCallLongLong, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_ULONG_LONG:
        script_context.SetResult(
            CallHelper<unsigned long long>(dcCallLongLong, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_FLOAT:
        script_context.SetResult(CallHelper<float>(dcCallFloat, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_DOUBLE:
        script_context.SetResult(CallHelper<double>(dcCallDouble, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_POINTER:
        script_context.SetResult(CallHelper<void*>(dcCallPointer, g_pCallVM, m_ulAddr));
        break;
    case DATA_TYPE_STRING:
        script_context.SetResult(CallHelper<const char*>(dcCallPointer, g_pCallVM, m_ulAddr));
        break;
    default:
        assert(!"Unknown function return type!");
        break;
    }
}

dyno::ReturnAction HookHandler(dyno::HookType hookType, dyno::Hook& hook)
{
    auto vf = g_HookMap[&hook];

    auto callback = hookType == dyno::HookType::Pre ? vf->m_precallback : vf->m_postcallback;

    if (callback == nullptr) {
        return dyno::ReturnAction::Ignored;
    }

    callback->Reset();
    callback->ScriptContext().Push(&hook);

    for (auto fnMethodToCall : callback->GetFunctions()) {
        if (!fnMethodToCall)
            continue;
        fnMethodToCall(&callback->ScriptContextStruct());

        auto result = callback->ScriptContext().GetResult<HookResult>();
        CSSHARP_CORE_TRACE("Received hook callback result of {}, hook mode {}", result,
                          (int)hookType);

        if (result >= HookResult::Handled) {
            return dyno::ReturnAction::Supercede;
        }
    }

    return dyno::ReturnAction::Ignored;
}

std::vector<dyno::DataObject> ConvertArgsToDynoHook(const std::vector<DataType_t>& dataTypes)
{
    std::vector<dyno::DataObject> converted;
    converted.reserve(dataTypes.size());

    for (DataType_t dt : dataTypes) {
        converted.push_back(dyno::DataObject(static_cast<dyno::DataType>(dt)));
    }

    return converted;
}

void ValveFunction::AddHook(CallbackT callable, bool post)
{
    dyno::HookManager& manager = dyno::HookManager::Get();
    dyno::Hook* hook = manager.hook((void*)m_ulAddr, [this] {
        return new dyno::x64SystemVcall(ConvertArgsToDynoHook(m_Args),
                                        static_cast<dyno::DataType>(this->m_eReturnType));
    });
    g_HookMap[hook] = this;
    hook->addCallback(dyno::HookType::Post, (dyno::HookHandler*)&HookHandler);
    hook->addCallback(dyno::HookType::Pre, (dyno::HookHandler*)&HookHandler);

    if (post) {
        if (m_postcallback == nullptr) {
            m_postcallback = globals::callbackManager.CreateCallback("");
        }
        m_postcallback->AddListener(callable);
    } else {
        if (m_precallback == nullptr) {
            m_precallback = globals::callbackManager.CreateCallback("");
        }
        m_precallback->AddListener(callable);
    }
}
void ValveFunction::RemoveHook(CallbackT callable, bool post) {
    dyno::HookManager& manager = dyno::HookManager::Get();
    dyno::Hook* hook = manager.hook((void*)m_ulAddr, [this] {
        return new dyno::x64SystemVcall(ConvertArgsToDynoHook(m_Args),
                                        static_cast<dyno::DataType>(this->m_eReturnType));
    });
    g_HookMap[hook] = this;

    if (post) {
        if (m_postcallback != nullptr) {
            m_postcallback->RemoveListener(callable);
        }
    } else {
        if (m_precallback != nullptr) {
            m_precallback->RemoveListener(callable);
        }
    }
}

} // namespace counterstrikesharp