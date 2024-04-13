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
#include "core/function.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"

namespace counterstrikesharp {
std::vector<ValveFunction*> m_managed_ptrs;

void* FindSignatureNative(ScriptContext& scriptContext)
{
    auto moduleName = scriptContext.GetArgument<const char*>(0);
    auto bytesStr = scriptContext.GetArgument<const char*>(1);

    return FindSignature(moduleName, bytesStr);
}

ValveFunction* CreateVirtualFunctionBySignature(ScriptContext& script_context)
{
    auto ptr = script_context.GetArgument<unsigned long>(0);
    auto binary_name = script_context.GetArgument<const char*>(1);
    auto signature_hex_string = script_context.GetArgument<const char*>(2);
    auto num_arguments = script_context.GetArgument<int>(3);
    auto return_type = script_context.GetArgument<DataType_t>(4);

    auto* function_addr = FindSignature(binary_name, signature_hex_string);

    if (function_addr == nullptr) {
        script_context.ThrowNativeError("Could not find signature %s", signature_hex_string);
        return nullptr;
    }

    auto args = std::vector<DataType_t>();
    for (int i = 0; i < num_arguments; i++) {
        args.push_back(script_context.GetArgument<DataType_t>(5 + i));
    }

    auto function = new ValveFunction(function_addr, CONV_CDECL, args, return_type);
    function->SetSignature(signature_hex_string);

    CSSHARP_CORE_TRACE("Created virtual function, pointer found at {}, signature {}", function_addr,
                       signature_hex_string);

    m_managed_ptrs.push_back(function);
    return function;
}

ValveFunction* CreateVirtualFunction(ScriptContext& script_context)
{
    auto ptr = script_context.GetArgument<void*>(0);
    auto vtable_offset = script_context.GetArgument<int>(1);
    auto num_arguments = script_context.GetArgument<int>(2);
    auto return_type = script_context.GetArgument<DataType_t>(3);

    void** vtable = *(void***)ptr;
    if (!vtable) {
        script_context.ThrowNativeError("Failed to get the virtual function table.");
        return nullptr;
    }

    auto function_addr = (void*)vtable[vtable_offset];

    auto args = std::vector<DataType_t>();
    for (int i = 0; i < num_arguments; i++) {
        args.push_back(script_context.GetArgument<DataType_t>(4 + i));
    }

    auto function = new ValveFunction(function_addr, CONV_THISCALL, args, return_type);
    function->SetOffset(vtable_offset);

    m_managed_ptrs.push_back(function);
    return function;
}

void HookFunction(ScriptContext& script_context)
{
    auto function = script_context.GetArgument<ValveFunction*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    if (!function) {
        script_context.ThrowNativeError("Invalid function pointer");
        return;
    }

    function->AddHook(callback, post);
}

void UnhookFunction(ScriptContext& script_context)
{
    auto function = script_context.GetArgument<ValveFunction*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    if (!function) {
        script_context.ThrowNativeError("Invalid function pointer");
        return;
    }

    function->RemoveHook(callback, post);
}

void ExecuteVirtualFunction(ScriptContext& script_context)
{
    auto function = script_context.GetArgument<ValveFunction*>(0);

    if (!function) {
        script_context.ThrowNativeError("Invalid function pointer");
        return;
    }

    function->Call(script_context, 1);
}

int GetNetworkVectorSize(ScriptContext& script_context)
{
    auto vec = script_context.GetArgument<CUtlVector<void*>*>(0);

    return vec->Count();
}

void* GetNetworkVectorElementAt(ScriptContext& script_context)
{
    auto vec = script_context.GetArgument<CUtlVector<CEntityHandle>*>(0);
    auto index = script_context.GetArgument<int>(1);

    return &vec->Element(index);
}

void RemoveAllNetworkVectorElements(ScriptContext& script_context)
{
    auto vec = script_context.GetArgument<CUtlVector<CEntityHandle>*>(0);

    vec->RemoveAll();
}

REGISTER_NATIVES(memory, {
    ScriptEngine::RegisterNativeHandler("CREATE_VIRTUAL_FUNCTION", CreateVirtualFunction);
    ScriptEngine::RegisterNativeHandler("CREATE_VIRTUAL_FUNCTION_BY_SIGNATURE",
                                        CreateVirtualFunctionBySignature);
    ScriptEngine::RegisterNativeHandler("EXECUTE_VIRTUAL_FUNCTION", ExecuteVirtualFunction);
    ScriptEngine::RegisterNativeHandler("HOOK_FUNCTION", HookFunction);
    ScriptEngine::RegisterNativeHandler("UNHOOK_FUNCTION", UnhookFunction);
    ScriptEngine::RegisterNativeHandler("FIND_SIGNATURE", FindSignatureNative);
    ScriptEngine::RegisterNativeHandler("GET_NETWORK_VECTOR_SIZE", GetNetworkVectorSize);
    ScriptEngine::RegisterNativeHandler("GET_NETWORK_VECTOR_ELEMENT_AT", GetNetworkVectorElementAt);
    ScriptEngine::RegisterNativeHandler("REMOVE_ALL_NETWORK_VECTOR_ELEMENTS", RemoveAllNetworkVectorElements);
})
} // namespace counterstrikesharp
