using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Core.Hosting;
using McMaster.NETCore.Plugins;
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
    private readonly Dictionary<string, Assembly> _sharedAssemblies = new();
    private bool _loadedSharedLibs = false;

    public PluginManager(IScriptHostConfiguration scriptHostConfiguration, ICommandManager commandManager,
        ILogger<PluginManager> logger, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
    {
        _scriptHostConfiguration = scriptHostConfiguration;
        _commandManager = commandManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    private void LoadLibrary(string path)
    {
        var loader = PluginLoader.CreateFromAssemblyFile(path, new[] { typeof(IPlugin), typeof(PluginCapability<>), typeof(PlayerCapability<>) },
            config => { config.PreferSharedTypes = true; });
        var assembly = loader.LoadDefaultAssembly();

        _sharedAssemblies[assembly.GetName().FullName] = assembly;
    }

    private void LoadSharedLibraries()
    {
        var sharedDirectory = Directory.GetDirectories(_scriptHostConfiguration.SharedPath);
        var sharedAssemblyPaths = sharedDirectory
            .Select(dir => Path.Combine(dir, Path.GetFileName(dir) + ".dll"))
            .Where(File.Exists)
            .ToArray();
        
        foreach (var sharedAssemblyPath in sharedAssemblyPaths)
        {
            try
            {
                LoadLibrary(sharedAssemblyPath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load shared assembly from {Path}", sharedAssemblyPath);
            }
        }
    }

    public void Load()
    {
        var pluginDirectories = Directory.GetDirectories(_scriptHostConfiguration.PluginPath);
        var pluginAssemblyPaths = pluginDirectories
            .Select(dir => Path.Combine(dir, Path.GetFileName(dir) + ".dll"))
            .Where(File.Exists)
            .ToArray();

        AssemblyLoadContext.Default.Resolving += (context, name) =>
        {
            if (!_loadedSharedLibs)
            {
                LoadSharedLibraries();
                _loadedSharedLibs = true;
            }

            if (!_sharedAssemblies.TryGetValue(name.FullName, out var assembly))
            {
                return null;
            }

            return assembly;
        };

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

        foreach (var plugin in _loadedPluginContexts)
        {
            plugin.Plugin.OnAllPluginsLoaded(false);
        }
    }

    public IEnumerable<PluginContext> GetLoadedPlugins()
    {
        return _loadedPluginContexts;
    }

    public void LoadPlugin(string path)
    {
        var plugin = new PluginContext(_serviceProvider, _commandManager, _scriptHostConfiguration, path,
            _loadedPluginContexts.Select(x => x.PluginId).DefaultIfEmpty(0).Max() + 1);
        _loadedPluginContexts.Add(plugin);
        plugin.Load();
    }
}