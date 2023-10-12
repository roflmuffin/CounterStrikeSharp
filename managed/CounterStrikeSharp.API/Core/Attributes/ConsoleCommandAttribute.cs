using System;
using CounterStrikeSharp.API.Modules.Events;

namespace CounterStrikeSharp.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string Command { get; }
    public string Description { get; }

    public ConsoleCommandAttribute(string command, string description = null)
    {
        Command = command;
        Description = description;
    }
}