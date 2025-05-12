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

#include <scripting/autonative.h>
#include <scripting/script_engine.h>

#include "core/timer_system.h"

namespace counterstrikesharp {

timers::Timer* CreateTimer(ScriptContext& script_context)
{
    auto interval = script_context.GetArgument<float>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto flags = script_context.GetArgument<int>(2);

    return globals::timerSystem.CreateTimer(interval, callback, flags);
}

void KillTimer(ScriptContext& script_context)
{
    auto timer = script_context.GetArgument<timers::Timer*>(0);
    globals::timerSystem.KillTimer(timer);
}

REGISTER_NATIVES(timers, {
    ScriptEngine::RegisterNativeHandler("CREATE_TIMER", CreateTimer);
    ScriptEngine::RegisterNativeHandler("KILL_TIMER", KillTimer);
})
} // namespace counterstrikesharp
