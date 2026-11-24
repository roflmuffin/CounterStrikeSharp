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
#include "core/core_bindings.h"
#include "core/function.h"
#include "core/gameconfig.h"
#include "core/globals.h"
#include "core/log.h"
#include "core/recipientfilters.h"
#include "core/cs2_sdk/entity/dump.h"

#include <vector>

#include <engine2/ivengineserver2.h>
#include "scripting/callback_manager.h"

namespace counterstrikesharp {

EntityManager::EntityManager() { m_profile_name = "EntityManager"; }

EntityManager::~EntityManager() {}

CCheckTransmitInfoList::CCheckTransmitInfoList(CCheckTransmitInfoHack** pInfoInfoList, int nInfoCount)
    : infoList(pInfoInfoList), infoCount(nInfoCount)
{
}

#ifdef _WIN32
#define CALL_CONV CONV_THISCALL
#else
#define CALL_CONV CONV_CDECL
#endif

void EntityManager::OnAllInitialized()
{
    // Created even when the CheckTransmit hook is disabled, so listeners still register (and never fire).
    check_transmit = globals::callbackManager.CreateCallback("CheckTransmit");
    if (bindings::ShouldRegisterHooks(bindings::features::CheckTransmit, "CheckTransmit hook"))
    {
        m_hooks.AddGlobalIndex<ISource2GameEntities, void, CCheckTransmitInfoHack**, uint32_t, CBitVec<16384>&, CBitVec<16384>&,
                               const Entity2Networkable_t**, const uint16*, uint32_t>(
            static_cast<int>(*bindings::GetBindingOffset("ISource2GameEntities::CheckTransmit")), *(void***)globals::gameEntities, this,
            nullptr, &EntityManager::CheckTransmit);
    }
    on_entity_spawned_callback = globals::callbackManager.CreateCallback("OnEntitySpawned");
    on_entity_created_callback = globals::callbackManager.CreateCallback("OnEntityCreated");
    on_entity_deleted_callback = globals::callbackManager.CreateCallback("OnEntityDeleted");
    on_entity_parent_changed_callback = globals::callbackManager.CreateCallback("OnEntityParentChanged");
    on_entity_take_damage_pre_callback = globals::callbackManager.CreateCallback("OnEntityTakeDamagePre");
    on_entity_take_damage_post_callback = globals::callbackManager.CreateCallback("OnEntityTakeDamagePost");
    on_player_take_damage_pre_callback = globals::callbackManager.CreateCallback("OnPlayerTakeDamagePre");
    on_player_take_damage_post_callback = globals::callbackManager.CreateCallback("OnPlayerTakeDamagePost");

    // Each of these Engine Features is resolved on its own: a failed Binding leaves only its function null (the natives
    // calling it then fail with an error) and is named in the boot Resolution Report.
    CEntityInstance_AcceptInput =
        reinterpret_cast<decltype(CEntityInstance_AcceptInput)>(bindings::GetBindingAddress("CEntityInstance_AcceptInput"));
    CEntitySystem_AddEntityIOEvent =
        reinterpret_cast<decltype(CEntitySystem_AddEntityIOEvent)>(bindings::GetBindingAddress("CEntitySystem_AddEntityIOEvent"));
    CBaseEntity_EmitSoundFilter =
        reinterpret_cast<decltype(CBaseEntity_EmitSoundFilter)>(bindings::GetBindingAddress("CBaseEntity_EmitSoundFilter"));
    CBaseEntity_DispatchSpawn =
        reinterpret_cast<decltype(CBaseEntity_DispatchSpawn)>(bindings::GetBindingAddress("CBaseEntity_DispatchSpawn"));

    if (bindings::ShouldRegisterHooks(bindings::features::EntityOutputHooks, "entity output hooks"))
    {
        m_pFireOutputInternal = reinterpret_cast<FireOutputInternal>(bindings::GetBindingAddress("CEntityIOOutput_FireOutputInternal"));
        m_hooks.AddFunction(reinterpret_cast<void*>(m_pFireOutputInternal), &DetourFireOutputInternal);
    }

    // Take-damage listeners stay registrable when disabled; they just never fire (the detour is added in AllPluginsLoaded).
    if (bindings::ShouldRegisterHooks(bindings::features::EntityTakeDamageHooks, "take damage hooks"))
    {
        CBaseEntity_TakeDamageOld =
            reinterpret_cast<decltype(CBaseEntity_TakeDamageOld)>(bindings::GetBindingAddress("CBaseEntity_TakeDamageOld"));
        Func_OnTakeDamage =
            new ValveFunction((void*)CBaseEntity_TakeDamageOld, CALL_CONV,
                              std::vector<DataType_t>{ DATA_TYPE_POINTER, DATA_TYPE_POINTER, DATA_TYPE_POINTER }, DATA_TYPE_LONG_LONG);
    }

    // Listener is added in ServerStartup as entity system is not initialised at this stage.
}

void EntityManager::OnShutdown()
{
    m_hooks.Clear();
    globals::callbackManager.ReleaseCallback(on_entity_spawned_callback);
    globals::callbackManager.ReleaseCallback(on_entity_created_callback);
    globals::callbackManager.ReleaseCallback(on_entity_deleted_callback);
    globals::callbackManager.ReleaseCallback(on_entity_parent_changed_callback);
    globals::callbackManager.ReleaseCallback(on_entity_take_damage_pre_callback);
    globals::callbackManager.ReleaseCallback(on_entity_take_damage_post_callback);
    globals::callbackManager.ReleaseCallback(on_player_take_damage_pre_callback);
    globals::callbackManager.ReleaseCallback(on_player_take_damage_post_callback);

    globals::callbackManager.ReleaseCallback(check_transmit);
    if (globals::entitySystem) globals::entitySystem->RemoveListenerEntity(&entityListener);
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

KHook::Return<void> EntityManager::CheckTransmit(ISource2GameEntities* hookThis,
                                                 CCheckTransmitInfoHack** ppInfoList,
                                                 uint32_t nInfoCount,
                                                 CBitVec<16384>& unionTransmitEdicts1,
                                                 CBitVec<16384>& unionTransmitEdicts2,
                                                 const Entity2Networkable_t** pNetworkables,
                                                 const uint16* pEntityIndicies,
                                                 uint32_t nEntities)
{
    // VPROF_BUDGET(m_profile_name.c_str(), "CS# CheckTransmit");

    auto callback = globals::entityManager.check_transmit;

    if (callback && callback->GetFunctionCount())
    {
        CCheckTransmitInfoList* infoList = new CCheckTransmitInfoList(ppInfoList, nInfoCount);

        callback->ScriptContext().Reset();
        callback->ScriptContext().Push(infoList);
        callback->Execute();

        delete infoList;
    }
    return { KHook::Action::Ignore };
}

int64 DetourCBaseEntity_TakeDamageOld(CBaseEntity* pThis, CTakeDamageInfo* pInfo, CTakeDamageResult* pResult)
{
    if (!globals::entityManager.Hook_OnTakeDamage_Alive_Pre(pThis, pInfo, pResult))
    {
        return 1;
    }

    auto res = CBaseEntity_TakeDamageOld(pThis, pInfo, pResult);

    globals::entityManager.Hook_OnTakeDamage_Alive_Post(pThis, pInfo, pResult);

    return res;
}

// Returns "should take damage"
bool EntityManager::Hook_OnTakeDamage_Alive_Pre(CBaseEntity* entity, CTakeDamageInfo* info, CTakeDamageResult* pResult)
{
    auto* cb_entity = globals::entityManager.on_entity_take_damage_pre_callback;

    HookResult result = HookResult::Continue;
    if (cb_entity && cb_entity->GetFunctionCount())
    {
        cb_entity->ScriptContext().Reset();
        cb_entity->ScriptContext().Push(entity);
        cb_entity->ScriptContext().Push(info);

        for (auto fnMethodToCall : cb_entity->GetFunctions())
        {
            if (!fnMethodToCall) continue;
            fnMethodToCall(&cb_entity->ScriptContextStruct());

            auto hookResult = cb_entity->ScriptContext().GetResult<HookResult>();

            if (hookResult >= HookResult::Stop)
            {
                return false;
            }

            if (hookResult >= HookResult::Handled)
            {
                result = hookResult;
            }
        }
    }

    auto* cb_player = globals::entityManager.on_player_take_damage_pre_callback;
    if (entity->IsPawn() && cb_player && cb_player->GetFunctionCount())
    {
        cb_player->ScriptContext().Reset();
        cb_player->ScriptContext().Push(entity);
        cb_player->ScriptContext().Push(info);

        for (auto fnMethodToCall : cb_player->GetFunctions())
        {
            if (!fnMethodToCall) continue;
            fnMethodToCall(&cb_player->ScriptContextStruct());

            auto hookResult = cb_player->ScriptContext().GetResult<HookResult>();

            if (hookResult >= HookResult::Stop)
            {
                return false;
            }

            if (hookResult >= HookResult::Handled)
            {
                result = hookResult;
            }
        }
    }

    if (result >= HookResult::Handled)
    {
        return false;
    }

    return true;
}

void EntityManager::Hook_OnTakeDamage_Alive_Post(CBaseEntity* entity, CTakeDamageInfo* info, CTakeDamageResult* pResult)
{
    auto* cb_entity = globals::entityManager.on_entity_take_damage_post_callback;

    HookResult result = HookResult::Continue;
    if (cb_entity && cb_entity->GetFunctionCount())
    {
        cb_entity->ScriptContext().Reset();
        cb_entity->ScriptContext().Push(entity);
        cb_entity->ScriptContext().Push(info);
        cb_entity->ScriptContext().Push(pResult);
        cb_entity->Execute();
    }

    auto* cb_player = globals::entityManager.on_player_take_damage_post_callback;
    if (entity->IsPawn() && cb_player && cb_player->GetFunctionCount())
    {
        cb_player->ScriptContext().Reset();
        cb_player->ScriptContext().Push(entity);
        cb_player->ScriptContext().Push(info);
        cb_player->ScriptContext().Push(pResult);
        cb_player->Execute();
    }
}

KHook::Return<void> DetourFireOutputInternal(CEntityIOOutput* const pThis,
                                             CEntityInstance* pActivator,
                                             CEntityInstance* pCaller,
                                             const CVariant* const value,
                                             float flDelay,
                                             void* unk1,
                                             char* unk2)
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
                    return { KHook::Action::Supersede };
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
        return { KHook::Action::Supersede };
    }

    KHook::Recall(m_pFireOutputInternal, KHook::Return<void>{ KHook::Action::Ignore }, pThis, pActivator, pCaller, value, flDelay, unk1,
                  unk2);

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
    return { KHook::Action::Ignore };
}

SndOpEventGuid_t EntityEmitSoundFilter(CRecipientFilter& filter, uint32 ent, const char* pszSound, float flVolume, float flPitch)
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
    params.m_nPitch = flPitch; // not working, can't fix, i think the game abandon it

    return CBaseEntity_EmitSoundFilter(filter, ent, params);
}

} // namespace counterstrikesharp
