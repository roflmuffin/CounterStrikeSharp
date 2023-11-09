using System;

namespace CounterStrikeSharp.API.Core.Attributes.Registration;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
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