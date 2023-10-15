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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Events;
using McMaster.NETCore.Plugins;

namespace CounterStrikeSharp.API.Core
{
    public class PluginContext
    {
        private BasePlugin _plugin;
        private PluginLoader _assemblyLoader;

        public string Name => _plugin?.ModuleName;
        public string Version => _plugin?.ModuleVersion;
        public Type PluginType => _plugin?.GetType();

        private readonly string _path;
        private readonly FileSystemWatcher _fileWatcher;

        public PluginContext(string path)
        {
            _path = path;

            _assemblyLoader = PluginLoader.CreateFromAssemblyFile(path, new[] { typeof(IPlugin) }, config =>
            {
                config.EnableHotReload = true;
                config.IsUnloadable = true;
            });

            _fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path)
            };

            _fileWatcher.Deleted += async (s, e) =>
            {
                if (e.FullPath == path)
                {
                    Console.WriteLine($"Plugin {Name} has been deleted, unloading...");
                    Unload(true);
                }
            };

            _fileWatcher.Filter = "*.dll";
            _fileWatcher.EnableRaisingEvents = true;
            _assemblyLoader.Reloaded += async (s, e) => await OnReloadedAsync(s, e);
        }

        private Task OnReloadedAsync(object sender, PluginReloadedEventArgs eventargs)
        {
            Console.WriteLine($"Reloading plugin {Name}");
            _assemblyLoader = eventargs.Loader;
            Unload(hotReload: true);
            Load(hotReload: true);

            return Task.CompletedTask;
        }

        public void Load(bool hotReload = false)
        {
            using (_assemblyLoader.EnterContextualReflection())
            {
                Type pluginType = _assemblyLoader.LoadDefaultAssembly().GetTypes()
                    .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));

                if (pluginType == null) throw new Exception("Unable to find plugin in DLL");

                Console.WriteLine($"Loading plugin: {pluginType.Name}");
                _plugin = (BasePlugin)Activator.CreateInstance(pluginType)!;
                _plugin.ModulePath = _path;
                _plugin.RegisterAllAttributes(_plugin);
                _plugin.Load(hotReload);

                Console.WriteLine($"Finished loading plugin: {Name}");
            }
        }

        public void Unload(bool hotReload = false)
        {
            var cachedName = Name;

            Console.WriteLine($"Unloading plugin {Name}");

            _plugin.Unload(hotReload);

            _plugin.Dispose();

            if (!hotReload)
            {
                _assemblyLoader.Dispose();
                _fileWatcher.Dispose();
            }

            Console.WriteLine($"Finished unloading plugin {cachedName}");
        }
    }
}