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
                   NetChannelBufType_t)

    namespace counterstrikesharp
{
    UserMessageManager::UserMessageManager() {}

    UserMessageManager::~UserMessageManager() {}

    void UserMessageManager::OnAllInitialized()
    {
        SH_ADD_HOOK_MEMFUNC(IGameEventSystem, PostEventAbstract, globals::gameEventSystem, this, &UserMessageManager::Hook_PostEvent,
                            false);
    }

    void UserMessageManager::OnShutdown()
    {
        SH_REMOVE_HOOK_MEMFUNC(IGameEventSystem, PostEventAbstract, globals::gameEventSystem, this, &UserMessageManager::Hook_PostEvent,
                               false);
    }

    void UserMessageManager::HookUserMessage(int messageId, CallbackT fnCallback, HookMode mode)
    {
        UserMessageHook* pHook;

        CSSHARP_CORE_TRACE("Hooking user message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

        auto search = m_hooksMap.find(messageId);
        // If hook struct is not found
        if (search == m_hooksMap.end())
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

            m_hooksMap[messageId] = pHook;

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

    void UserMessageManager::UnhookUserMessage(int messageId, CallbackT fnCallback, HookMode mode)
    {
        UserMessageHook* pHook;
        ScriptCallback* pCallback;

        auto search = m_hooksMap.find(messageId);
        if (search == m_hooksMap.end())
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

        CSSHARP_CORE_TRACE("Unhooking user message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

        return;
    }

    void UserMessageManager::Hook_PostEvent(CSplitScreenSlot nSlot, bool bLocalOnly, int nClientCount, const uint64* clients,
                                            INetworkMessageInternal* pEvent, const CNetMessage* pData, unsigned long nSize,
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

} // namespace counterstrikesharp
