using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Core.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Plugin.Host;

public class PluginManager : IPluginManager
{
    private readonly HashSet<PluginContext> _loadedPluginContexts = new();
    private readonly IScriptHostConfiguration _scriptHostConfiguration;
    private readonly ICommandManager _commandManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PluginManager> _logger;

    public PluginManager(IScriptHostConfiguration scriptHostConfiguration, ICommandManager commandManager, ILogger<PluginManager> logger, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
    {
        _scriptHostConfiguration = scriptHostConfiguration;
        _commandManager = commandManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void Load()
    {
        var pluginDirectories = Directory.GetDirectories(_scriptHostConfiguration.PluginPath);
        var pluginAssemblyPaths = pluginDirectories
            .Select(dir => Path.Combine(dir, Path.GetFileName(dir) + ".dll"))
            .Where(File.Exists)
            .ToArray();

        foreach (var path in pluginAssemblyPaths)
        {
            try
            {
                LoadPlugin(path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load plugin from {Path}", path);
            }
        }
    }

    public IEnumerable<PluginContext> GetLoadedPlugins()
    {
        return _loadedPluginContexts;
    }

    public void LoadPlugin(string path)
    {
        var plugin = new PluginContext(_serviceProvider, _commandManager, _scriptHostConfiguration, path, _loadedPluginContexts.Select(x => x.PluginId).DefaultIfEmpty(0).Max() + 1);
        _loadedPluginContexts.Add(plugin);
        plugin.Load();
    }
}