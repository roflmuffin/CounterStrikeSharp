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
#include "core/recipientfilters.h"

#include <funchook.h>
#include <vector>

#include <public/eiface.h>
#include "scripting/callback_manager.h"

SH_DECL_HOOK7_void(ISource2GameEntities,
                   CheckTransmit,
                   SH_NOATTRIB,
                   0,
                   CCheckTransmitInfo**,
                   int,
                   CBitVec<16384>&,
                   const Entity2Networkable_t**,
                   const uint16*,
                   int,
                   bool);

namespace counterstrikesharp {

EntityManager::EntityManager() { m_profile_name = "EntityManager"; }

EntityManager::~EntityManager() {}

CCheckTransmitInfoList::CCheckTransmitInfoList(CCheckTransmitInfo** pInfoInfoList, int nInfoCount)
    : infoList(pInfoInfoList), infoCount(nInfoCount)
{
}

void EntityManager::OnAllInitialized()
{
    SH_ADD_HOOK_MEMFUNC(ISource2GameEntities, CheckTransmit, globals::gameEntities, this, &EntityManager::CheckTransmit, true);

    check_transmit = globals::callbackManager.CreateCallback("CheckTransmit");
    on_entity_spawned_callback = globals::callbackManager.CreateCallback("OnEntitySpawned");
    on_entity_created_callback = globals::callbackManager.CreateCallback("OnEntityCreated");
    on_entity_deleted_callback = globals::callbackManager.CreateCallback("OnEntityDeleted");
    on_entity_parent_changed_callback = globals::callbackManager.CreateCallback("OnEntityParentChanged");

    m_pFireOutputInternal = reinterpret_cast<FireOutputInternal>(
        modules::server->FindSignature(globals::gameConfig->GetSignature("CEntityIOOutput_FireOutputInternal")));

    if (m_pFireOutputInternal == nullptr)
    {
        CSSHARP_CORE_CRITICAL("Failed to find signature for \'CEntityIOOutput_FireOutputInternal\'");
        return;
    }

    CEntityInstance_AcceptInput = decltype(CEntityInstance_AcceptInput)(
        modules::server->FindSignature(globals::gameConfig->GetSignature("CEntityInstance_AcceptInput")));

    if (!CEntityInstance_AcceptInput)
    {
        CSSHARP_CORE_CRITICAL("Failed to find signature for \'CEntityInstance_AcceptInput\'");
    }

    CEntitySystem_AddEntityIOEvent = decltype(CEntitySystem_AddEntityIOEvent)(
        modules::server->FindSignature(globals::gameConfig->GetSignature("CEntitySystem_AddEntityIOEvent")));

    if (!CEntitySystem_AddEntityIOEvent)
    {
        CSSHARP_CORE_CRITICAL("Failed to find signature for \'CEntitySystem_AddEntityIOEvent\'");
    }

    CBaseEntity_EmitSoundFilter = decltype(CBaseEntity_EmitSoundFilter)(
        modules::server->FindSignature(globals::gameConfig->GetSignature("CBaseEntity_EmitSoundFilter")));

    if (!CBaseEntity_EmitSoundFilter)
    {
        CSSHARP_CORE_CRITICAL("Failed to find signature for \'CBaseEntity_EmitSoundFilter\'");
    }

    auto m_hook = funchook_create();
    funchook_prepare(m_hook, (void**)&m_pFireOutputInternal, (void*)&DetourFireOutputInternal);
    funchook_install(m_hook, 0);

    // Listener is added in ServerStartup as entity system is not initialised at this stage.
}

void EntityManager::OnShutdown()
{
    globals::callbackManager.ReleaseCallback(on_entity_spawned_callback);
    globals::callbackManager.ReleaseCallback(on_entity_created_callback);
    globals::callbackManager.ReleaseCallback(on_entity_deleted_callback);
    globals::callbackManager.ReleaseCallback(on_entity_parent_changed_callback);
    globals::callbackManager.ReleaseCallback(check_transmit);
    globals::entitySystem->RemoveListenerEntity(&entityListener);

    SH_REMOVE_HOOK_MEMFUNC(ISource2GameEntities, CheckTransmit, globals::gameEntities, this, &EntityManager::CheckTransmit, true);
}

void CEntityListener::OnEntitySpawned(CEntityInstance* pEntity)
{
    auto callback = globals::entityManager.on_entity_spawned_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->Execute();
    }
}
void CEntityListener::OnEntityCreated(CEntityInstance* pEntity)
{
    auto callback = globals::entityManager.on_entity_created_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->Execute();
    }
}
void CEntityListener::OnEntityDeleted(CEntityInstance* pEntity)
{
    auto callback = globals::entityManager.on_entity_deleted_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->Execute();
    }
}
void CEntityListener::OnEntityParentChanged(CEntityInstance* pEntity, CEntityInstance* pNewParent)
{
    auto callback = globals::entityManager.on_entity_parent_changed_callback;

    if (callback && callback->GetFunctionCount())
    {
        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(pEntity);
        callback->ScriptContext().Push(pNewParent);
        callback->Execute();
    }
}

void EntityManager::HookEntityOutput(const char* szClassname, const char* szOutput, CallbackT fnCallback, HookMode mode)
{
    auto outputKey = OutputKey_t(szClassname, szOutput);
    CallbackPair* pCallbackPair;

    auto search = m_pHookMap.find(outputKey);
    if (search == m_pHookMap.end())
    {
        m_pHookMap[outputKey] = new CallbackPair();
        pCallbackPair = m_pHookMap[outputKey];
    }
    else
        pCallbackPair = search->second;

    auto* pCallback = mode == HookMode::Pre ? pCallbackPair->pre : pCallbackPair->post;
    pCallback->AddListener(fnCallback);
}

void EntityManager::UnhookEntityOutput(const char* szClassname, const char* szOutput, CallbackT fnCallback, HookMode mode)
{
    auto outputKey = OutputKey_t(szClassname, szOutput);

    auto search = m_pHookMap.find(outputKey);
    if (search != m_pHookMap.end())
    {
        auto* pCallbackPair = search->second;

        auto* pCallback = mode == Pre ? pCallbackPair->pre : pCallbackPair->post;

        pCallback->RemoveListener(fnCallback);

        if (!pCallbackPair->HasCallbacks())
        {
            m_pHookMap.erase(outputKey);
        }
    }
}

void EntityManager::CheckTransmit(CCheckTransmitInfo** pInfoInfoList,
                                  int nInfoCount,
                                  CBitVec<16384>& unionTransmitEdicts,
                                  const Entity2Networkable_t** pNetworkables,
                                  const uint16* pEntityIndicies,
                                  int nEntityIndices,
                                  bool bEnablePVSBits)
{
    VPROF_BUDGET(m_profile_name.c_str(), "CS# CheckTransmit");

    auto callback = globals::entityManager.check_transmit;

    if (callback && callback->GetFunctionCount())
    {
        CCheckTransmitInfoList* infoList = new CCheckTransmitInfoList(pInfoInfoList, nInfoCount);

        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(infoList);
        callback->Execute();

        delete infoList;
    }
}

void DetourFireOutputInternal(
    CEntityIOOutput* const pThis, CEntityInstance* pActivator, CEntityInstance* pCaller, const CVariant* const value, float flDelay)
{
    std::vector vecSearchKeys{ OutputKey_t("*", pThis->m_pDesc->m_pName), OutputKey_t("*", "*") };

    if (pCaller)
    {
        vecSearchKeys.push_back(OutputKey_t(pCaller->GetClassname(), pThis->m_pDesc->m_pName));
        OutputKey_t(pCaller->GetClassname(), "*");
    }

    std::vector<CallbackPair*> vecCallbackPairs;

    if (pCaller)
    {
        CSSHARP_CORE_TRACE("[EntityManager][FireOutputHook] - {}, {}", pThis->m_pDesc->m_pName, pCaller->GetClassname());

        auto& hookMap = globals::entityManager.m_pHookMap;

        for (auto& searchKey : vecSearchKeys)
        {
            auto search = hookMap.find(searchKey);
            if (search != hookMap.end())
            {
                vecCallbackPairs.push_back(search->second);
            }
        }
    }
    else
        CSSHARP_CORE_TRACE("[EntityManager][FireOutputHook] - {}, unknown caller", pThis->m_pDesc->m_pName);

    HookResult result = HookResult::Continue;

    for (auto pCallbackPair : vecCallbackPairs)
    {
        if (pCallbackPair->pre->GetFunctionCount())
        {
            pCallbackPair->pre->ScriptContext().Reset();
            pCallbackPair->pre->ScriptContext().Push(pThis);
            pCallbackPair->pre->ScriptContext().Push(pThis->m_pDesc->m_pName);
            pCallbackPair->pre->ScriptContext().Push(pActivator);
            pCallbackPair->pre->ScriptContext().Push(pCaller);
            pCallbackPair->pre->ScriptContext().Push(value);
            pCallbackPair->pre->ScriptContext().Push(flDelay);

            for (auto fnMethodToCall : pCallbackPair->pre->GetFunctions())
            {
                if (!fnMethodToCall) continue;
                fnMethodToCall(&pCallbackPair->pre->ScriptContextStruct());

                auto thisResult = pCallbackPair->pre->ScriptContext().GetResult<HookResult>();

                if (thisResult >= HookResult::Stop)
                {
                    return;
                }

                if (thisResult > result)
                {
                    result = thisResult;
                }
            }
        }
    }

    if (result >= HookResult::Handled)
    {
        return;
    }

    m_pFireOutputInternal(pThis, pActivator, pCaller, value, flDelay);

    for (auto pCallbackPair : vecCallbackPairs)
    {
        if (pCallbackPair->post->GetFunctionCount())
        {
            pCallbackPair->post->ScriptContext().Reset();
            pCallbackPair->post->ScriptContext().Push(pThis);
            pCallbackPair->post->ScriptContext().Push(pThis->m_pDesc->m_pName);
            pCallbackPair->post->ScriptContext().Push(pActivator);
            pCallbackPair->post->ScriptContext().Push(pCaller);
            pCallbackPair->post->ScriptContext().Push(value);
            pCallbackPair->post->ScriptContext().Push(flDelay);
            pCallbackPair->post->Execute();
        }
    }
}

SndOpEventGuid_t EntityEmitSoundFilter(IRecipientFilter& filter, uint32 ent, const char* pszSound, float flVolume, float flPitch)
{
    if (!CBaseEntity_EmitSoundFilter)
    {
        CSSHARP_CORE_ERROR("[EntityManager][EmitSoundFilter] - Failed to emit a sound. Signature for \'CBaseEntity_EmitSoundFilter\' is "
                           "not found. The latest update may have broken it.");
        return SndOpEventGuid_t{};
    }

    EmitSound_t params;
    params.m_pSoundName = pszSound;
    params.m_flVolume = flVolume;
    params.m_nPitch = flPitch;

    return CBaseEntity_EmitSoundFilter(filter, ent, params);
}

} // namespace counterstrikesharp
