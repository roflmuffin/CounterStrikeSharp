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
    private CCSPlayerController player = null!;
    private CCSPlayerPawn pawn = null!;

    public async Task InitializeAsync()
    {
        (player, pawn) = await CreateTestPlayerAsync();
        AssertTestPlayer(player, pawn);
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
        await Server.NextWorldUpdateAsync(() =>
        {
            AssertTestPlayer(player, pawn);
            Assert.NotEmpty(pawn.WeaponServices!.MyWeapons);

            player.RemoveWeapons();
            Assert.Empty(pawn.WeaponServices.MyWeapons);
        });
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
        await Server.NextWorldUpdateAsync(() =>
        {
            AssertTestPlayer(player, pawn);
            var originalTeam = player.Team;
            Assert.Contains(originalTeam, new[] { CsTeam.Terrorist, CsTeam.CounterTerrorist });
            var newTeam = originalTeam == CsTeam.Terrorist ? CsTeam.CounterTerrorist : CsTeam.Terrorist;

            player.ChangeTeam(newTeam);
            Assert.True(player.IsValid, "ChangeTeam invalidated the test controller during the call.");
            Assert.Equal(newTeam, player.Team);
        });
    }

    [Fact]
    public async Task Offset_CCSPlayer_ItemServices_GiveNamedItem()
    {
        await InitializeAsync();

        await Server.NextWorldUpdateAsync(() =>
        {
            AssertTestPlayer(player, pawn);
            player.RemoveWeapons();
            Assert.Empty(pawn.WeaponServices!.MyWeapons);

            var weapon = player.GiveNamedItem<CBasePlayerWeapon>("weapon_ak47");
            Assert.NotNull(weapon);
            Assert.True(weapon.IsValid);
            Assert.Equal("weapon_ak47", weapon.DesignerName);
            Assert.Equal(weapon.EntityHandle.Raw, Assert.Single(pawn.WeaponServices.MyWeapons).Raw);
        });
    }

    [Fact]
    public async Task Offset_CCSPlayerController_Respawn()
    {
        await InitializeAsync();
        pawn.CommitSuicide(false, false);
        await WaitOneFrame();
        Assert.NotEqual((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);

        player.Respawn();
        await WaitOneFrame();

        var newPawn = player.PlayerPawn.Value;
        Assert.NotNull(newPawn);
        Assert.Equal((byte)LifeState_t.LIFE_ALIVE, newPawn.LifeState);
    }

    [Fact]
    public async Task Offset_CBaseEntity_IsPlayerPawn()
    {
        await InitializeAsync();
        Assert.True(pawn.IsPlayerPawn());

        var worldEnt = Utilities.GetEntityFromIndex<CWorld>(0);
        Assert.False(worldEnt.IsPlayerPawn());
    }

    [Fact]
    public async Task Signature_CBaseEntity_SetPawn()
    {
        await InitializeAsync();
        pawn.Controller.Value.SetPawn(pawn.Controller.Value.As<CCSPlayerController>().PlayerPawn.Value);
    }
}