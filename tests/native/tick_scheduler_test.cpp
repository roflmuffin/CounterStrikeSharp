#include <doctest.h>

#include <vector>

#include "core/tick_scheduler.h"

using counterstrikesharp::TickScheduler;

namespace {
void RunDue(TickScheduler& scheduler, int currentTick)
{
    for (auto& callback : scheduler.getCallbacks(currentTick))
    {
        callback();
    }
}
} // namespace

TEST_CASE("TickScheduler runs callbacks once their tick is reached, earliest first, exactly once")
{
    TickScheduler scheduler;
    std::vector<int> ran;
    scheduler.schedule(5, [&] {
        ran.push_back(5);
    });
    scheduler.schedule(3, [&] {
        ran.push_back(3);
    });
    scheduler.schedule(9, [&] {
        ran.push_back(9);
    });

    RunDue(scheduler, 2);
    CHECK(ran.empty());

    RunDue(scheduler, 5);
    CHECK(ran == std::vector<int>{ 3, 5 });

    RunDue(scheduler, 5);
    CHECK(ran == std::vector<int>{ 3, 5 });

    RunDue(scheduler, 100);
    CHECK(ran == std::vector<int>{ 3, 5, 9 });
}
