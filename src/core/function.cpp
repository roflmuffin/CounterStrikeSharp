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

namespace counterstrikesharp {

DCCallVM* g_pCallVM = dcNewCallVM(4096);

// ============================================================================
// >> GetDynCallConvention
// ============================================================================
int GetDynCallConvention(Convention_t eConv) {
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

ValveFunction::ValveFunction(void* ulAddr,
                             Convention_t callingConvention,
                             std::vector<DataType_t> args,
                             DataType_t returnType)
    : m_ulAddr(ulAddr) {
    m_Args = args;

    m_eReturnType = returnType;

    m_eCallingConvention = callingConvention;

    m_iCallingConvention = GetDynCallConvention(m_eCallingConvention);
}

ValveFunction::ValveFunction(void* ulAddr,
                             Convention_t callingConvention,
                             DataType_t* args,
                             int argCount,
                             DataType_t returnType)
    : m_ulAddr(ulAddr)

{
    m_Args = std::vector<DataType_t>(args, args + argCount);
    m_eReturnType = returnType;

    m_eCallingConvention = callingConvention;
    m_iCallingConvention = GetDynCallConvention(m_eCallingConvention);
}

ValveFunction::~ValveFunction() {}

bool ValveFunction::IsCallable() {
    return (m_eCallingConvention != CONV_CUSTOM) && (m_iCallingConvention != -1);
}

// bool ValveFunction::IsHookable() { return m_pCallingConvention != NULL; }
//
// bool ValveFunction::IsHooked() { return GetHookManager()->FindHook((void*)m_ulAddr) != NULL; }
//
// CHook* ValveFunction::GetHook() { return GetHookManager()->FindHook((void*)m_ulAddr); }

// ValveFunction* ValveFunction::GetTrampoline() {
//     CHook* pHook = GetHookManager()->FindHook((void*)m_ulAddr);
//     if (!pHook) return nullptr;
//
//     return new ValveFunction((unsigned long)pHook->m_pTrampoline, m_eCallingConvention, m_Args,
//                              m_eReturnType);
// }

template <class ReturnType, class Function>
ReturnType CallHelper(Function func, DCCallVM* vm, void* addr) {
    ReturnType result;
    result = (ReturnType)func(vm, (void*)addr);
    return result;
}

void CallHelperVoid(DCCallVM* vm, void* addr) { dcCallVoid(vm, (void*)addr); }

void ValveFunction::Call(ScriptContext& script_context, int offset) {
    if (!IsCallable()) return;

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
                dcArgLongLong(g_pCallVM,
                              script_context.GetArgument<unsigned long long>(contextIndex));
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
                dcArgPointer(g_pCallVM,
                             (void*)script_context.GetArgument<const char*>(contextIndex));
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
//
// CHook* HookFunctionHelper(void* addr, ICallingConvention* pConv) {
//    CHook* result;
//    result = GetHookManager()->HookFunction(addr, pConv);
//    return result;
//}
//
// void ValveFunction::DeleteHook() {
//    CHook* pHook = GetHookManager()->FindHook((void*)m_ulAddr);
//    if (!pHook) return;
//
//    // Set the calling convention to NULL, because DynamicHooks will delete it
//    // otherwise.
//    pHook->m_pCallingConvention = NULL;
//    GetHookManager()->UnhookFunction((void*)m_ulAddr);
//}
//
// CHook* ValveFunction::AddHook(HookType_t eType, void* callable) {
//    if (!IsHookable()) return nullptr;
//
//    CHook* pHook = GetHookManager()->FindHook((void*)m_ulAddr);
//
//    if (!pHook) {
//        pHook = HookFunctionHelper((void*)m_ulAddr, m_pCallingConvention);
//
//        // DynamicHooks will handle our convention from there, regardless if we
//        // allocated it or not.
//        m_bAllocatedCallingConvention = false;
//    }
//
//    // Add the hook handler. If it's already added, it won't be added twice
//    pHook->AddCallback(eType, (HookHandlerFn*)(void*)callable);
//
//    return pHook;
//}

}  // namespace counterstrikesharp