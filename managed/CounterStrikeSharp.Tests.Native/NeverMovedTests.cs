using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

// Interfaces that never shifted a slot we use: their slots are verified Slot Bindings, hooks go to the verified index and
// direct calls stay compiled (disabled if the verified index ever differs from the compiled one).
public class NeverMovedTests
{
    [Fact]
    public void NeverMovedEngineFeatures_AreVerifiedAndAvailable()
    {
        foreach (var feature in new[]
                 {
                     EngineFeature.ServerStartup, EngineFeature.ServerLoopMode, EngineFeature.ServerGlobalVars,
                     EngineFeature.NetworkUserMessages, EngineFeature.EventsGameEvents,
                 })
        {
            var status = EngineFeatures.GetStatus(feature);

            Assert.True(status.IsAvailable, status.ToString());
            Assert.Empty(status.Heals);
        }
    }

    [Fact]
    public void GlobalVars_AreReadThroughTheVerifiedSlots()
    {
        Assert.Equal("de_dust2", Server.MapName);
        Assert.True(Server.TickCount > 0, $"tick count {Server.TickCount}");
        Assert.True(Server.CurrentTime > 0, $"current time {Server.CurrentTime}");
        Assert.True(Server.MaxPlayers > 0, $"max players {Server.MaxPlayers}");
    }
}
