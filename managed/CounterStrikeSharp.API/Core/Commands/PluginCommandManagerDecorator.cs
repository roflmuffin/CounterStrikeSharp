using System.Collections.Generic;
using CounterStrikeSharp.API.Core.Plugin;

namespace CounterStrikeSharp.API.Core.Commands;

/// <summary>
/// Decorator for <see cref="ICommandManager"/> that tracks registered commands and removes them when disposed.
/// Used for plugins that register commands to ensure they are removed when the plugin is unloaded.
/// </summary>
public class PluginCommandManagerDecorator : ICommandManager, IDisposable
{
    private readonly ICommandManager _inner;
    private readonly List<CommandDefinition> _trackedCommands = new();

    public PluginCommandManagerDecorator(ICommandManager inner)
    {
        _inner = inner;
    }

    public void RegisterCommand(CommandDefinition definition)
    {
        _inner.RegisterCommand(definition);        
        _trackedCommands.Add(definition);
    }

    public void RemoveCommand(CommandDefinition definition)
    {
        _inner.RemoveCommand(definition);
        _trackedCommands.Remove(definition);
    }

    public void Dispose()
    {
        for (int i = _trackedCommands.Count - 1; i >= 0; i--)
        {
            RemoveCommand(_trackedCommands[i]);
        }
    }
}