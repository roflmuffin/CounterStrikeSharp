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

struct ClientMessageHook
{
    ClientMessageHook()
    {
        m_pPreHook = nullptr;
        m_pPostHook = nullptr;
    }
    counterstrikesharp::ScriptCallback* m_pPreHook;
    counterstrikesharp::ScriptCallback* m_pPostHook;
    int m_messageId;
};

class ClientMessageManager : public GlobalClass
{
  public:
    ClientMessageManager();
    ~ClientMessageManager();
    void OnAllInitialized() override;
    void OnShutdown() override;
    bool FindPlayerByNetChan(INetChannel *pChannel, CPlayerSlot *pFoundSlot);
    bool Hook_FilterMessage(const CNetMessage *pData, INetChannel *pChannel);

    void UnhookClientMessage(int messageId, CallbackT fnCallback, HookMode mode);
    void HookClientMessage(int messageId, CallbackT fnCallback, HookMode mode);

  private:
    ScriptCallback* m_on_client_message_callback;
    std::map<int, ClientMessageHook*> m_hooksMap;
    int m_hookid;
};

} // namespace counterstrikesharp
