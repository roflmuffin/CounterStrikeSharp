using System.Numerics;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

// The player helpers fail soft per Engine Feature (ADR-0002). These tests pass whether cssharp.player/commit-suicide is
// available or not (E2E breaks its offset): CommitSuicide throws the typed exception only while it is disabled, and
// respawn and teleport keep working either way.
public class PlayerHelperFailSoftTests
{
    private static bool CommitSuicideAvailable => EngineFeatures.IsAvailable(EngineFeature.PlayerCommitSuicide);

    [Fact]
    public async Task CommitSuicide_ThrowsTypedExceptionWhenDisabled()
    {
        if (CommitSuicideAvailable) return;
        var (_, pawn) = await CreateTestPlayerAsync();

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => pawn.CommitSuicide(false, true));
        Assert.Equal(EngineFeatureNames.PlayerCommitSuicide, exception.EngineFeature);
        Assert.Equal("CBasePlayerPawn_CommitSuicide", exception.Binding);
        Assert.Equal((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);
        Console.WriteLine($"[PlayerHelperFailSoftTests] CommitSuicide threw {exception.GetType().Name}: {exception.Message}");
    }

    [Fact]
    public async Task RespawnAndTeleport_WorkWithoutCommitSuicide()
    {
        var (player, pawn) = await CreateTestPlayerAsync();

        // Teleport
        var newPosition = (Vector3)pawn.AbsOrigin! + new Vector3(0, 0, 100);
        pawn.Teleport(newPosition, null, null);
        await WaitOneFrame();
        Assert.Equal(newPosition.Z, pawn.AbsOrigin!.Z, 1f);

        // Respawn, after a death that does not go through CommitSuicide.
        player.ExecuteClientCommandFromServer("kill");
        await WaitUntilAsync(() => !pawn.IsValid || pawn.LifeState != (byte)LifeState_t.LIFE_ALIVE, "the test bot to die");

        player.Respawn();
        await WaitOneFrame();

        var newPawn = player.PlayerPawn.Value;
        Assert.NotNull(newPawn);
        Assert.Equal((byte)LifeState_t.LIFE_ALIVE, newPawn.LifeState);
    }
}
