using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Targets;

public class TargetResult : IEnumerable<CCSPlayerController>
{
    public TargetResultCount Count { get; set; }
    public List<CCSPlayerController> Players { get; set; } = new();

    private void ThrowNonSingle()
    {
        if (Count == TargetResultCount.Multiple)
            throw new TargetAcquisitionException(
                "Multiple clients found.",
                TargetResultCount.Single,
                TargetResultCount.Multiple);
        throw new TargetAcquisitionException(
            "No clients found",
            TargetResultCount.Single,
            TargetResultCount.None);
    }

    public CCSPlayerController Single()
    {
        if (Count != TargetResultCount.Single) ThrowNonSingle();

        return Players.First();
    }

    public CCSPlayerController? SingleOrDefault()
    {
        return Count != TargetResultCount.Single ? null : Players.FirstOrDefault();
    }

    public IEnumerator<CCSPlayerController> GetEnumerator()
    {
        return Players.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
