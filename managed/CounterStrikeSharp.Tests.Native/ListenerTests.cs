using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;
using Xunit;

public class ListenerTests
{
    [Fact]
    public async Task CanRegisterAndDeregisterListeners()
    {
        int callCount = 0;
        var callback = FunctionReference.Create((int playerSlot, string name, string ipAddress) =>
        {
            Assert.NotNull(ipAddress);
            Assert.NotEmpty(name);
            Assert.Equal("127.0.0.1", ipAddress);
            callCount++;
        });

        NativeAPI.IssueServerCommand("bot_quota 0; bot_quota_mode normal");
        await WaitOneFrame();

        NativeAPI.AddListener("OnClientConnect", callback);

        // Test hooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();

        Assert.Equal(1, callCount);
        NativeAPI.RemoveListener("OnClientConnect", callback);

        // Test unhooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();
        Assert.Equal(1, callCount);

        NativeAPI.IssueServerCommand("bot_quota 1");
    }
}
