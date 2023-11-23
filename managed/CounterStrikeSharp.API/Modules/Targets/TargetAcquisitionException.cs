using System;

namespace CounterStrikeSharp.API.Modules.Targets;

public class TargetAcquisitionException : Exception
{
    public TargetResultCount Expected { get; set; }
    public TargetResultCount Received { get; set; }

    public TargetAcquisitionException(string message, TargetResultCount expected, TargetResultCount received) :
        base(message)
    {
        Expected = expected;
        Received = received;
    }
}
