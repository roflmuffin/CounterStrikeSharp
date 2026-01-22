using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

public class GameTests
{
    private CCSPlayerController player;
    private readonly CCSPlayerPawn? pawn;

    public GameTests()
    {
        this.player = Utilities.GetPlayers().First(p => p.LifeState == (byte)LifeState_t.LIFE_ALIVE);
        this.pawn = player.PlayerPawn.Value;
    }

    [Fact]
    public async Task Offset_CBasePlayerPawn_CommitSuicide()
    {
        Assert.Equal((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);

        pawn.CommitSuicide(true, true);
        await WaitOneFrame();
        Assert.NotEqual((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);
    }

    [Fact]
    public async Task Offset_CCSPlayer_ItemServices_RemoveWeapons()
    {
        Assert.True(pawn.WeaponServices.MyWeapons.Any());

        player.RemoveWeapons();
        await WaitOneFrame();

        Assert.False(pawn.WeaponServices.MyWeapons.Any());
    }

    [Fact]
    public async Task Offset_CBaseEntity_Teleport()
    {
        var originalPosition = pawn.AbsOrigin;
        var newPosition = (Vector3)originalPosition + new Vector3(0, 0, 100);

        pawn.Teleport(newPosition, null, null);
        await WaitOneFrame();

        Assert.Equal(newPosition, (Vector3)pawn.AbsOrigin);
    }

    [Fact]
    public async Task Offset_CCSPlayerController_ChangeTeam()
    {
        var originalTeam = player.Team;
        var newTeam = originalTeam == CsTeam.Terrorist ? CsTeam.CounterTerrorist : CsTeam.Terrorist;

        player.ChangeTeam(newTeam);
        await WaitOneFrame();

        Assert.Equal(newTeam, player.Team);
    }
}