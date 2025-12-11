using System.Threading.Tasks;
using CounterStrikeSharp.API;

public static class TestUtils
{
    public static async Task WaitOneFrame()
    {
        await Server.RunOnTickAsync(Server.TickCount + 2, () => { }).ConfigureAwait(false);
    }
}