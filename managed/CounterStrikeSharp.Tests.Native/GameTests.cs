using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

public class GameTests
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
    public async Task Offset_CBasePlayerPawn_CommitSuicide()
    {
        await InitializeAsync();
        Assert.Equal((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);

        pawn.CommitSuicide(true, true);
        await WaitOneFrame();
        Assert.NotEqual((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);
    }

    [Fact]
    public async Task Offset_CCSPlayer_ItemServices_RemoveWeapons()
    {
        await InitializeAsync();
        Assert.True(pawn.WeaponServices.MyWeapons.Any());

        player.RemoveWeapons();
        await WaitOneFrame();

        Assert.False(pawn.WeaponServices.MyWeapons.Any());
    }

    [Fact]
    public async Task Offset_CBaseEntity_Teleport()
    {
        await InitializeAsync();
        var originalPosition = pawn.AbsOrigin;
        var newPosition = (Vector3)originalPosition + new Vector3(0, 0, 100);

        pawn.Teleport(newPosition, null, null);
        await WaitOneFrame();

        Assert.Equal(newPosition.X, pawn.AbsOrigin.X, 1f);
        Assert.Equal(newPosition.Y, pawn.AbsOrigin.Y, 1f);
        Assert.Equal(newPosition.Z, pawn.AbsOrigin.Z, 1f);
    }

    [Fact]
    public async Task Offset_CCSPlayerController_ChangeTeam()
    {
        await InitializeAsync();
        var originalTeam = player.Team;
        var newTeam = originalTeam == CsTeam.Terrorist ? CsTeam.CounterTerrorist : CsTeam.Terrorist;

        player.ChangeTeam(newTeam);
        await WaitOneFrame();

        Assert.Equal(newTeam, player.Team);
    }

    [Fact]
    public async Task Offset_CCSPlayer_ItemServices_GiveNamedItem()
    {
        await InitializeAsync();

        player.RemoveWeapons();
        await WaitOneFrame();

        Assert.False(pawn.WeaponServices.MyWeapons.Any());

        var weapon = player.GiveNamedItem<CMolotovGrenade>("weapon_ak47");
        await WaitOneFrame();

        Assert.NotNull(weapon);
        Assert.Equal("weapon_ak47", weapon.DesignerName);
        Assert.Single(pawn.WeaponServices.MyWeapons);
    }
}