using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using NativeTestsPlugin;
using Xunit;

public static class TestUtils
{
    public static async Task WaitOneFrame()
    {
        await Server.NextFrameAsync(() => { }).ConfigureAwait(false);
    }

    public static async Task WaitUntilAsync(Func<bool> condition, string description, double timeoutSeconds = 10)
    {
        var stopwatch = Stopwatch.StartNew();
        do
        {
            if (await Server.NextWorldUpdateAsync(condition)) return;
        } while (stopwatch.Elapsed.TotalSeconds < timeoutSeconds);

        throw new TimeoutException($"Timed out after {timeoutSeconds}s waiting for {description}.");
    }

    public static async Task<(CCSPlayerController Player, CCSPlayerPawn Pawn)> CreateTestPlayerAsync()
    {
        Assert.Equal(NativeTestsPlugin.NativeTestsPlugin.gameThreadId, Thread.CurrentThread.ManagedThreadId);
        var oldBots = Utilities.GetPlayers().Where(p => p.IsBot).ToArray();

        Server.ExecuteCommand("bot_quota 0; bot_kick");
        await WaitUntilAsync(() => oldBots.All(p => !p.IsValid) && !Utilities.GetPlayers().Any(p => p.IsBot),
            "old test bots to be removed");

        Server.ExecuteCommand("bot_quota_mode normal; bot_quota 5");
        CCSPlayerController? player = null;
        CCSPlayerPawn? pawn = null;
        var stableFrames = 0;
        await WaitUntilAsync(() =>
        {
            foreach (var candidate in Utilities.GetPlayers().Where(p => p.IsBot))
            {
                var candidatePawn = candidate.PlayerPawn.Value;
                if (candidatePawn is not { IsValid: true } ||
                    candidatePawn.LifeState != (byte)LifeState_t.LIFE_ALIVE ||
                    candidatePawn.Collision == null || candidatePawn.CBodyComponent?.SceneNode == null ||
                    candidatePawn.ItemServices == null || candidatePawn.WeaponServices == null ||
                    !candidatePawn.WeaponServices.MyWeapons.Any()) continue;

                stableFrames = candidate == player && candidatePawn == pawn ? stableFrames + 1 : 1;
                player = candidate;
                pawn = candidatePawn;
                return stableFrames >= 3;
            }

            stableFrames = 0;
            return false;
        }, "a live test bot with a stable pawn and initialized loadout");

        AssertTestPlayer(player!, pawn!);
        return (player!, pawn!);
    }

    public static void AssertTestPlayer(CCSPlayerController player, CCSPlayerPawn pawn)
    {
        Assert.Equal(NativeTestsPlugin.NativeTestsPlugin.gameThreadId, Thread.CurrentThread.ManagedThreadId);
        Assert.True(player.IsValid, "Test controller is no longer valid.");
        Assert.True(pawn.IsValid, "Test pawn is no longer valid.");
        Assert.Equal(pawn, player.PlayerPawn.Value);
        Assert.Equal((byte)LifeState_t.LIFE_ALIVE, pawn.LifeState);
        Assert.NotNull(pawn.Collision);
        Assert.NotNull(pawn.CBodyComponent?.SceneNode);
        Assert.NotNull(pawn.ItemServices);
        Assert.NotNull(pawn.WeaponServices);
    }

    public static async Task WaitForSeconds(float seconds)
    {
        var startTick = Server.TickCount;
        var ticksToWait = (int)(seconds / Server.TickInterval);
        var targetTick = startTick + ticksToWait;
        await Server.RunOnTickAsync(targetTick, () => { }).ConfigureAwait(false);
    }
}