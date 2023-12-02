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

#include "core/managers/entity_manager.h"
#include "core/gameconfig.h"
#include "core/log.h"

#include <funchook.h>

#include <public/eiface.h>
#include "scripting/callback_manager.h"

namespace counterstrikesharp {

EntityManager::EntityManager() {}

EntityManager::~EntityManager() {}

void EntityManager::OnAllInitialized() {
    on_entity_spawned_callback = globals::callbackManager.CreateCallback("OnEntitySpawned");
    on_entity_created_callback = globals::callbackManager.CreateCallback("OnEntityCreated");
    on_entity_deleted_callback = globals::callbackManager.CreateCallback("OnEntityDeleted");
    on_entity_parent_changed_callback = globals::callbackManager.CreateCallback("OnEntityParentChanged");

    m_pFireOutputInternal = reinterpret_cast<FireOutputInternal>(
        modules::server->FindSignature(globals::gameConfig->GetSignature("CEntityIOOutput_FireOutputInternal")));

    if (m_pFireOutputInternal == nullptr) {
        CSSHARP_CORE_ERROR("Failed to find signature for \'CEntityIOOutput_FireOutputInternal\'");
        return;
    }

    auto m_hook = funchook_create();
    funchook_prepare(m_hook, (void**)&m_pFireOutputInternal, (void*)&DetourFireOutputInternal);
    funchook_install(m_hook, 0);

    // Listener is added in ServerStartup as entity system is not initialised at this stage.
}

void EntityManager::OnShutdown() {
    globals::callbackManager.ReleaseCallback(on_entity_spawned_callback);
    globals::callbackManager.ReleaseCallback(on_entity_created_callback);
    globals::callbackManager.ReleaseCallback(on_entity_deleted_callback);
    globals::callbackManager.ReleaseCallback(on_entity_parent_changed_callback);
    globals::entitySystem->RemoveListenerEntity(&entityListener);
}

void CEntityListener::OnEntitySpawned(CEntityInstance *pEntity) {
    auto callback = globals::entityManager.on_entity_spawned_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->Execute();
    }
}
void CEntityListener::OnEntityCreated(CEntityInstance *pEntity) {
    auto callback = globals::entityManager.on_entity_created_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->Execute();
    }
}
void CEntityListener::OnEntityDeleted(CEntityInstance *pEntity) {
    auto callback = globals::entityManager.on_entity_deleted_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->Execute();
    }
}
void CEntityListener::OnEntityParentChanged(CEntityInstance *pEntity, CEntityInstance *pNewParent) {
    auto callback = globals::entityManager.on_entity_parent_changed_callback;

    if (callback && callback->GetFunctionCount()) {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->ScriptContext().Push(pNewParent);
        callback->Execute();
    }
}

void EntityManager::HookEntityOutput(const char* szClassname, const char* szOutput,
                                     CallbackT fnCallback)
{
    auto outputKey = OutputKey_t(szClassname, szOutput);
    counterstrikesharp::ScriptCallback* callback;

    auto search = m_pHookMap.find(outputKey);
    if (search == m_pHookMap.end()) {
        callback = globals::callbackManager.CreateCallback("");
        m_pHookMap[outputKey] = callback;
    }
    else
        callback = search->second;

    callback->AddListener(fnCallback);
}

void EntityManager::UnhookEntityOutput(const char* szClassname, const char* szOutput,
                                     CallbackT fnCallback)
{
    auto outputKey = OutputKey_t(szClassname, szOutput);
    counterstrikesharp::ScriptCallback* callback;

    auto search = m_pHookMap.find(outputKey);
    if (search != m_pHookMap.end()) {
        auto callback = search->second;
        callback->RemoveListener(fnCallback);

        if (!callback->GetFunctionCount()) {
            globals::callbackManager.ReleaseCallback(callback);
            m_pHookMap.erase(outputKey);
        }
    }
}

void DetourFireOutputInternal(CEntityIOOutput* const pThis, CEntityInstance* pActivator,
                              CEntityInstance* pCaller, const CVariant* const value, float flDelay)
{
    if (pCaller) {
        CSSHARP_CORE_TRACE("[EntityManager][FireOutputHook] - {}, {}", pThis->m_pDesc->m_pName,
                           pCaller->GetClassname());

        auto outputKey = OutputKey_t(pCaller->GetClassname(), pThis->m_pDesc->m_pName);
        auto& hookMap = globals::entityManager.m_pHookMap;

        auto search = hookMap.find(outputKey);
        if (search != hookMap.end()) {
            auto callback = search->second;

            if (callback && callback->GetFunctionCount()) {
                callback->ScriptContext().Reset();
                callback->ScriptContext().Push(pThis->m_pDesc->m_pName);
                callback->ScriptContext().Push(pActivator);
                callback->ScriptContext().Push(pCaller);
                callback->ScriptContext().Push(flDelay);
                callback->Execute();
            }
        }
    } else
        CSSHARP_CORE_TRACE("[EntityManager][FireOutputHook] - {}, unknown caller", pThis->m_pDesc->m_pName);

    m_pFireOutputInternal(pThis, pActivator, pCaller, value, flDelay);
}
    
}  // namespace counterstrikesharp