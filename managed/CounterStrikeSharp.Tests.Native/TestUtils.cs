using System.Threading.Tasks;
using CounterStrikeSharp.API;

public static class TestUtils
{
    public static async Task WaitOneFrame()
    {
        await Server.NextFrameAsync(() => { }).ConfigureAwait(false);
    }

    public static async Task WaitForSeconds(float seconds)
    {
        var startTick = Server.TickCount;
        var ticksToWait = (int)(seconds / Server.TickInterval);
        var targetTick = startTick + ticksToWait;
        await Server.RunOnTickAsync(targetTick, () => { }).ConfigureAwait(false);
    }
}