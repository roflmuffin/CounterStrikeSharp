#include "core/managers/clientmessage_manager.h"

#include <entity2/entitysystem.h>
#include <public/eiface.h>

#include "core/gameconfig.h"
#include "core/ClientMessage.h"
#include "core/log.h"
#include "core/managers/player_manager.h"
#include "igameeventsystem.h"
#include "scripting/callback_manager.h"

using namespace google;

SH_DECL_MANUALHOOK2(FilterMessage, 0, 0, 0, bool, const CNetMessage*, INetChannel*);

    namespace counterstrikesharp
{
    ClientMessageManager::ClientMessageManager() {}

    ClientMessageManager::~ClientMessageManager() {}

    void ClientMessageManager::OnAllInitialized()
    {
        const char* name = "CServerSideClient";
        void* serverSideClientVTable = reinterpret_cast<void*>(modules::engine->FindVirtualTable(name, globals::gameConfig->GetOffset(name)));
        if (serverSideClientVTable == nullptr)
        {
            CSSHARP_CORE_ERROR("Failed to find signature for \'%s\'", name);
            return;
        }

        m_hookid =
            SH_ADD_MANUALDVPHOOK(FilterMessage, serverSideClientVTable, SH_MEMBER(this, &ClientMessageManager::Hook_FilterMessage), false);
    }

    void ClientMessageManager::OnShutdown() { SH_REMOVE_HOOK_ID(m_hookid); }

    void ClientMessageManager::HookClientMessage(int messageId, CallbackT fnCallback, HookMode mode)
    {
        ClientMessageHook* pHook;

        CSSHARP_CORE_TRACE("Hooking client message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

        auto search = m_hooksMap.find(messageId);
        // If hook struct is not found
        if (search == m_hooksMap.end())
        {
            pHook = new ClientMessageHook();

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

    void ClientMessageManager::UnhookClientMessage(int messageId, CallbackT fnCallback, HookMode mode)
    {
        ClientMessageHook* pHook;
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

        CSSHARP_CORE_TRACE("Unhooking client message: {0} with callback pointer: {1}", messageId, (void*)fnCallback);

        return;
    }

    bool ClientMessageManager::FindPlayerByNetChan(INetChannel *pChannel, CPlayerSlot *pSlot)
    {
        for (int i = 0; i < 64; ++i)
        {
            CPlayerSlot slot(i);
            auto pNetChan = (INetChannel*)globals::engine->GetPlayerNetInfo(slot);
            if (pNetChan == pChannel)
            {
                *pSlot = slot;
                return true;
            }
        }

        return false;
    }

    bool ClientMessageManager::Hook_FilterMessage(const CNetMessage* pData, INetChannel* pChannel)
    {
        INetworkMessageInternal* pEvent = pData->GetNetMessage();

        CPlayerSlot player(0);
        if (!FindPlayerByNetChan(pChannel, &player))
        {
            return false;
        }

        int sender = player.Get();

        auto message = ClientMessage(pEvent, sender, pData);

        auto iMessageID = message.GetMessageID();

        auto I = m_hooksMap.find(iMessageID);

        HookResult result = HookResult::Continue;

        if (I != m_hooksMap.end())
        {
            auto pEventHook = I->second;
            auto* pCallback = pEventHook->m_pPreHook;

            if (pCallback)
            {
                CSSHARP_CORE_TRACE("Pushing client message `{}` sender: {}, post: {}", iMessageID, sender, false);
                pCallback->Reset();
                pCallback->ScriptContext().Push(&message);

                for (auto fnMethodToCall : pCallback->GetFunctions())
                {
                    if (!fnMethodToCall) continue;
                    fnMethodToCall(&pCallback->ScriptContextStruct());

                    auto hookResult = pCallback->ScriptContext().GetResult<HookResult>();

                    if (hookResult >= HookResult::Stop)
                    {
                        RETURN_META_VALUE(MRES_SUPERCEDE, true);
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
            RETURN_META_VALUE(MRES_SUPERCEDE, true);
        }

        RETURN_META_VALUE(MRES_IGNORED, true);
    }
    
} // namespace counterstrikesharp
