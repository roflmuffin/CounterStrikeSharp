using System;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class CommandTests
{
    [Fact]
    public async Task AddCommandHandler()
    {
        var mock = new Mock<Action>();
        var methodCallback = FunctionReference.Create(() =>
        {
            mock.Object.Invoke();
        });

        NativeAPI.AddCommand("test_native", "description", true, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, methodCallback);
        NativeAPI.IssueServerCommand("test_native");
        await WaitOneFrame();
        mock.Verify(s => s(), Times.Once);

        NativeAPI.RemoveCommand("test_native", methodCallback);
        NativeAPI.IssueServerCommand("test_native");
        await WaitOneFrame();
        mock.Verify(s => s(), Times.Once);
    }

    [Fact]
    public async Task IssueServerCommand()
    {
        bool called = false;
        NativeAPI.AddCommandListener("say", FunctionReference.Create(() =>
        {
            called = true;
        }), true);

        NativeAPI.IssueServerCommand("say Hello, world!");
        await WaitOneFrame();

        Assert.True(called, "The 'say' command handler was not called.");
    }
}