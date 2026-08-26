using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

public class ScriptingTests
{
    private CCSPlayerController player;
    private CCSPlayerPawn? pawn;

    public async Task InitializeAsync()
    {
        Server.ExecuteCommand("bot_kick; bot_quota 5; bot_quota_mode normal");
        await WaitOneFrame();
        this.player = Utilities.GetPlayers().Last(p => p.LifeState == (byte)LifeState_t.LIFE_ALIVE);
        if (player.PlayerPawn.Value == null)
        {
            throw new Exception("No valid player pawn found for test player.");
        }

        this.pawn = player.PlayerPawn.Value;
    }

    [Fact]
    public async Task NetworkedVector_SchemaClass()
    {
        await InitializeAsync();

        foreach (var player in Utilities.GetPlayers())
        {
            Assert.Equal(30, player.ActionTrackingServices.PerRoundStats.Count);
        }
    }
}