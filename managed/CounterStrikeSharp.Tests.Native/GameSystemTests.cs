using System.Threading;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

// Listeners fired from our game system (Engine Feature cssharp.server/game-system-events).
public class GameSystemTests
{
    private static int _precacheCount;

    internal static void RegisterOnLoad(BasePlugin plugin)
    {
        // Load runs before the first map loads, so precache fires after this.
        plugin.RegisterListener<Listeners.OnServerPrecacheResources>(_ => Interlocked.Increment(ref _precacheCount));
    }

    [Fact]
    public void PrecacheResourcesListenerFiredOnMapLoad()
    {
        Assert.True(_precacheCount > 0);
    }

    [Fact]
    public async Task EntityThinkListenersAreFired()
    {
        var plugin = NativeTestsPlugin.Instance;
        int pre = 0, post = 0;
        Listeners.OnServerPreEntityThink onPre = () => pre++;
        Listeners.OnServerPostEntityThink onPost = () => post++;

        plugin.RegisterListener(onPre);
        plugin.RegisterListener(onPost);
        try
        {
            await WaitOneFrame();
            await WaitOneFrame();
            await WaitOneFrame();

            Assert.True(pre > 0);
            Assert.True(post > 0);
        }
        finally
        {
            plugin.RemoveListener(onPre);
            plugin.RemoveListener(onPost);
        }
    }
}
