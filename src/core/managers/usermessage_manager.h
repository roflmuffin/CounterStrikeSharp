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

    void UnhookUserMessage(int messageId, CallbackT fnCallback, HookMode mode);
    void HookUserMessage(int messageId, CallbackT fnCallback, HookMode mode);

  private:
    ScriptCallback* m_on_user_message_callback;
    std::map<int, UserMessageHook*> m_hooksMap;
};

} // namespace counterstrikesharp
