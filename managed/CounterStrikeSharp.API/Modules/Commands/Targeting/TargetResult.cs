using System.Collections;

namespace CounterStrikeSharp.API.Modules.Commands.Targeting;

public class TargetResult : IEnumerable<CCSPlayerController>
{
    public List<CCSPlayerController> Players { get; set; } = new();

    public IEnumerator<CCSPlayerController> GetEnumerator()
    {
        return Players.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
