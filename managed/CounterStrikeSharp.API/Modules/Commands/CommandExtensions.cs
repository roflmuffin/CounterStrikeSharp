using CounterStrikeSharp.API.Modules.Commands.Targeting;

namespace CounterStrikeSharp.API.Modules.Commands;

public static class CommandExtensions
{
    /// <summary>
    /// Treats the argument at the specified index as a target string (@all, @me etc.) and returns the result.
    /// </summary>
    public static TargetResult GetArgTargetResult(this CommandInfo commandInfo, int index)
    {
        return new Target(commandInfo.GetArg(index)).GetTarget(commandInfo.CallingPlayer);
    }
}