using CounterStrikeSharp.API.Modules.Commands;

namespace CounterStrikeSharp.API.Core.Commands;

public class CommandDefinition
{
    public CommandDefinition(string name, string description, CommandInfo.CommandCallback callback)
    {
        Name = name;
        Description = description;
        Callback = callback;
    }

    public CommandDefinition()
    {
    }

    public string Name { get; init; }
    public string Description { get; init; }
    public CommandInfo.CommandCallback Callback { get; init; }

    public CommandUsage ExecutableBy { get; init; } = CommandUsage.CLIENT_AND_SERVER;

    public string? UsageHint { get; init; }

    public int? MinArgs { get; init; }

    public override string ToString()
    {
        return $"Name: {Name}, Description: {Description}, ExecutableBy: {ExecutableBy}, " +
               $"UsageHint: {UsageHint}, MinArgs: {MinArgs}";
    }
}