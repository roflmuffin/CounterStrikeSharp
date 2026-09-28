using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    public async Task QueueTaskForFrame_ReturnsValue()
    {
        var tickCount = Server.TickCount;
        var returnValue = await Server.NextFrameAsync(() => Server.TickCount);

        Assert.Equal(tickCount + 1, returnValue);
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
    public async Task QueueTaskForNextWorldUpdate_ReturnsValue()
    {
        var tickCount = Server.TickCount;
        var returnValue = await Server.NextWorldUpdateAsync(() => Server.TickCount);

        Assert.Equal(tickCount + 1, returnValue);
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
    public Task NextFrameConcurrentQueueDrainsProperly()
        => AssertQueueDrainsProperly(Server.NextFrameAsync);

    [Fact]
    public Task NextWorldUpdateConcurrentQueueDrainsProperly()
        => AssertQueueDrainsProperly(Server.NextWorldUpdateAsync);

    private static async Task AssertQueueDrainsProperly(Func<Action, Task> enqueue)
    {
        const int targetCalls = 4096;
        var limit = CoreConfig.MaximumFrameTasksExecutedPerTick;
        Assert.True(limit > 0, "Frame task budget must be positive.");
        var callsByFrame = new ConcurrentDictionary<int, int>();
        var callsByIndex = new int[targetCalls];
        var tasks = new Task[targetCalls];
        for (int i = 0; i < targetCalls; i++)
        {
            var index = i;
            tasks[i] = enqueue(() =>
            {
                Assert.Equal(NativeTestsPlugin.gameThreadId, Thread.CurrentThread.ManagedThreadId);
                callsByFrame.AddOrUpdate(Server.TickCount, 1, (_, count) => count + 1);
                Interlocked.Increment(ref callsByIndex[index]);
            });
        }

        await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(30));

        Assert.All(callsByFrame.Values, count => Assert.InRange(count, 1, limit));
        Assert.Equal(targetCalls, callsByFrame.Values.Sum());
        Assert.All(callsByIndex, count => Assert.Equal(1, count));
    }
}
