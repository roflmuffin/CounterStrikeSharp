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

#include "core/timer_system.h"

#include <public/eiface.h>

#include <algorithm>

#include "core/globals.h"
#include "core/log.h"
#include "core/managers/player_manager.h"
#include "scripting/callback_manager.h"

namespace counterstrikesharp {
namespace timers {
double universal_time = 0.0f;
double timer_next_think = 0.0f;
} // namespace timers

timers::Timer::Timer(float interval, float exec_time, CallbackT callback, int flags)
    : m_interval(interval), m_exec_time(exec_time), m_flags(flags), m_kill_me(false), m_in_exec(false)
{
    m_callback = globals::callbackManager.CreateCallback("Timer");
    m_callback->AddListener(callback);
}

timers::Timer::~Timer() { globals::callbackManager.ReleaseCallback(m_callback); }

TimerSystem::TimerSystem()
{
    m_has_map_ticked = false;
    m_has_map_simulated = false;
    m_last_ticked_time = 0.0f;
}

void TimerSystem::OnAllInitialized()
{
    m_on_tick_callback_ = globals::callbackManager.CreateCallback("OnTick");
    on_map_end_callback = globals::callbackManager.CreateCallback("OnMapEnd");
}

void TimerSystem::OnShutdown()
{
    globals::callbackManager.ReleaseCallback(m_on_tick_callback_);
    globals::callbackManager.ReleaseCallback(on_map_end_callback);
}

void TimerSystem::OnLevelEnd()
{
    if (on_map_end_callback && on_map_end_callback->GetFunctionCount())
    {
        on_map_end_callback->ScriptContext().Reset();
        on_map_end_callback->Execute();
    }

    globals::timerSystem.RemoveMapChangeTimers();

    m_has_map_simulated = false;
    m_has_map_ticked = false;
}

void TimerSystem::OnStartupServer()
{
    if (m_has_map_ticked)
    {
        CALL_GLOBAL_LISTENER(OnLevelEnd());

        CSSHARP_CORE_TRACE("name={0}", "LevelShutdown");
    }

    m_has_map_ticked = false;
    m_has_map_simulated = false;
}

void TimerSystem::OnGameFrame(bool simulating)
{
    if (simulating && m_has_map_ticked)
    {
        timers::universal_time += globals::getGlobalVars()->curtime - m_last_ticked_time;
        if (!m_has_map_simulated)
        {
            m_has_map_simulated = true;
        }
    }
    else
    {
        timers::universal_time += globals::engine_fixed_tick_interval;
    }

    m_last_ticked_time = globals::getGlobalVars()->curtime;
    m_has_map_ticked = true;

    // Handle timer tick
    if (timers::universal_time >= timers::timer_next_think)
    {
        RunFrame();

        timers::timer_next_think = CalculateNextThink(timers::timer_next_think, 0.1f);
    }

    if (m_on_tick_callback_->GetFunctionCount())
    {
        m_on_tick_callback_->ScriptContext().Reset();
        m_on_tick_callback_->Execute();
    }

    globals::playerManager.RunAuthChecks();
}

double TimerSystem::CalculateNextThink(double last_think_time, float interval)
{
    if (timers::universal_time - last_think_time - interval <= 0.1)
    {
        return last_think_time + interval;
    }
    else
    {
        return timers::universal_time + interval;
    }
}

void TimerSystem::RunFrame()
{
    for (int i = m_once_off_timers.size() - 1; i >= 0; i--)
    {
        auto timer = m_once_off_timers[i];
        if (timers::universal_time >= timer->m_exec_time)
        {
            timer->m_in_exec = true;
            timer->m_callback->ScriptContext().Reset();
            timer->m_callback->Execute();

            m_once_off_timers.erase(m_once_off_timers.begin() + i);
            delete timer;
        }
    }

    for (int i = m_repeat_timers.size() - 1; i >= 0; i--)
    {
        auto timer = m_repeat_timers[i];
        if (timers::universal_time >= timer->m_exec_time)
        {
            timer->m_in_exec = true;
            timer->m_callback->ScriptContext().Reset();
            timer->m_callback->Execute();

            if (timer->m_kill_me)
            {
                m_repeat_timers.erase(m_repeat_timers.begin() + i);
                delete timer;
                continue;
            }

            timer->m_in_exec = false;
            timer->m_exec_time = CalculateNextThink(timer->m_exec_time, timer->m_interval);
        }
    }
}

void TimerSystem::RemoveMapChangeTimers()
{
    auto isMapChangeTimer = [](timers::Timer* timer) {
        bool shouldRemove = timer->m_flags & TIMER_FLAG_NO_MAPCHANGE;
        if (shouldRemove)
        {
            delete timer;
        }
        return shouldRemove;
    };

    std::erase_if(m_once_off_timers, isMapChangeTimer);
    std::erase_if(m_repeat_timers, isMapChangeTimer);
}

timers::Timer* TimerSystem::CreateTimer(float interval, CallbackT callback, int flags)
{
    float exec_time = timers::universal_time + interval;

    auto timer = new timers::Timer(interval, exec_time, callback, flags);

    if (flags & TIMER_FLAG_REPEAT)
    {
        m_repeat_timers.push_back(timer);
        return timer;
    }

    m_once_off_timers.push_back(timer);
    return timer;
}

void TimerSystem::KillTimer(timers::Timer* timer)
{
    if (!timer) return;

    if (std::find(m_repeat_timers.begin(), m_repeat_timers.end(), timer) == m_repeat_timers.end() &&
        std::find(m_once_off_timers.begin(), m_once_off_timers.end(), timer) == m_once_off_timers.end())
    {
        return;
    }

    if (timer->m_kill_me) return;

    // If were executing, make sure it doesn't run again next time.
    if (timer->m_in_exec)
    {
        timer->m_kill_me = true;
        return;
    }

    if (timer->m_flags & TIMER_FLAG_REPEAT)
    {
        auto it = std::remove_if(m_repeat_timers.begin(), m_repeat_timers.end(), [timer](timers::Timer* i) {
            return timer == i;
        });

        bool success;
        if ((success = it != m_repeat_timers.end())) m_repeat_timers.erase(it, m_repeat_timers.end());
        delete timer;
    }
    else
    {
        auto it = std::remove_if(m_once_off_timers.begin(), m_once_off_timers.end(), [timer](timers::Timer* i) {
            return timer == i;
        });

        bool success;
        if ((success = it != m_once_off_timers.end())) m_once_off_timers.erase(it, m_once_off_timers.end());
        delete timer;
    }
}

double TimerSystem::GetTickedTime() { return timers::universal_time; }
} // namespace counterstrikesharp
