using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class GameEventTests
{
    [Fact]
    public async Task CanRegisterAndDeregisterEventHandlers()
    {
        int callCount = 0;
        var callback = FunctionReference.Create((EventPlayerConnect @event) =>
        {
            Assert.NotNull(@event);
            Assert.NotEmpty(@event.Name);
            Assert.True(@event.Bot);
            callCount++;
        });

        NativeAPI.HookEvent("player_connect", callback, true);

        // Test hooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();

        Assert.Equal(1, callCount);
        NativeAPI.UnhookEvent("player_connect", callback, true);

        // Test unhooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();
        Assert.Equal(1, callCount);
    }
}

