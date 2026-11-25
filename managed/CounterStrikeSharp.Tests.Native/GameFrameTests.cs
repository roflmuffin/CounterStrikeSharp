using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

// GameFrame is hooked through its Slot Binding, at the slot index the boot Vtable Check verified.
public class GameFrameTests
{
    [Fact]
    public void GameFrame_IsVerifiedAndAvailable()
    {
        var status = EngineFeatures.GetStatus(EngineFeature.ServerGameFrame);

        Assert.True(status.IsAvailable, status.ToString());
    }

    [Fact]
    public async Task GameFrame_Ticks()
    {
        var before = Server.TickCount;

        await Server.NextFrameAsync(() => { });
        await Server.NextFrameAsync(() => { });

        Assert.True(Server.TickCount > before, $"tick count stayed at {before}");
    }
}
