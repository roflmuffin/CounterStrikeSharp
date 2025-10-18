/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Logging;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Core.Plugin.Host;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Threading;
using System;

namespace CounterStrikeSharp.API.Core.Plugin
{
    public interface ISelfPluginControl
    {
        void TerminateSelf(string reason);
    }

    public class PluginContext : IPluginContext, ISelfPluginControl
    {
        public PluginState State { get; set; } = PluginState.Unregistered;
        public IPlugin Plugin { get; private set; }

        private PluginLoader Loader { get; set; }

        private ServiceProvider ServiceProvider { get; set; }

        public int PluginId { get; }

        private readonly ICommandManager _commandManager;
        private readonly IScriptHostConfiguration _hostConfiguration;
        private readonly string _path;
        private readonly FileSystemWatcher _fileWatcher;
        private readonly IServiceProvider _applicationServiceProvider;

        public string FilePath => _path;
        private IServiceScope _serviceScope;

        public string TerminationReason { get; private set; }

        // TOOD: ServiceCollection
        private ILogger _logger = CoreLogging.Factory.CreateLogger<PluginContext>();

        public PluginContext(IServiceProvider applicationServiceProvider, ICommandManager commandManager,
            IScriptHostConfiguration hostConfiguration,
            string path, int id)
        {
            _commandManager = commandManager;
            _hostConfiguration = hostConfiguration;
            _path = path;
            PluginId = id;

            Loader = PluginLoader.CreateFromAssemblyFile(path,
                new[]
                {
                    typeof(IPlugin), typeof(ILogger), typeof(IServiceCollection), typeof(IPluginServiceCollection<>),
                    typeof(ICommandManager)
                }, config =>
                {
                    config.EnableHotReload = true;
                    config.IsUnloadable = true;
                    config.PreferSharedTypes = true;
                });

            if (CoreConfig.PluginHotReloadEnabled)
            {
                _fileWatcher = new FileSystemWatcher
                {
                    Path = Path.GetDirectoryName(path)
                };

                _fileWatcher.Deleted += async (s, e) =>
                {
                    Server.NextWorldUpdate(() =>
                    {
                        if (e.FullPath == path)
                        {
                            _logger.LogInformation("Plugin {Name} has been deleted, unloading...", Plugin.ModuleName);
                            Unload(true);
                        }
                    });
                };

                _fileWatcher.Filter = "*.dll";
                _fileWatcher.EnableRaisingEvents = true;
                Loader.Reloaded += async (s, e) => await OnReloadedAsync(s, e);
            }
        }

        private Task OnReloadedAsync(object sender, PluginReloadedEventArgs eventargs)
        {
            Server.NextWorldUpdate(() =>
            {
                _logger.LogInformation("Reloading plugin {Name}", Plugin.ModuleName);
                Loader = eventargs.Loader;
                Unload(hotReload: true);
                Load(hotReload: true);
                Plugin.OnAllPluginsLoaded(hotReload: true);
            });

            return Task.CompletedTask;
        }

        public void Load(bool hotReload = false)
        {
            if (State == PluginState.Loaded) return;

            using (Loader.EnterContextualReflection())
            {
                var defaultAssembly = Loader.LoadDefaultAssembly();

                Type pluginType = defaultAssembly.GetExportedTypes()
                    .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));

                if (pluginType == null) throw new Exception("Unable to find plugin in assembly");

                var serviceCollection = new ServiceCollection();

                serviceCollection.Scan(scan =>
                    scan.FromAssemblies(defaultAssembly)
                        .AddClasses(c => c.AssignableTo<IPlugin>())
                        .AsSelf()
                        .WithSingletonLifetime()
                );

                serviceCollection.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog(new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .Enrich.With(new PluginNameEnricher(this))
                        .WriteTo.Console(
                            outputTemplate:
                            "{Timestamp:HH:mm:ss} [{Level:u4}] (plugin:{PluginName}) {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(
                            Path.Join(new[]
                            {
                                _hostConfiguration.RootPath, "logs",
                                $"log-{pluginType.Assembly.GetName().Name}.txt"
                            }), rollingInterval: RollingInterval.Day,
                            outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] plugin:{PluginName} {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(Path.Join(new[] { _hostConfiguration.RootPath, "logs", $"log-all.txt" }),
                            rollingInterval: RollingInterval.Day, shared: true,
                            outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] plugin:{PluginName} {Message:lj}{NewLine}{Exception}")
                        .CreateLogger());
                });

                Type interfaceType = typeof(IPluginServiceCollection<>).MakeGenericType(pluginType);
                Type[] serviceCollectionConfiguratorTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .ToArray();

                if (serviceCollectionConfiguratorTypes.Any())
                {
                    foreach (var t in serviceCollectionConfiguratorTypes)
                    {
                        var pluginServiceCollection = Activator.CreateInstance(t);
                        MethodInfo method = t.GetMethod("ConfigureServices");
                        method?.Invoke(pluginServiceCollection, new object[] { serviceCollection });
                    }
                }

                serviceCollection.AddScoped<ICommandManager>(c => _commandManager);
                serviceCollection.DecorateSingleton<ICommandManager, PluginCommandManagerDecorator>();

                serviceCollection.AddSingleton<IPluginContext>(this);
                serviceCollection.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
                serviceCollection.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
                serviceCollection.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer));
                ServiceProvider = serviceCollection.BuildServiceProvider();

                var minimumApiVersion = pluginType.GetCustomAttribute<MinimumApiVersion>()?.Version;
                var currentVersion = Api.GetVersion();

                // Ignore version 0 for local development
                if (currentVersion > 0 && minimumApiVersion != null && minimumApiVersion > currentVersion)
                    throw new Exception(
                        $"Plugin \"{Path.GetFileName(_path)}\" requires a newer version of CounterStrikeSharp. The plugin expects version [{minimumApiVersion}] but the current version is [{currentVersion}].");

                _logger.LogInformation("Loading plugin {Name}", pluginType.Assembly.GetName().Name);

                _serviceScope = ServiceProvider.CreateScope();

                Plugin = _serviceScope.ServiceProvider.GetRequiredService(pluginType) as IPlugin;

                if (Plugin == null) throw new Exception("Unable to create plugin instance");

                State = PluginState.Loading;

                Plugin.ModulePath = _path;
                Plugin.Logger = _serviceScope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(pluginType);
                Plugin.CommandManager = _serviceScope.ServiceProvider.GetRequiredService<ICommandManager>();
                Plugin.RegisterAllAttributes(Plugin);
                Plugin.Localizer = ServiceProvider.GetRequiredService<IStringLocalizer>();
                Plugin.Logger = ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(pluginType);

                Plugin.InitializeConfig(Plugin, pluginType);

                if (Plugin is BasePlugin basePlugin)
                {
                    basePlugin.SelfControl = this;
                }

                this.TerminationReason = string.Empty;
                try
                {
                    Plugin.Load(hotReload);
                }
                catch (Exception ex)
                {
                    if ((ex.InnerException ?? ex) is PluginTerminationException pluginEx)
                    {
                        _logger.LogCritical("Terminating plugin {Name} with reason: {Reason}", Plugin.ModuleName, pluginEx.TerminationReason);
                        this.TerminationReason = pluginEx.TerminationReason;
                    }
                    else
                    {
                        _logger.LogError(ex, "Failed to load plugin {Name}", Plugin.ModuleName);
                        this.TerminationReason = ex.Message ?? "Unknown";
                    }

                    Unload(hotReload);
                    return;
                }

                _logger.LogInformation("Finished loading plugin {Name}", Plugin.ModuleName);

                State = PluginState.Loaded;
            }
        }


        public void Unload(bool hotReload = false)
        {
            if (State == PluginState.Unloaded) return;

            State = PluginState.Unloaded;
            var cachedName = Plugin.ModuleName;

            _logger.LogInformation("Unloading plugin {Name}", Plugin.ModuleName);

            try
            {
                Plugin.Unload(hotReload);
            }
            catch
            {
                _logger.LogError("Failed to unload {Name} during error recovery, forcing cleanup", Plugin.ModuleName);
                return;
            }
            finally
            {
                Plugin.Dispose();
                _serviceScope.Dispose();
            }

            _logger.LogInformation("Finished unloading plugin {Name}", cachedName);
        }

        public void TerminateWithReason(string reason)
        {
            this.TerminationReason = reason;

            switch (State)
            {
                case PluginState.Unloaded:
                case PluginState.Loading:
                    break;
                case PluginState.Loaded:
                    _logger.LogInformation("Terminating plugin {Name} with reason: {Reason}", Plugin.ModuleName, reason);
                    Unload(false);
                    break;
            }

            // Force execution flow interruption via globally-handled exception to prevent stack unwinding
            throw new PluginTerminationException(reason);
        }

        void ISelfPluginControl.TerminateSelf(string reason)
        {
            if (State != PluginState.Unloaded)
            {
                if (Thread.CurrentThread.IsThreadPoolThread)
                {
                    Server.NextFrame(() => TerminateWithReason(reason));
                }
                else
                {
                    TerminateWithReason(reason);
                }

                // **Failsafe mechanism** ensures execution termination
                // Prevents control flow leakage back to plugin execution context
                throw new NotImplementedException();
            }
        }
    }
}