using System.Collections.Concurrent;
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

        if (CoreConfig.PluginResolveNugetPackages)
        {
            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                if (TryLoadDependency(path, assembly.GetName().Name, assemblyName, out var dependency))
                {
                    _sharedAssemblies.TryAdd(dependency.GetName().Name, dependency);
                }
            }
        }

        _sharedAssemblies[assembly.GetName().Name] = assembly;
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
        var pluginAssemblyPaths = GetPluginsAssemblyPaths();

        AssemblyLoadContext.Default.Resolving += (context, name) =>
        {
            if (!_loadedSharedLibs)
            {
                LoadSharedLibraries();
                _loadedSharedLibs = true;
            }

            if (!_sharedAssemblies.TryGetValue(name.Name, out var assembly))
            {
                if (CoreConfig.PluginResolveNugetPackages && TryLoadExternalLibrary(name, out assembly))
                {
                    return assembly;
                }

                return null;
            }

            return assembly;
        };

        if (CoreConfig.PluginAutoLoadEnabled)
        {
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

        foreach (var plugin in _loadedPluginContexts)
        {
            try
            {
                plugin.Plugin?.OnAllPluginsLoaded(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OnAllPluginsLoaded failed");
            }
        }
    }

    private bool TryLoadExternalLibrary(AssemblyName assemblyName, out Assembly? assembly)
    {
        assembly = null;
        if (!TryResolveReflectionAssemblyPath(out var pluginName, out var pluginPath))
        {
            return false;
        }

        if (!TryLoadDependency(pluginPath, pluginName, assemblyName, out assembly))
        {
            return false;
        }

        return true;
    }

    private bool TryLoadDependency(string pluginAssemblyPath,
        string pluginAssemblyName,
        AssemblyName dependencyAssemblyName,
        out Assembly? assembly)
    {
        assembly = null;

        var dependencyName = dependencyAssemblyName.Name!;
        if (string.IsNullOrEmpty(pluginAssemblyPath) || _sharedAssemblies.ContainsKey(dependencyName))
        {
            return false;
        }

        var resolver = new PluginContextNuGetDependencyResolver(
            rootAssemblyName: pluginAssemblyName,
            rootAssemblyPath: Path.GetDirectoryName(pluginAssemblyPath)!,
            assemblyName: dependencyAssemblyName);

        var dependencyPath = resolver.ResolvePath();
        if (string.IsNullOrWhiteSpace(dependencyPath))
        {
            return false;
        }

        var loader = PluginLoader.CreateFromAssemblyFile(dependencyPath, configure: c =>
        {
            c.PreferSharedTypes = true;
        });

        assembly = loader.LoadDefaultAssembly();
        _sharedAssemblies[dependencyAssemblyName.Name!] = assembly;

        return true;
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

    private static bool TryResolveReflectionAssemblyPath(out string? assemblyName, out string? assemblyPath)
    {
        assemblyPath = null;
        assemblyName = null;

        if (AssemblyLoadContext.CurrentContextualReflectionContext is var reflectionContext && reflectionContext is null)
        {
            return false;
        }

        var mainAssemblyPathField = reflectionContext
            .GetType()
            .GetField("_mainAssemblyPath", BindingFlags.NonPublic | BindingFlags.Instance);

        if (mainAssemblyPathField is null)
        {
            return false;
        }

        assemblyPath = (string)mainAssemblyPathField.GetValue(reflectionContext)!;
        return !string.IsNullOrEmpty(assemblyPath);
    }
    
    private string[] GetPluginsAssemblyPaths()
    {
        // Skip "disabled" at root level
        var rootSubDirs = Directory.GetDirectories(_scriptHostConfiguration.PluginPath)
            .Where(d => !Path.GetFileName(d).Equals("disabled", StringComparison.OrdinalIgnoreCase));

        var pluginDirectories = new List<string>();

        foreach (var subDir in rootSubDirs)
        {
            var stack = new Stack<string>();
            stack.Push(subDir);

            while (stack.Count > 0)
            {
                var currentDir = stack.Pop();
                var dirName = Path.GetFileName(currentDir);
                var expectedDll = Path.Combine(currentDir, dirName + ".dll");

                if (File.Exists(expectedDll))
                {
                    pluginDirectories.Add(currentDir);
                    // Stop scanning deeper in this directory
                    continue;
                }

                // Add subdirectories to stack for further scanning
                foreach (var child in Directory.GetDirectories(currentDir))
                    stack.Push(child);
            }
        }

        return pluginDirectories
                .Select(d => Path.Combine(d, Path.GetFileName(d) + ".dll"))
                .ToArray();
    }
}
