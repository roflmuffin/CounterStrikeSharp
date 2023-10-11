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

#include <vector>

#include "core/global_listener.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {
class TimerSystem;
class ScriptCallback;

namespace timers {
#define TIMER_FLAG_REPEAT (1 << 0)       /**< Timer will repeat until stopped */
#define TIMER_FLAG_NO_MAPCHANGE (1 << 1) /**< Timer will not carry over mapchanges */

class Timer {
    friend class TimerSystem;

public:
    Timer(float interval, float exec_time, CallbackT callback, int flags);
    ~Timer();

    float m_interval;
    ScriptCallback *m_callback;
    int m_flags;
    float m_exec_time;
    bool m_in_exec;
    bool m_kill_me;
};

}  // namespace timers

class ScriptCallback;

class TimerSystem : public GlobalClass {
public:
    TimerSystem();
    void OnAllInitialized() override;
    void OnLevelEnd() override;
    void OnGameFrame(bool simulating);
    double CalculateNextThink(double last_think_time, float interval);
    void RunFrame();
    void RemoveMapChangeTimers();
    timers::Timer *CreateTimer(float interval, CallbackT callback, int flags);
    void KillTimer(timers::Timer *timer);
    double GetTickedTime();

private:
    bool m_has_map_ticked = false;
    bool m_has_map_simulated = false;
    float m_last_ticked_time = 0.0f;
    ScriptCallback *m_on_tick_callback_ = nullptr;

    std::vector<timers::Timer *> m_once_off_timers;
    std::vector<timers::Timer *> m_repeat_timers;
};
}  // namespace counterstrikesharp
