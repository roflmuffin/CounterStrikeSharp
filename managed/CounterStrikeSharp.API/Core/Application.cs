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
using System.Text;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Core.Plugin.Host;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core
{
    public sealed class Application
    {
        private static Application _instance = null!;
        public ILogger Logger { get; }
        public static Application Instance => _instance!;

        public static string RootDirectory => Instance._scriptHostConfiguration.RootPath;

        private readonly IScriptHostConfiguration _scriptHostConfiguration;
        private readonly GameDataProvider _gameDataProvider;
        private readonly CoreConfig _coreConfig;
        private readonly IPluginManager _pluginManager;
        private readonly IPluginContextQueryHandler _pluginContextQueryHandler;

        public Application(ILoggerFactory loggerFactory, IScriptHostConfiguration scriptHostConfiguration,
            GameDataProvider gameDataProvider, CoreConfig coreConfig, IPluginManager pluginManager,
            IPluginContextQueryHandler pluginContextQueryHandler)
        {
            Logger = loggerFactory.CreateLogger("Core");
            _scriptHostConfiguration = scriptHostConfiguration;
            _gameDataProvider = gameDataProvider;
            _coreConfig = coreConfig;
            _pluginManager = pluginManager;
            _pluginContextQueryHandler = pluginContextQueryHandler;
            _instance = this;
        }

        public void Start()
        {
            Logger.LogInformation("CounterStrikeSharp is starting up...");

            _coreConfig.Load();
            _gameDataProvider.Load();

            var adminPath = Path.Combine(_scriptHostConfiguration.RootPath, "configs", "admins.json");
            Logger.LogInformation("Loading Admins from {Path}", adminPath);
            AdminManager.LoadAdminData(adminPath);
            
            var adminGroupsPath = Path.Combine(_scriptHostConfiguration.RootPath, "configs", "admin_groups.json");
            Logger.LogInformation("Loading Admin Groups from {Path}", adminGroupsPath);
            AdminManager.LoadAdminGroups(adminGroupsPath);

            var overridePath = Path.Combine(_scriptHostConfiguration.RootPath, "configs", "admin_overrides.json");
            Logger.LogInformation("Loading Admin Command Overrides from {Path}", overridePath);
            AdminManager.LoadCommandOverrides(overridePath);

            AdminManager.MergeGroupPermsIntoAdmins();
            AdminManager.AddCommands();

            _pluginManager.Load();

            for (var i = 1; i <= 9; i++)
            {
                CommandUtils.AddStandaloneCommand("css_" + i, "Command Key Handler", (player, info) =>
                {
                    if (player == null) return;
                    var key = Convert.ToInt32(info.GetArg(0).Split("_")[1]);
                    ChatMenus.OnKeyPress(player, key);
                });
            }

            RegisterPluginCommands();
        }

        [RequiresPermissions("@css/generic")]
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

        [RequiresPermissions("@css/generic")]
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
                        $"  List of all plugins currently loaded by CounterStrikeSharp: {_pluginManager.GetLoadedPlugins().Count()} plugins loaded.",
                        true);

                    foreach (var plugin in _pluginManager.GetLoadedPlugins())
                    {
                        var sb = new StringBuilder();
                        sb.AppendFormat("  [#{0}:{1}]: \"{2}\" ({3})", plugin.PluginId,
                            plugin.State.ToString().ToUpper(), plugin.Plugin.ModuleName,
                            plugin.Plugin.ModuleVersion);
                        if (!string.IsNullOrEmpty(plugin.Plugin.ModuleAuthor))
                            sb.AppendFormat(" by {0}", plugin.Plugin.ModuleAuthor);
                        if (!string.IsNullOrEmpty(plugin.Plugin.ModuleDescription))
                        {
                            sb.Append("\n");
                            sb.Append("    ");
                            sb.Append(plugin.Plugin.ModuleDescription);
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
                        path = Path.Combine(_scriptHostConfiguration.RootPath, $"plugins/{path}/{path}.dll");
                    }
                    else
                    {
                        path = Path.Combine(_scriptHostConfiguration.RootPath, path);
                    }

                    var plugin = _pluginContextQueryHandler.FindPluginByModulePath(path);

                    if (plugin == null)
                    {
                        try
                        {
                            _pluginManager.LoadPlugin(path);
                        } catch (Exception e)
                        {
                            info.ReplyToCommand($"Could not load plugin \"{path}\")", true);
                        }
                    }
                    else
                    {
                        plugin.Load(false);
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
                    IPluginContext? plugin = _pluginContextQueryHandler.FindPluginByIdOrName(pluginIdentifier);
                    if (plugin == null)
                    {
                        info.ReplyToCommand($"Could not unload plugin \"{pluginIdentifier}\")", true);
                        break;
                    }

                    plugin.Unload(false);
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
                    var plugin = _pluginContextQueryHandler.FindPluginByIdOrName(pluginIdentifier);

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

        private void RegisterPluginCommands()
        {
            CommandUtils.AddStandaloneCommand("css", "Counter-Strike Sharp options.", OnCSSCommand);
            CommandUtils.AddStandaloneCommand("css_plugins", "Counter-Strike Sharp plugin options.",
                OnCSSPluginCommand);
        }
    }
}