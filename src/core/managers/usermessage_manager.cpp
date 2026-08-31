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

#include "core/managers/usermessage_manager.h"

#include <entity2/entitysystem.h>
#include <public/eiface.h>

#include "core/UserMessage.h"
#include "core/log.h"
#include "core/managers/player_manager.h"
#include "igameeventsystem.h"
#include "scripting/callback_manager.h"

using namespace google;

SH_DECL_HOOK8_void(IGameEventSystem,
                   PostEventAbstract,
                   SH_NOATTRIB,
                   0,
                   CSplitScreenSlot,
                   bool,
                   int,
                   const uint64*,
                   INetworkMessageInternal*,
                   const CNetMessage*,
                   unsigned long,
                   NetChannelBufType_t);

SH_DECL_HOOK4_void(IServerGameClients, ClientSvcUserMessage, SH_NOATTRIB, 0, CPlayerSlot, int, uint32, const void*);

namespace counterstrikesharp {
UserMessageManager::UserMessageManager() {}

UserMessageManager::~UserMessageManager() {}

void UserMessageManager::OnAllInitialized()
{
    SH_ADD_HOOK_MEMFUNC(IGameEventSystem, PostEventAbstract, globals::gameEventSystem, this, &UserMessageManager::Hook_PostEvent, false);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientSvcUserMessage, globals::serverGameClients, this,
                        &UserMessageManager::Hook_ClientSvcUserMessage, false);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientSvcUserMessage, globals::serverGameClients, this,
                        &UserMessageManager::Hook_ClientSvcUserMessagePost, true);
}

void UserMessageManager::OnShutdown()
{
    SH_REMOVE_HOOK_MEMFUNC(IGameEventSystem, PostEventAbstract, globals::gameEventSystem, this, &UserMessageManager::Hook_PostEvent, false);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientSvcUserMessage, globals::serverGameClients, this,
                           &UserMessageManager::Hook_ClientSvcUserMessage, false);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientSvcUserMessage, globals::serverGameClients, this,
                           &UserMessageManager::Hook_ClientSvcUserMessagePost, true);
}

void UserMessageManager::HookUserMessage(int messageId, CallbackT fnCallback, HookMode mode)
{
    CSSHARP_CORE_TRACE("Hooking user message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

    HookMessageInternal(m_hooksMap, messageId, fnCallback, mode);
}

void UserMessageManager::UnhookUserMessage(int messageId, CallbackT fnCallback, HookMode mode)
{
    CSSHARP_CORE_TRACE("Unhooking user message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

    UnhookMessageInternal(m_hooksMap, messageId, fnCallback, mode);
}

void UserMessageManager::HookClientMessage(int messageId, CallbackT fnCallback, HookMode mode)
{
    CSSHARP_CORE_TRACE("Hooking client user message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

    HookMessageInternal(m_clientHooksMap, messageId, fnCallback, mode);
}

void UserMessageManager::UnhookClientMessage(int messageId, CallbackT fnCallback, HookMode mode)
{
    CSSHARP_CORE_TRACE("Unhooking client user message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

    UnhookMessageInternal(m_clientHooksMap, messageId, fnCallback, mode);
}

void UserMessageManager::HookMessageInternal(std::map<int, UserMessageHook*>& hooksMap, int messageId, CallbackT fnCallback, HookMode mode)
{
    UserMessageHook* pHook;

    auto search = hooksMap.find(messageId);
    // If hook struct is not found
    if (search == hooksMap.end())
    {
        pHook = new UserMessageHook();

        if (mode == HookMode::Post)
        {
            pHook->m_pPostHook = globals::callbackManager.CreateCallback("");
            pHook->m_pPostHook->AddListener(fnCallback);
        }
        else
        {
            pHook->m_pPreHook = globals::callbackManager.CreateCallback("");
            pHook->m_pPreHook->AddListener(fnCallback);
        }

        pHook->m_messageId = messageId;

        hooksMap[messageId] = pHook;

        return;
    }
    else
    {
        pHook = search->second;
    }

    if (mode == HookMode::Post)
    {
        if (!pHook->m_pPostHook)
        {
            pHook->m_pPostHook = globals::callbackManager.CreateCallback("");
        }

        pHook->m_pPostHook->AddListener(fnCallback);
    }
    else
    {
        if (!pHook->m_pPreHook)
        {
            pHook->m_pPreHook = globals::callbackManager.CreateCallback("");
        }

        pHook->m_pPreHook->AddListener(fnCallback);
    }
}

void UserMessageManager::UnhookMessageInternal(std::map<int, UserMessageHook*>& hooksMap,
                                               int messageId,
                                               CallbackT fnCallback,
                                               HookMode mode)
{
    UserMessageHook* pHook;
    ScriptCallback* pCallback;

    auto search = hooksMap.find(messageId);
    if (search == hooksMap.end())
    {
        return;
    }

    pHook = search->second;

    if (mode == HookMode::Post)
    {
        pCallback = pHook->m_pPostHook;
    }
    else
    {
        pCallback = pHook->m_pPreHook;
    }

    if (!pCallback)
    {
        return;
    }

    pCallback->RemoveListener(fnCallback);

    if (pCallback->GetFunctionCount() == 0)
    {
        globals::callbackManager.ReleaseCallback(pCallback);

        if (mode == HookMode::Post)
        {
            pHook->m_pPostHook = nullptr;
        }
        else
        {
            pHook->m_pPreHook = nullptr;
        }
    }

    return;
}

void UserMessageManager::Hook_PostEvent(CSplitScreenSlot nSlot,
                                        bool bLocalOnly,
                                        int nClientCount,
                                        const uint64* clients,
                                        INetworkMessageInternal* pEvent,
                                        const CNetMessage* pData,
                                        unsigned long nSize,
                                        NetChannelBufType_t bufType)
{
    auto message = UserMessage(pEvent, pData, nClientCount, const_cast<uint64*>(clients));

    auto iMessageID = message.GetMessageID();
    auto I = m_hooksMap.find(iMessageID);

    HookResult result = HookResult::Continue;

    if (I != m_hooksMap.end())
    {
        auto pEventHook = I->second;
        auto* pCallback = pEventHook->m_pPreHook;

        if (pCallback)
        {
            CSSHARP_CORE_TRACE("Pushing user message `{}` pointer: {}, post: {}", iMessageID, (void*)pEvent, false);
            pCallback->Reset();
            pCallback->ScriptContext().Push(&message);

            for (auto fnMethodToCall : pCallback->GetFunctions())
            {
                if (!fnMethodToCall) continue;
                fnMethodToCall(&pCallback->ScriptContextStruct());

                auto hookResult = pCallback->ScriptContext().GetResult<HookResult>();

                if (hookResult >= HookResult::Stop)
                {
                    RETURN_META(MRES_SUPERCEDE);
                }

                if (hookResult >= HookResult::Handled)
                {
                    result = hookResult;
                }
            }
        }
    }

    if (result >= HookResult::Handled)
    {
        RETURN_META(MRES_SUPERCEDE);
    }

    RETURN_META(MRES_IGNORED);
}

void UserMessageManager::Hook_ClientSvcUserMessage(CPlayerSlot slot, int um_type, uint32 size, const void* buf)
{
    if (DispatchClientMessageCallbacks(slot, um_type, size, buf, HookMode::Pre) >= HookResult::Handled)
    {
        RETURN_META(MRES_SUPERCEDE);
    }

    RETURN_META(MRES_IGNORED);
}

void UserMessageManager::Hook_ClientSvcUserMessagePost(CPlayerSlot slot, int um_type, uint32 size, const void* buf)
{
    DispatchClientMessageCallbacks(slot, um_type, size, buf, HookMode::Post);

    RETURN_META(MRES_IGNORED);
}

HookResult UserMessageManager::DispatchClientMessageCallbacks(CPlayerSlot slot, int um_type, uint32 size, const void* buf, HookMode mode)
{
    auto I = m_clientHooksMap.find(um_type);
    if (I == m_clientHooksMap.end())
    {
        return HookResult::Continue;
    }

    auto* pCallback = mode == HookMode::Post ? I->second->m_pPostHook : I->second->m_pPreHook;
    if (!pCallback || pCallback->GetFunctionCount() == 0)
    {
        return HookResult::Continue;
    }

    // Client messages arrive as a raw serialized payload, so deserialize them into
    // an allocated message that the managed UserMessage wrapper can work with.
    UserMessage message(um_type);
    if (message.GetSerializableMessage() == nullptr || !message.ParseFromBuffer(buf, (int)size))
    {
        CSSHARP_CORE_TRACE("Failed to deserialize client user message `{}` from slot {}", um_type, slot.Get());
        message.FreeNetMessage();
        return HookResult::Continue;
    }

    message.SetSenderSlot(slot.Get());

    CSSHARP_CORE_TRACE("Pushing client user message `{}` from slot {}, post: {}", um_type, slot.Get(), mode == HookMode::Post);

    HookResult result = HookResult::Continue;

    pCallback->Reset();
    pCallback->ScriptContext().Push(&message);

    for (auto fnMethodToCall : pCallback->GetFunctions())
    {
        if (!fnMethodToCall) continue;
        fnMethodToCall(&pCallback->ScriptContextStruct());

        auto hookResult = pCallback->ScriptContext().GetResult<HookResult>();

        if (hookResult >= HookResult::Stop)
        {
            result = hookResult;
            break;
        }

        if (hookResult >= HookResult::Handled)
        {
            result = hookResult;
        }
    }

    message.FreeNetMessage();

    return result;
}

} // namespace counterstrikesharp
