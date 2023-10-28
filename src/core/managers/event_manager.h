/**
 * =============================================================================
 * SourceMod
 * Copyright (C) 2004-2016 AlliedModders LLC.  All rights reserved.
 * =============================================================================
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * As a special exception, AlliedModders LLC gives you permission to link the
 * code of this program (as well as its derivative works) to "Half-Life 2," the
 * "Source Engine," the "SourcePawn JIT," and any Game MODs that run on software
 * by the Valve Corporation.  You must obey the GNU General Public License in
 * all respects for all other code used.  Additionally, AlliedModders LLC grants
 * this exception to all derivative works.  AlliedModders LLC defines further
 * exceptions, found in LICENSE.txt (as of this writing, version JULY-31-2007),
 * or <http://www.sourcemod.net/license.php>.
 *
 * This file has been modified from its original form, under the GNU General
 * Public License, version 3.0.
 */

#pragma once

class CUtlString;

#include <igameeventsystem.h>
#include <public/igameevents.h>

#include <map>
#include <string>
#include <stack>

#include "core/global_listener.h"
#include "core/globals.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {
class ScriptCallback;
class PluginFunction;
} // namespace counterstrikesharp

struct EventHook
{
    EventHook()
    {
        m_pPreHook = nullptr;
        m_pPostHook = nullptr;
    }
    counterstrikesharp::ScriptCallback* m_pPreHook;
    counterstrikesharp::ScriptCallback* m_pPostHook;
    std::string m_Name;
};

struct EventOverride {
  bool m_bDontBroadcast;
};

namespace counterstrikesharp {

class EventManager : public IGameEventListener2, public GlobalClass
{
  public:
    EventManager();
    ~EventManager() override;

    // GlobalClass
    void OnShutdown() override;
    void OnAllInitialized() override;
    void OnStartup() override;

    // IGameEventListener2
    void FireGameEvent(IGameEvent* pEvent) override;

    bool UnhookEvent(const char* szName, CallbackT fnCallback, bool bPost);
    bool HookEvent(const char* szName, CallbackT fnCallback, bool bPost);

  private:
    bool OnFireEvent(IGameEvent* pEvent, bool bDontBroadcast);
    bool OnFireEventPost(IGameEvent* pEvent, bool bDontBroadcast);

    std::map<std::string, EventHook*> m_hooksMap;

    std::stack<EventHook *> m_EventStack;
    std::stack<IGameEvent *> m_EventCopies;
};

} // namespace counterstrikesharp
