using System.Collections.Generic;
using CounterStrikeSharp.API.Core.Plugin;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Commands;

/// <summary>
/// Decorator for <see cref="ICommandManager"/> that tracks registered commands and removes them when disposed.
/// Used for plugins that register commands to ensure they are removed when the plugin is unloaded.
/// </summary>
public class PluginCommandManagerDecorator : ICommandManager, IDisposable
{
    private readonly ICommandManager _inner;
    private readonly List<CommandDefinition> _trackedCommands = new();
    private readonly IPluginContext _pluginContext;
    private readonly ILogger<PluginCommandManagerDecorator> _logger;

    public PluginCommandManagerDecorator(ICommandManager inner, IPluginContext pluginContext, ILogger<PluginCommandManagerDecorator> logger)
    {
        _pluginContext = pluginContext;
        _logger = logger;
        _inner = inner;
    }

    public void RegisterCommand(CommandDefinition definition)
    {
        _inner.RegisterCommand(definition);        
        _trackedCommands.Add(definition);
        _logger.LogDebug("Registered command {Command} from plugin {Plugin}", definition.Name, _pluginContext.Plugin.ModuleName);
    }

    public void RemoveCommand(CommandDefinition definition)
    {
        _inner.RemoveCommand(definition);
        _trackedCommands.Remove(definition);
        _logger.LogDebug("Removed command {Command} from plugin {Plugin}", definition.Name, _pluginContext.Plugin.ModuleName);
    }

    public void Dispose()
    {
        for (int i = _trackedCommands.Count - 1; i >= 0; i--)
        {
            RemoveCommand(_trackedCommands[i]);
        }
    }
}