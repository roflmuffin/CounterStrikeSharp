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
using System.Text;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core
{
    public sealed class GlobalContext
    {
        private static GlobalContext _instance = null;
        public static GlobalContext Instance => _instance;

        private DirectoryInfo rootDir;

        private readonly List<PluginContext> _loadedPlugins = new();

        public GlobalContext()
        {
            rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            _instance = this;
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
            CoreConfig.Load(Path.Combine(rootDir.FullName, "configs", "core.json"));

            Console.WriteLine("Loading GameData from \"gamedata/gamedata.json\"");
            GameData.Load(Path.Combine(rootDir.FullName, "gamedata", "gamedata.json"));

            Console.WriteLine("Loading Admins from \"configs/admins.json\"");
            AdminManager.Load(Path.Combine(rootDir.FullName, "configs", "admins.json"));

            for (int i = 1; i <= 9; i++)
            {
                CommandUtils.AddStandaloneCommand("css_" + i, "Command Key Handler", (player, info) =>
                {
                    if (player == null) return;
                    var key = Convert.ToInt32(info.GetArg(0).Split("_")[1]);
                    ChatMenus.OnKeyPress(player, key);
                });
            }

            Console.WriteLine("Loading C# plugins...");
            int pluginCount = LoadAllPlugins();
            Console.WriteLine($"All managed modules were loaded. {pluginCount} plugins loaded.");

            RegisterPluginCommands();
        }

        public void LoadPlugin(string path)
        {
            var existingPlugin = FindPluginByModulePath(path);
            if (existingPlugin != null)
            {
                throw new Exception("Plugin is already loaded.");
            }

            var plugin = new PluginContext(path, _loadedPlugins.Select(x => x.PluginId).DefaultIfEmpty(0).Max() + 1);
            plugin.Load();
            _loadedPlugins.Add(plugin);
        }

        public int LoadAllPlugins()
        {
            DirectoryInfo modules_directory_info;
            try
            {
                modules_directory_info = new DirectoryInfo(Path.Combine(rootDir.FullName, "plugins"));
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unable to access .NET modules directory: " + e.GetType().ToString() + " " + e.Message);
                return 0;
            }

            DirectoryInfo[] proper_modules_directories;
            try
            {
                proper_modules_directories = modules_directory_info.GetDirectories();
            }
            catch
            {
                proper_modules_directories = new DirectoryInfo[0];
            }

            var filePaths = proper_modules_directories
                .Where(d => d.GetFiles().Any((f) => f.Name == d.Name + ".dll"))
                .Select(d => d.GetFiles().First((f) => f.Name == d.Name + ".dll").FullName)
                .ToArray();

            foreach (var path in filePaths)
            {
                try
                {
                    LoadPlugin(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load plugin {path} with error {e}");
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

            info.ReplyToCommand("  CounterStrikeSharp was created and is maintained by Michael \"roflmuffin\" Wilson.\n" +
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
                    info.ReplyToCommand($"  List of all plugins currently loaded by CounterStrikeSharp: {_loadedPlugins.Count} plugins loaded.", true);

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
                        info.ReplyToCommand("Valid usage: css_plugins start/load [relative plugin path || absolute plugin path] (e.g \"TestPlugin\", \"plugins/TestPlugin/TestPlugin.dll\")\n", true);
                        break;
                    }

                    // If our arugment doesn't end in ".dll" - try and construct a path similar to PluginName/PluginName.dll.
                    // We'll assume we have a full path if we have ".dll".
                    var path = info.GetArg(2);
                    if (!path.EndsWith(".dll"))
                    {
                        path = Path.Combine(rootDir.FullName, $"plugins/{path}/{path}.dll");
                    }
                    else
                    {
                        path = Path.Combine(rootDir.FullName, path);
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
                        info.ReplyToCommand("Valid usage: css_plugins stop/unload [plugin name || #plugin id] (e.g \"TestPlugin\", \"1\")\n", true);
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
                        info.ReplyToCommand("Valid usage: css_plugins restart/reload [plugin name || #plugin id] (e.g \"TestPlugin\", \"#1\")\n", true);
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
                    plugin.Load(true);
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

        public void RegisterPluginCommands()
        {
            CommandUtils.AddStandaloneCommand("css", "Counter-Strike Sharp options.", OnCSSCommand);
            CommandUtils.AddStandaloneCommand("css_plugins", "Counter-Strike Sharp plugin options.", OnCSSPluginCommand);
        }
        
    }
}