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
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;

namespace CounterStrikeSharp.API.Core
{
    public sealed class GlobalContext
    {
        private static GlobalContext _instance = null;
        public static GlobalContext Instance => _instance;

        private DirectoryInfo rootDir;

        private List<PluginContext> _loadedPlugins = new List<PluginContext>();

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

        /**
         * Temporary way for base CSS to add commands without a plugin context
         */
        private void AddCommand(string name, string description, CommandInfo.CommandCallback handler)
        {
            var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
            {
                var command = new CommandInfo(ptr);
                if (i == -1)
                {
                    handler?.Invoke(null, command);
                    return;
                }

                var entity = new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1));
                handler?.Invoke(entity.IsValid ? entity : null, command);
            });

            var subscriber = new BasePlugin.CallbackSubscriber(handler, wrappedHandler, () => { });
            NativeAPI.AddCommand(name, description, false, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND,
                subscriber.GetInputArgument());
        }

        public void LoadAll()
        {
            Console.WriteLine("Loading GameData");
            GameData.Load(Path.Combine(rootDir.FullName, "gamedata", "gamedata.json"));
            
            for (int i = 1; i <= 9; i++)
            {
                AddCommand("css_" + i, "Command Key Handler", (player, info) =>
                {
                    if (player == null) return;
                    var key = Convert.ToInt32(info.GetArg(0).Split("_")[1]);
                    ChatMenus.OnKeyPress(player, key);
                });
            }

            Console.WriteLine("Loading .NET modules...");

            DirectoryInfo modules_directory_info;
            try
            {
                modules_directory_info = new DirectoryInfo(Path.Combine(rootDir.FullName, "plugins"));
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unable to access .NET modules directory: " + e.GetType().ToString() + " " + e.Message);
                return;
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
                var plugin = new PluginContext(path);

                try
                {
                    _loadedPlugins.Add(plugin);
                    plugin.Load();
                }
                catch (Exception e)
                {
                    _loadedPlugins.Remove(plugin);
                    Console.WriteLine($"Failed to load plugin {path} with error {e}");
                }
            }

            Console.WriteLine($"All managed modules were loaded. {_loadedPlugins.Count} plugins loaded.");
        }

        public PluginContext FindPluginByType(Type moduleClass)
        {
            var plugin = _loadedPlugins.FirstOrDefault(x => x.PluginType == moduleClass);
            return plugin;
        }
    }
}