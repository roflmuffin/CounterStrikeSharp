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

#pragma once

#include "core/global_listener.h"
#include "core/globals.h"
#include "inetchannel.h"
#include "networksystem/inetworkserializer.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {
class ScriptCallback;

struct UserMessageHook
{
    UserMessageHook()
    {
        m_pPreHook = nullptr;
        m_pPostHook = nullptr;
    }
    counterstrikesharp::ScriptCallback* m_pPreHook;
    counterstrikesharp::ScriptCallback* m_pPostHook;
    int m_messageId;
};

class UserMessageManager : public GlobalClass
{
  public:
    UserMessageManager();
    ~UserMessageManager();
    void OnAllInitialized() override;
    void OnShutdown() override;
    void Hook_PostEvent(CSplitScreenSlot nSlot,
                        bool bLocalOnly,
                        int nClientCount,
                        const uint64* clients,
                        INetworkMessageInternal* pEvent,
                        const CNetMessage* pData,
                        unsigned long nSize,
                        NetChannelBufType_t bufType);
    void Hook_ClientSvcUserMessage(CPlayerSlot slot, int um_type, uint32 size, const void* buf);
    void Hook_ClientSvcUserMessagePost(CPlayerSlot slot, int um_type, uint32 size, const void* buf);

    void UnhookUserMessage(int messageId, CallbackT fnCallback, HookMode mode);
    void HookUserMessage(int messageId, CallbackT fnCallback, HookMode mode);

    // Hooks a user message sent from a client to the server (e.g. CS_UM_CustomHudClicked).
    void UnhookClientMessage(int messageId, CallbackT fnCallback, HookMode mode);
    void HookClientMessage(int messageId, CallbackT fnCallback, HookMode mode);

  private:
    void HookMessageInternal(std::map<int, UserMessageHook*>& hooksMap, int messageId, CallbackT fnCallback, HookMode mode);
    void UnhookMessageInternal(std::map<int, UserMessageHook*>& hooksMap, int messageId, CallbackT fnCallback, HookMode mode);
    HookResult DispatchClientMessageCallbacks(CPlayerSlot slot, int um_type, uint32 size, const void* buf, HookMode mode);

    ScriptCallback* m_on_user_message_callback;
    std::map<int, UserMessageHook*> m_hooksMap;
    std::map<int, UserMessageHook*> m_clientHooksMap;
};

} // namespace counterstrikesharp
