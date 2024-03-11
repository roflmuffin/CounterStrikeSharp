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

#include "tick_scheduler.h"

namespace counterstrikesharp {

void TickScheduler::schedule(int tick, std::function<void()> callback)
{
    std::lock_guard<std::mutex> lock(taskMutex);
    scheduledTasks.push(std::make_pair(tick, callback));
}

std::vector<std::function<void()>> TickScheduler::getCallbacks(int currentTick)
{
    std::vector<std::function<void()>> callbacksToRun;

    std::lock_guard<std::mutex> lock(taskMutex);

    if (scheduledTasks.empty()) {
        return callbacksToRun;
    }

    // Process tasks due for the current tick
    while (!scheduledTasks.empty() && scheduledTasks.top().first <= currentTick) {
        callbacksToRun.push_back(scheduledTasks.top().second);
        scheduledTasks.pop();
    }

    return callbacksToRun;
}
} // namespace counterstrikesharp