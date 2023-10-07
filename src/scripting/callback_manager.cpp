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

#include "scripting/callback_manager.h"
#include "core/log.h"
#include <algorithm>

namespace counterstrikesharp
{

ScriptCallback::ScriptCallback(const char *name) : m_root_context(fxNativeContext{})
{
    m_script_context_raw = ScriptContextRaw(m_root_context);
    m_name = std::string(name);
}

ScriptCallback::~ScriptCallback()
{
    m_functions.clear();
}

void ScriptCallback::AddListener(CallbackT plugin_function)
{
    m_functions.push_back(plugin_function);
}

bool ScriptCallback::RemoveListener(CallbackT pluginFunction)
{
    bool success;

    m_functions.erase(std::remove(m_functions.begin(), m_functions.end(), pluginFunction), m_functions.end());

    return success;
}

void ScriptCallback::Execute(bool resetContext)
{
    for (auto method_to_call : m_functions)
    {
        if (method_to_call)
        {
            method_to_call(&ScriptContextStruct());
        }
    }

    if (resetContext)
    {
        ResetContext();
    }
}

void ScriptCallback::ResetContext()
{
    ScriptContext().Reset();
}

CallbackManager::CallbackManager()
{
}

ScriptCallback *CallbackManager::CreateCallback(const char *name)
{
    auto *callback = new ScriptCallback(name);
    m_managed.push_back(callback);

    return callback;
}

ScriptCallback *CallbackManager::FindCallback(const char *name)
{
    for (auto it = m_managed.begin(); it != m_managed.end(); ++it)
    {
        ScriptCallback *marshal = *it;
        if (strcmp(marshal->GetName().c_str(), name) == 0)
        {
            return marshal;
        }
    }

    return nullptr;
}

void CallbackManager::ReleaseCallback(ScriptCallback *callback)
{
    bool success;
    auto it =
        std::remove_if(m_managed.begin(), m_managed.end(), [callback](ScriptCallback *i) { return callback == i; });

    if ((success = it != m_managed.end()))
        m_managed.erase(it, m_managed.end());
    delete callback;
}

bool CallbackManager::TryAddFunction(const char *name, CallbackT pCallable)
{
    auto *fwd = FindCallback(name);
    if (fwd)
    {
        fwd->AddListener(pCallable);
        return true;
    }

    return false;
}

bool CallbackManager::TryRemoveFunction(const char *name, CallbackT pCallable)
{
    auto *fwd = FindCallback(name);
    if (fwd)
    {
        bool success = fwd->RemoveListener(pCallable);
        return success;
    }

    return false;
}

void CallbackManager::PrintCallbackDebug()
{
    CSSHARP_CORE_INFO("----CALLBACKS----");
    for (auto it : m_managed)
    {
        CSSHARP_CORE_INFO("{0} ({0})\n", it->GetName().c_str(), 1);
    }
}
} // namespace counterstrikesharp