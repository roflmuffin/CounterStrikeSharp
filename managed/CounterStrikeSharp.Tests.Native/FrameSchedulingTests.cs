using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class FrameSchedulingTests
{
    [Fact]
    public async Task QueueTaskForNextFrame_RunsOnMainThread()
    {
        await Task.Run(async () =>
        {
            await Task.Delay(10);
            Assert.NotEqual(Thread.CurrentThread.ManagedThreadId, NativeTestsPlugin.gameThreadId);

            await Server.NextFrameAsync(() => { Assert.Equal(Thread.CurrentThread.ManagedThreadId, NativeTestsPlugin.gameThreadId); });
        });
    }

    [Fact]
    public async Task QueueTaskForNextFrame_ExecutesCallback()
    {
        var mock = new Mock<Action>();

        Server.NextFrame(mock.Object);
        await WaitOneFrame();

        mock.Verify(s => s(), Times.Once);
    }

    [Fact]
    public async Task QueueTaskForNextFrame_ExecutesNestedCallbacks()
    {
        int startingTick = Server.TickCount;
        List<int> executionTicks = new List<int>();

        Action callback = () => executionTicks.Add(Server.TickCount);
        Server.NextFrame(() =>
        {
            callback();
            Server.NextFrame(() =>
            {
                callback();
                Server.NextFrame(() => { callback(); });
            });
        });

        await Server.RunOnTickAsync(startingTick + 4, () => { });
        Assert.Equal(3, executionTicks.Count);
        for (int i = 0; i < executionTicks.Count; i++)
        {
            Assert.Equal(startingTick + i + 1, executionTicks[i]);
        }
    }

    [Fact]
    public async Task QueueTaskForFrame_ExecutesAtSpecifiedTick()
    {
        var mock = new Mock<Action>();
        var callback = FunctionReference.Create(mock.Object);
        var targetTick = Server.TickCount + 5;

        NativeAPI.QueueTaskForFrame(targetTick, callback);

        mock.Verify(s => s(), Times.Never);

        // Wait for the tick to pass
        await Server.RunOnTickAsync(targetTick + 1, () => { });

        mock.Verify(s => s(), Times.Once);
    }

    [Fact]
    public async Task QueueTaskForNextWorldUpdate_RunsOnMainThread()
    {
        await Task.Run(async () =>
        {
            await Task.Delay(10);
            Assert.NotEqual(Thread.CurrentThread.ManagedThreadId, NativeTestsPlugin.gameThreadId);

            await Server.NextWorldUpdateAsync(() =>
            {
                Assert.Equal(Thread.CurrentThread.ManagedThreadId, NativeTestsPlugin.gameThreadId);
            });
        });
    }


    [Fact]
    public async Task QueueTaskForNextWorldUpdate_ExecutesCallback()
    {
        var mock = new Mock<Action>();

        Server.NextWorldUpdate(mock.Object);
        await WaitOneFrame();

        mock.Verify(s => s(), Times.Once);
    }

    [Fact]
    public async Task QueueTaskForNextWorldUpdate_ExecutesNestedCallbacks()
    {
        int startingTick = Server.TickCount;
        List<int> executionTicks = new List<int>();

        Action callback = () => executionTicks.Add(Server.TickCount);
        Server.NextWorldUpdate(() =>
        {
            callback();
            Server.NextWorldUpdate(() =>
            {
                callback();
                Server.NextWorldUpdate(() => { callback(); });
            });
        });

        await Server.RunOnTickAsync(startingTick + 4, () => { });
        Assert.Equal(3, executionTicks.Count);
        for (int i = 0; i < executionTicks.Count; i++)
        {
            Assert.Equal(startingTick + i + 1, executionTicks[i]);
        }
    }

    [Fact]
    public async Task NextFrame_HighLevelApi_ExecutesCallback()
    {
        bool called = false;
        Server.NextFrame(() => { called = true; });

        await WaitOneFrame();

        Assert.True(called, "NextFrame callback should have been called");
    }

    [Fact]
    public async Task NextFrameAsync_ReturnsCompletedTask()
    {
        bool called = false;
        var task = Server.NextFrameAsync(() => { called = true; });

        await task;

        Assert.True(called, "NextFrameAsync callback should have been called");
        Assert.True(task.IsCompleted, "Task should be completed");
    }

    [Fact]
    public async Task RunOnTickAsync_ReturnsCompletedTask()
    {
        bool called = false;
        var targetTick = Server.TickCount + 3;
        var task = Server.RunOnTickAsync(targetTick, () => { called = true; });

        await task;

        Assert.True(called, "RunOnTickAsync callback should have been called");
        Assert.True(task.IsCompleted, "Task should be completed");
    }

    [Fact]
    public async Task NextWorldUpdate_HighLevelApi_ExecutesCallback()
    {
        bool called = false;
        Server.NextWorldUpdate(() => { called = true; });

        await WaitOneFrame();

        Assert.True(called, "NextWorldUpdate callback should have been called");
    }

    [Fact]
    public async Task NextFrameConcurrentQueueDrainsProperly()
    {
        int callCount = 0;
        int targetCalls = 4096;
        var callsByFrame = new ConcurrentDictionary<int, int>();
        for (int i = 0; i < targetCalls; i++)
        {
            Server.NextFrame(() =>
            {
                callsByFrame.AddOrUpdate(Server.TickCount, 1, (_, count) => count + 1);
                Interlocked.Increment(ref callCount);
            });
        }

        // All tasks should have been drained by latest NextFrameAsync
        await Server.NextFrameAsync(() => { }).ConfigureAwait(false);

        Assert.Equal(4096, callCount);
    }

    [Fact]
    public async Task NextWorldUpdateConcurrentQueueDrainsProperly()
    {
        int callCount = 0;
        int targetCalls = 4096;
        var callsByFrame = new ConcurrentDictionary<int, int>();
        for (int i = 0; i < targetCalls; i++)
        {
            Server.NextWorldUpdate(() =>
            {
                callsByFrame.AddOrUpdate(Server.TickCount, 1, (_, count) => count + 1);
                Interlocked.Increment(ref callCount);
            });
        }

        // All tasks should have been drained by latest NextFrameAsync
        await Server.NextWorldUpdateAsync(() => { }).ConfigureAwait(false);

        Assert.Equal(4096, callCount);
    }
}
