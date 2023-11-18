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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using McMaster.NETCore.Plugins;

namespace CounterStrikeSharp.API.Core
{
    public sealed class GlobalContext
    {
        public static GlobalContext Instance { get; private set; }

        private readonly string _rootDir;

        private readonly List<PluginContext> _loadedPlugins = new();

        public PluginApiRegistry ApiRegistry { get; } = new();

        private readonly List<(AssemblyName name, Assembly assembly)> _sharedAssemblies = new();

        private readonly AssemblyLoadContext _pluginLoadContext = new("plugin_load_context");

        public GlobalContext()
        {
            _rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.Parent!.FullName;
            Instance = this;

            var defaultContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;

            _pluginLoadContext.Resolving += (context, name) =>
            {
                try
                {
                    return defaultContext.LoadFromAssemblyName(name);
                }
                catch
                {
                    var assembly = _sharedAssemblies.First(a => a.name.Name == name.Name);
                    return assembly.assembly;
                }
            };

            LoadSharedLibs();
        }

        private void LoadSharedLibs()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(_rootDir, "shared")))
            {
                var dirName = Path.GetFileName(dir);
                var dllPath = Path.Combine(dir, dirName + ".dll");
                if (!Path.Exists(dllPath)) continue;

                var loader = PluginLoader.CreateFromAssemblyFile(dllPath, new[] { typeof(IPlugin) });
                var assembly = loader.LoadDefaultAssembly();

                _sharedAssemblies.Add((assembly.GetName(), assembly));

                Console.WriteLine($"Load shared library: {assembly.FullName}");
            }

            Console.ResetColor();
        }

        ~GlobalContext()
        {
            foreach (var plugin in _loadedPlugins)
            {
                plugin.Unload();
            }
        }

        public void OnNativeUnload()
        {
            foreach (var plugin in _loadedPlugins)
            {
                plugin.Unload();
            }
        }

        public void InitGlobalContext()
        {
            Console.WriteLine("Loading CoreConfig from \"configs/core.json\"");
            CoreConfig.Load(Path.Combine(_rootDir, "configs", "core.json"));

            Console.WriteLine("Loading GameData from \"gamedata/gamedata.json\"");
            GameData.Load(Path.Combine(_rootDir, "gamedata", "gamedata.json"));

            Console.WriteLine("Loading Admins from \"configs/admins.json\"");
            AdminManager.Load(Path.Combine(_rootDir, "configs", "admins.json"));

            for (var i = 1; i <= 9; i++)
            {
                CommandUtils.AddStandaloneCommand("css_" + i, "Command Key Handler", (player, info) =>
                {
                    if (player == null) return;
                    var key = Convert.ToInt32(info.GetArg(0).Split("_")[1]);
                    ChatMenus.OnKeyPress(player, key);
                });
            }

            Console.WriteLine("Loading C# plugins...");
            var pluginCount = LoadAllPlugins();
            Console.WriteLine($"All managed modules were loaded. {pluginCount} plugins loaded.");

            RegisterPluginCommands();
        }

        private PluginContext CreatePluginContext(string path)
        {
            var existingPlugin = FindPluginByModulePath(path);
            if (existingPlugin != null)
            {
                throw new FileLoadException("Plugin is already loaded.");
            }

            return new PluginContext(path, _sharedAssemblies.Select(a => a.name), _pluginLoadContext,
                _loadedPlugins.Select(x => x.PluginId).DefaultIfEmpty(0).Max() + 1);
        }

        private void LoadPlugin(string path)
        {
            var context = CreatePluginContext(path);
            context.FullLoad();
            _loadedPlugins.Add(context);
        }

        private int LoadAllPlugins()
        {
            DirectoryInfo modulesDirectoryInfo;
            try
            {
                modulesDirectoryInfo = new DirectoryInfo(Path.Combine(_rootDir, "plugins"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }

            DirectoryInfo[] properModulesDirectories;
            try
            {
                properModulesDirectories = modulesDirectoryInfo.GetDirectories();
            }
            catch
            {
                properModulesDirectories = Array.Empty<DirectoryInfo>();
            }

            var filePaths = properModulesDirectories
                .Select(dir => Path.Combine(dir.FullName, Path.GetFileName(dir.FullName) + ".dll"))
                .Where(File.Exists)
                .ToArray();

            var contexts = new List<PluginContext>(filePaths.Length);

            foreach (var path in filePaths)
            {
                Console.WriteLine($"Plugin path: {path}");
                try
                {
                    var context = CreatePluginContext(path);
                    context.InstantiatePlugin();

                    contexts.Add(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load plugin {path} with error {e}");
                }
            }

            foreach (var context in contexts)
            {
                try
                {
                    context.Load();
                    _loadedPlugins.Add(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load plugin {context.Name} with error {e}");
                }
            }

            return _loadedPlugins.Count;
        }

        public void UnloadAllPlugins()
        {
            foreach (var plugin in _loadedPlugins)
            {
                plugin.Unload();
                _loadedPlugins.Remove(plugin);
            }
        }

        private PluginContext? FindPluginByType(Type moduleClass)
        {
            return _loadedPlugins.FirstOrDefault(x => x.PluginType == moduleClass);
        }

        private PluginContext? FindPluginById(int id)
        {
            return _loadedPlugins.FirstOrDefault(x => x.PluginId == id);
        }

        private PluginContext? FindPluginByModuleName(string name)
        {
            return _loadedPlugins.FirstOrDefault(x => x.Name == name);
        }

        private PluginContext? FindPluginByModulePath(string path)
        {
            return _loadedPlugins.FirstOrDefault(x => x.PluginPath == path);
        }

        private PluginContext? FindPluginByIdOrName(string query)
        {
            PluginContext? plugin = null;
            if (Int32.TryParse(query, out var pluginNumber))
            {
                plugin = FindPluginById(pluginNumber);
                if (plugin != null) return plugin;
            }

            plugin = FindPluginByModuleName(query);

            return plugin;
        }

        [RequiresPermissions("can_execute_css_commands")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private void OnCSSCommand(CCSPlayerController? caller, CommandInfo info)
        {
            var currentVersion = Api.GetVersion();

            info.ReplyToCommand(
                "  CounterStrikeSharp was created and is maintained by Michael \"roflmuffin\" Wilson.\n" +
                "  Counter-Strike Sharp uses code borrowed from SourceMod, Source.Python, FiveM, Saul Rennison and CS2Fixes.\n" +
                "  See ACKNOWLEDGEMENTS.md for more information.\n" +
                "  Current API Version: " + currentVersion, true);
            return;
        }

        [RequiresPermissions("can_execute_css_commands")]
        [CommandHelper(minArgs: 1,
            usage: "[option]\n" +
                   "  list - List all plugins currently loaded.\n" +
                   "  start / load - Loads a plugin not currently loaded.\n" +
                   "  stop / unload - Unloads a plugin currently loaded.\n" +
                   "  restart / reload - Reloads a plugin currently loaded.",
            whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private void OnCSSPluginCommand(CCSPlayerController? caller, CommandInfo info)
        {
            switch (info.GetArg(1))
            {
                case "list":
                {
                    info.ReplyToCommand(
                        $"  List of all plugins currently loaded by CounterStrikeSharp: {_loadedPlugins.Count} plugins loaded.",
                        true);

                    foreach (var plugin in _loadedPlugins)
                    {
                        var sb = new StringBuilder();
                        sb.AppendFormat("  [#{0}]: \"{1}\" ({2})", plugin.PluginId, plugin.Name, plugin.Version);
                        if (!string.IsNullOrEmpty(plugin.Author)) sb.AppendFormat(" by {0}", plugin.Author);
                        if (!string.IsNullOrEmpty(plugin.Description))
                        {
                            sb.Append("\n");
                            sb.Append("    ");
                            sb.Append(plugin.Description);
                        }

                        info.ReplyToCommand(sb.ToString(), true);
                    }

                    break;
                }
                case "start":
                case "load":
                {
                    if (info.ArgCount < 2)
                    {
                        info.ReplyToCommand(
                            "Valid usage: css_plugins start/load [relative plugin path || absolute plugin path] (e.g \"TestPlugin\", \"plugins/TestPlugin/TestPlugin.dll\")\n",
                            true);
                        break;
                    }

                    // If our arugment doesn't end in ".dll" - try and construct a path similar to PluginName/PluginName.dll.
                    // We'll assume we have a full path if we have ".dll".
                    var path = info.GetArg(2);
                    if (!path.EndsWith(".dll"))
                    {
                        path = Path.Combine(_rootDir, $"plugins/{path}/{path}.dll");
                    }
                    else
                    {
                        path = Path.Combine(_rootDir, path);
                    }

                    try
                    {
                        LoadPlugin(path);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to load plugin {path} with error {e}");
                    }

                    break;
                }

                case "stop":
                case "unload":
                {
                    if (info.ArgCount < 2)
                    {
                        info.ReplyToCommand(
                            "Valid usage: css_plugins stop/unload [plugin name || #plugin id] (e.g \"TestPlugin\", \"1\")\n",
                            true);
                        break;
                    }

                    var pluginIdentifier = info.GetArg(2);
                    PluginContext? plugin = FindPluginByIdOrName(pluginIdentifier);
                    if (plugin == null)
                    {
                        info.ReplyToCommand($"Could not unload plugin \"{pluginIdentifier}\")", true);
                        break;
                    }

                    plugin.Unload();
                    _loadedPlugins.Remove(plugin);
                    break;
                }

                case "restart":
                case "reload":
                {
                    if (info.ArgCount < 2)
                    {
                        info.ReplyToCommand(
                            "Valid usage: css_plugins restart/reload [plugin name || #plugin id] (e.g \"TestPlugin\", \"#1\")\n",
                            true);
                        break;
                    }

                    var pluginIdentifier = info.GetArg(2);
                    var plugin = FindPluginByIdOrName(pluginIdentifier);

                    if (plugin == null)
                    {
                        info.ReplyToCommand($"Could not reload plugin \"{pluginIdentifier}\")", true);
                        break;
                    }

                    plugin.Unload(true);
                    plugin.FullLoad(true);
                    break;
                }

                default:
                    info.ReplyToCommand("Valid usage: css_plugins [option]\n" +
                                        "  list - List all plugins currently loaded.\n" +
                                        "  start / load - Loads a plugin not currently loaded.\n" +
                                        "  stop / unload - Unloads a plugin currently loaded.\n" +
                                        "  restart / reload - Reloads a plugin currently loaded."
                        , true);
                    break;
            }
        }

        private void RegisterPluginCommands()
        {
            CommandUtils.AddStandaloneCommand("css", "Counter-Strike Sharp options.", OnCSSCommand);
            CommandUtils.AddStandaloneCommand("css_plugins", "Counter-Strike Sharp plugin options.",
                OnCSSPluginCommand);
        }
    }
}