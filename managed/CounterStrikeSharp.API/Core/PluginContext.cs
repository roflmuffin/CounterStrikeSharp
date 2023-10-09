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

        public PluginContext(string path)
        {
            _path = path;

            _assemblyLoader = PluginLoader.CreateFromAssemblyFile(path, new[] { typeof(IPlugin) }, config =>
            {
                config.EnableHotReload = true;
                config.IsUnloadable = true;
            });

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
                _plugin = (BasePlugin)Activator.CreateInstance(pluginType);
                _plugin.Load(hotReload);

                Console.WriteLine($"Finished loading plugin: {Name}");
            }
        }

        public void Unload(bool hotReload = false)
        {
            var cachedName = Name;

            Console.WriteLine($"Unloading plugin {Name}");

            _plugin.Unload(hotReload);

            foreach (var kv in _plugin.Handlers)
            {
                var data = kv.Value.GetValue() as object[];
                _plugin.DeregisterEventHandler(data[0].ToString(), (Action<GameEvent>)kv.Key,
                    Convert.ToBoolean(data[1]));
            }

            foreach (var kv in _plugin.CommandHandlers)
            {
                // _plugin.RemoveCommand((string)kv.Value.GetValue(), (CommandInfo.CommandCallback)kv.Key);
            }

            foreach (var kv in _plugin.ConvarChangeHandlers)
            {
                // var convar = (ConVar)kv.Value.GetValue();
                // _plugin.UnhookConVarChange((ConVar)kv.Value.GetValue(), (ConVar.ConVarChangedCallback)kv.Key);
                // convar.Unregister();
            }

            foreach (var kv in _plugin.Listeners)
            {
                _plugin.RemoveListener((string)kv.Value.GetValue(), kv.Key);
            }

            foreach (var timer in _plugin.Timers)
            {
                timer.Kill();
            }

            if (!hotReload) _assemblyLoader.Dispose();

            Console.WriteLine($"Finished unloading plugin {cachedName}");
        }
    }
}