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

#include <functional>
#include <queue>
#include <mutex>
#include <condition_variable>

namespace counterstrikesharp {
class TickScheduler
{
 public:
   struct TaskComparator
   {
       bool operator()(const std::pair<int, std::function<void()>>& a,
                       const std::pair<int, std::function<void()>>& b) const
       {
           return a.first > b.first;
       }
   };

   void schedule(int tick, std::function<void()> callback);
   std::vector<std::function<void()>> getCallbacks(int currentTick);
 private:
   std::priority_queue<std::pair<int, std::function<void()>>,
                       std::vector<std::pair<int, std::function<void()>>>,
                       TaskComparator>
       scheduledTasks;
   std::mutex taskMutex;
};
} // namespace counterstrikesharp