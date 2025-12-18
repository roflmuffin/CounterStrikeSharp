using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class FrameSchedulingTests
{
    [Fact]
    public async Task QueueTaskForNextFrame_ExecutesCallback()
    {
        var mock = new Mock<Action>();
        var callback = FunctionReference.Create(mock.Object);

        NativeAPI.QueueTaskForNextFrame(callback);
        await WaitOneFrame();

        mock.Verify(s => s(), Times.Once);
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
    public async Task QueueTaskForNextWorldUpdate_ExecutesCallback()
    {
        var mock = new Mock<Action>();
        var callback = FunctionReference.Create(mock.Object);

        NativeAPI.QueueTaskForNextWorldUpdate(callback);
        await WaitOneFrame();

        mock.Verify(s => s(), Times.Once);
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
}
