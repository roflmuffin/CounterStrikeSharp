using System.Collections.Generic;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class TimerTests
{
    [Fact]
    public async Task CreateTimer_ExecutesCallback()
    {
        await WaitOneFrame();

        var callCount = 0;
        var callback = FunctionReference.Create(() => { callCount++; });

        var timerPtr = NativeAPI.CreateTimer(0.1f, callback, 0);

        try
        {
            Assert.NotEqual(IntPtr.Zero, timerPtr);

            // Wait for the timer to fire (0.1 seconds + some buffer)
            await WaitForSeconds(0.2f);
            await WaitOneFrame();

            Assert.True(callCount >= 1, $"Timer callback should have been called at least once, got {callCount}");
        }
        finally
        {
            NativeAPI.KillTimer(timerPtr);
        }
    }

    [Fact]
    public async Task CreateTimer_RepeatsWithFlag()
    {
        await WaitOneFrame();

        var callCount = 0;
        var callback = FunctionReference.Create(() => { callCount++; });

        var timerPtr = NativeAPI.CreateTimer(0.05f, callback, (int)TimerFlags.REPEAT);

        try
        {
            Assert.NotEqual(IntPtr.Zero, timerPtr);

            await WaitForSeconds(0.250f);
            await WaitOneFrame();

            Assert.True(callCount >= 2, $"Repeating timer should have been called multiple times, got {callCount}");
        }
        finally
        {
            NativeAPI.KillTimer(timerPtr);
        }
    }

    [Fact]
    public async Task CreateTimer_TickBased()
    {
        var startTick = Server.TickCount;
        var tickCounts = new List<int>();
        int timesTicked = 0;

        // Run every 4 ticks
        var timer = new Timer(4 * Server.TickInterval, () =>
        {
            timesTicked++;
            tickCounts.Add(Server.TickCount);
        }, TimerFlags.REPEAT);

        await Server.RunOnTickAsync(startTick + 16, () => { });
        timer.Kill();

        Assert.Equal(4, timesTicked);
        for (int i = 0; i < tickCounts.Count; i++)
        {
            var expectedTick = startTick + (i + 1) * 4;
            Assert.Equal(expectedTick, tickCounts[i]);
        }
    }

    [Fact]
    public async Task KillTimer_StopsExecution()
    {
        await WaitOneFrame();

        var callCount = 0;
        var callback = FunctionReference.Create(() => { callCount++; });

        var timerPtr = NativeAPI.CreateTimer(0.05f, callback, (int)TimerFlags.REPEAT);

        // Wait for at least one call
        await WaitForSeconds(0.10f);
        await WaitOneFrame();

        var countBeforeKill = callCount;

        // Kill the timer
        NativeAPI.KillTimer(timerPtr);

        // Wait a bit more
        await WaitForSeconds(0.15f);
        await WaitOneFrame();

        var countAfterKill = callCount;

        Assert.True(countAfterKill <= countBeforeKill + 1,
            $"Timer should stop after kill. Before: {countBeforeKill}, After: {countAfterKill}");
    }

    [Fact]
    public async Task Timer_HighLevelApi_Works()
    {
        await WaitOneFrame();

        var callCount = 0;
        var timer = new Timer(0.1f, () => { callCount++; });

        try
        {
            // Wait for the timer to fire
            await WaitForSeconds(0.3f);
            await WaitOneFrame();

            Assert.True(callCount >= 1, $"Timer callback should have been called, got {callCount}");
        }
        finally
        {
            timer.Kill();
        }
    }
}
