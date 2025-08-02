using System;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class CommandTests
{
    private readonly Mock<Action> _mock;
    private readonly FunctionReference _methodCallback;

    public CommandTests()
    {
        _mock = new Mock<Action>();
        _methodCallback = FunctionReference.Create(() => { _mock.Object.Invoke(); });
    }

    [Fact]
    public async Task AddCommandHandler()
    {
        NativeAPI.AddCommand("css_test_native", "description", true, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, _methodCallback);
        NativeAPI.IssueServerCommand("css_test_native");
        await WaitOneFrame();
        _mock.Verify(s => s(), Times.Once);

        NativeAPI.RemoveCommand("css_test_native", _methodCallback);
        NativeAPI.IssueServerCommand("css_test_native");
        await WaitOneFrame();
        _mock.Verify(s => s(), Times.Exactly(3));
    }

    [Fact]
    public async Task CanTriggerCommandsWithPublicChatTrigger()
    {
        NativeAPI.AddCommand("css_test_public_chat", "description", true, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, _methodCallback);
        NativeAPI.IssueServerCommand("css_test_public_chat");
        await WaitOneFrame();
        _mock.Verify(s => s(), Times.Once);

        NativeAPI.IssueServerCommand("say \"!test_public_chat\"");
        await WaitOneFrame();
        _mock.Verify(s => s(), Times.Exactly(2));
    }

    [Fact]
    public async Task CanTriggerCommandsWithSilentChatTrigger()
    {
        NativeAPI.AddCommand("css_test_silent_chat", "description", true, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, _methodCallback);
        NativeAPI.IssueServerCommand("css_test_silent_chat");
        await WaitOneFrame();
        _mock.Verify(s => s(), Times.Once);

        NativeAPI.IssueServerCommand("say \"!test_silent_chat\"");
        await WaitOneFrame();
        _mock.Verify(s => s(), Times.Exactly(2));
    }

    [Fact]
    public async Task IssueServerCommand()
    {
        bool called = false;
        NativeAPI.AddCommandListener("say", FunctionReference.Create(() => { called = true; }), true);

        NativeAPI.IssueServerCommand("say Hello, world!");
        await WaitOneFrame();

        Assert.True(called, "The 'say' command handler was not called.");
    }
}