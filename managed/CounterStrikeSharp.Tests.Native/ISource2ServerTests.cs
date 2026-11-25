using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Xunit;

namespace NativeTestsPlugin;

// ISource2Server's hooks and direct calls go through Slot Bindings, at the slot index the boot Vtable Check verified
// (or healed). Hibernation, Steam API, fatal shutdown and not-in-game updates cannot be triggered on the test server;
// for those only the Engine Feature status is checked.
public class ISource2ServerTests
{
    public static IEnumerable<object[]> Features() =>
    [
        [EngineFeature.ServerHibernationEvents], [EngineFeature.ServerSteamApiEvents], [EngineFeature.ServerHostnameChanged],
        [EngineFeature.ServerFatalShutdown], [EngineFeature.ServerUpdateWhenNotInGame], [EngineFeature.ServerPreWorldUpdate],
        [EngineFeature.ServerEconItemSystem], [EngineFeature.ServerIsPaused],
    ];

    [Theory]
    [MemberData(nameof(Features))]
    public void EngineFeature_IsAvailable(EngineFeature feature)
    {
        var status = EngineFeatures.GetStatus(feature);

        Assert.True(status.IsAvailable, status.ToString());
    }

    [Fact]
    public async Task PreWorldUpdate_ListenerFires()
    {
        var calls = 0;
        var callback = FunctionReference.Create((bool simulating) => { calls++; });
        NativeAPI.AddListener("OnServerPreWorldUpdate", callback);
        try
        {
            await WaitOneFrame();
            await WaitOneFrame();
        }
        finally
        {
            NativeAPI.RemoveListener("OnServerPreWorldUpdate", callback);
        }

        Assert.True(calls > 0, "OnServerPreWorldUpdate never fired");
    }

    [Fact]
    public async Task HostNameChanged_ListenerFiresWithTheNewName()
    {
        var hostname = $"cssharp-e2e-{Guid.NewGuid():N}";
        string? received = null;
        var callback = FunctionReference.Create((string name) => { received = name; });
        NativeAPI.AddListener("OnHostNameChanged", callback);
        try
        {
            Server.ExecuteCommand($"hostname \"{hostname}\"");
            await WaitUntilAsync(() => received == hostname, "OnHostNameChanged with the new hostname", 5);
        }
        finally
        {
            NativeAPI.RemoveListener("OnHostNameChanged", callback);
        }

        Assert.Equal(hostname, received);
    }

    [Fact]
    public void IsServerPaused_IsFalseOnARunningServer()
    {
        Assert.False(NativeAPI.IsServerPaused());
    }

    [Fact]
    public void GetEconItemSystem_ReturnsTheSameValueEveryCall()
    {
        // On the dedicated server builds archived so far the slot's function returns null; the point is that the call
        // reaches the verified slot and comes back.
        Assert.Equal(NativeAPI.GetEconItemSystem(), NativeAPI.GetEconItemSystem());
    }
}
