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

namespace counterstrikesharp {

ScriptCallback::ScriptCallback(const char* szName) : m_root_context(fxNativeContext{})
{
    m_script_context_raw = ScriptContextRaw(m_root_context);
    m_name = std::string(szName);
}

ScriptCallback::~ScriptCallback() { m_functions.clear(); }

void ScriptCallback::AddListener(CallbackT fnPluginFunction)
{
    m_functions.push_back(fnPluginFunction);
}

bool ScriptCallback::RemoveListener(CallbackT fnPluginFunction)
{
    bool bSuccess;

    m_functions.erase(std::remove(m_functions.begin(), m_functions.end(), fnPluginFunction),
                      m_functions.end());

    return bSuccess;
}

void ScriptCallback::Execute(bool bResetContext)
{
    for (auto fnMethodToCall : m_functions) {
        if (fnMethodToCall) {
            fnMethodToCall(&ScriptContextStruct());
        }
    }

    if (bResetContext) {
        Reset();
    }
}

void ScriptCallback::Reset() { ScriptContext().Reset(); }

CallbackManager::CallbackManager() = default;

ScriptCallback* CallbackManager::CreateCallback(const char* szName)
{
    CSSHARP_CORE_TRACE("Creating callback {0}", szName);
    auto* pCallback = new ScriptCallback(szName);
    m_managed.push_back(pCallback);

    return pCallback;
}

ScriptCallback* CallbackManager::FindCallback(const char* szName)
{
    for (auto* pMarshal : m_managed) {
        if (strcmp(pMarshal->GetName().c_str(), szName) == 0) {
            return pMarshal;
        }
    }

    return nullptr;
}

void CallbackManager::ReleaseCallback(ScriptCallback* pCallback)
{
    auto I = std::remove_if(m_managed.begin(), m_managed.end(),
                            [pCallback](ScriptCallback* pI) { return pCallback == pI; });

    if (I != m_managed.end())
        m_managed.erase(I, m_managed.end());
    delete pCallback;
}

bool CallbackManager::TryAddFunction(const char* szName, CallbackT fnCallable)
{
    auto* pCallback = FindCallback(szName);
    if (pCallback) {
        pCallback->AddListener(fnCallable);
        return true;
    }

    return false;
}

bool CallbackManager::TryRemoveFunction(const char* szName, CallbackT fnCallable)
{
    auto* pCallback = FindCallback(szName);
    if (pCallback) {
        return pCallback->RemoveListener(fnCallable);
    }

    return false;
}

void CallbackManager::PrintCallbackDebug()
{
    CSSHARP_CORE_INFO("----CALLBACKS----");
    for (auto* pCallback : m_managed) {
        CSSHARP_CORE_INFO("{0} ({0})\n", pCallback->GetName().c_str(), 1);
    }
}
} // namespace counterstrikesharp