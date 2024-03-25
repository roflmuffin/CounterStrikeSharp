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

using System.Globalization;
using System.Linq;
using System.Text;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Core.Plugin.Host;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IPlayerLanguageManager _playerLanguageManager;
        private readonly ICommandManager _commandManager;

        public Application(ILoggerFactory loggerFactory, IScriptHostConfiguration scriptHostConfiguration,
            GameDataProvider gameDataProvider, CoreConfig coreConfig, IPluginManager pluginManager,
            IPluginContextQueryHandler pluginContextQueryHandler, IPlayerLanguageManager playerLanguageManager,
            ICommandManager commandManager)
        {
            Logger = loggerFactory.CreateLogger("Core");
            _scriptHostConfiguration = scriptHostConfiguration;
            _gameDataProvider = gameDataProvider;
            _coreConfig = coreConfig;
            _pluginManager = pluginManager;
            _pluginContextQueryHandler = pluginContextQueryHandler;
            _playerLanguageManager = playerLanguageManager;
            _commandManager = commandManager;
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

            RegisterPluginCommands();

            _pluginManager.Load();

            for (var i = 1; i <= 9; i++)
            {
                _commandManager.RegisterCommand(new($"css_{i}", "Command Key Handler",
                    (player, info) =>
                    {
                        if (player == null) return;
                        var key = Convert.ToInt32(info.GetArg(0).Split("_")[1]);
                        MenuManager.OnKeyPress(player, key);
                    }));
            }
        }

        [RequiresPermissions("@css/generic")]
        private void OnCSSCommand(CCSPlayerController? caller, CommandInfo info)
        {
            var versionString = $"v{Api.GetVersion()} ({Api.GetVersionString()})";

            info.ReplyToCommand(
                "  CounterStrikeSharp was created and is maintained by Michael \"roflmuffin\" Wilson.\n" +
                "  Counter-Strike Sharp uses code borrowed from SourceMod, Source.Python, FiveM, Saul Rennison, source2gen and CS2Fixes.\n" +
                "  See ACKNOWLEDGEMENTS.md for more information.\n" +
                "  Current API Version: " + versionString);
            return;
        }

        [RequiresPermissions("@css/generic")]
        private void OnCSSPluginCommand(CCSPlayerController? caller, CommandInfo info)
        {
            switch (info.GetArg(1))
            {
                case "list":
                {
                    info.ReplyToCommand(
                        $"  List of all plugins currently loaded by CounterStrikeSharp: {_pluginManager.GetLoadedPlugins().Count()} plugins loaded.");

                    foreach (var plugin in _pluginManager.GetLoadedPlugins())
                    {
                        var sb = new StringBuilder();
                        sb.AppendFormat("  [#{0}:{1}]: \"{2}\" ({3})", plugin.PluginId,
                            plugin.State.ToString().ToUpper(), plugin.Plugin?.ModuleName ?? "Unknown",
                            plugin.Plugin?.ModuleVersion ?? "Unknown");
                        if (!string.IsNullOrEmpty(plugin.Plugin?.ModuleAuthor))
                            sb.AppendFormat(" by {0}", plugin.Plugin.ModuleAuthor);
                        if (!string.IsNullOrEmpty(plugin.Plugin?.ModuleDescription))
                        {
                            sb.Append("\n");
                            sb.Append("    ");
                            sb.Append(plugin.Plugin.ModuleDescription);
                        }

                        info.ReplyToCommand(sb.ToString());
                    }

                    break;
                }
                case "start":
                case "load":
                {
                    if (info.ArgCount < 3)
                    {
                        info.ReplyToCommand(
                            "Valid usage: css_plugins start/load [relative plugin path || absolute plugin path] (e.g \"TestPlugin\", \"plugins/TestPlugin/TestPlugin.dll\")\n");
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
                            plugin = _pluginContextQueryHandler.FindPluginByModulePath(path);
                            plugin.Plugin.OnAllPluginsLoaded(false);
                        }
                        catch (Exception e)
                        {
                            info.ReplyToCommand($"Could not load plugin \"{path}\"");
                            Logger.LogError(e, "Could not load plugin \"{Path}\"", path);
                        }
                    }
                    else
                    {
                        plugin.Load(false);
                        plugin.Plugin.OnAllPluginsLoaded(false);
                    }

                    break;
                }

                case "stop":
                case "unload":
                {
                    if (info.ArgCount < 3)
                    {
                        info.ReplyToCommand(
                            "Valid usage: css_plugins stop/unload [plugin name || #plugin id] (e.g \"TestPlugin\", \"1\")\n");
                        break;
                    }

                    var pluginIdentifier = info.GetArg(2);
                    IPluginContext? plugin = _pluginContextQueryHandler.FindPluginByIdOrName(pluginIdentifier);
                    if (plugin == null)
                    {
                        info.ReplyToCommand($"Could not unload plugin \"{pluginIdentifier}\"");
                        break;
                    }

                    plugin.Unload(false);
                    break;
                }

                case "restart":
                case "reload":
                {
                    if (info.ArgCount < 3)
                    {
                        info.ReplyToCommand(
                            "Valid usage: css_plugins restart/reload [plugin name || #plugin id] (e.g \"TestPlugin\", \"#1\")\n");
                        break;
                    }

                    var pluginIdentifier = info.GetArg(2);
                    var plugin = _pluginContextQueryHandler.FindPluginByIdOrName(pluginIdentifier);

                    if (plugin == null)
                    {
                        info.ReplyToCommand($"Could not reload plugin \"{pluginIdentifier}\"");
                        break;
                    }

                    plugin.Unload(true);
                    plugin.Load(true);
                    plugin.Plugin.OnAllPluginsLoaded(true);
                    break;
                }

                default:
                    info.ReplyToCommand("Valid usage: css_plugins [option]\n" +
                                        "  list - List all plugins currently loaded.\n" +
                                        "  start / load - Loads a plugin not currently loaded.\n" +
                                        "  stop / unload - Unloads a plugin currently loaded.\n" +
                                        "  restart / reload - Reloads a plugin currently loaded.");
                    break;
            }
        }
        
        private void OnLangCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            var steamId = (SteamID)player.SteamID;

            if (command.ArgCount == 1)
            {
                var language = _playerLanguageManager.GetLanguage(steamId);
                command.ReplyToCommand($"Current language is \"{language.Name}\" ({language.NativeName})");
                return;
            }

            if (command.ArgCount != 2)
            {
                return;
            }

            try
            {
                var language = command.GetArg(1);
                var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).Single(x => x.Name == language);
                _playerLanguageManager.SetLanguage(steamId, cultureInfo);
                command.ReplyToCommand($"Language set to {cultureInfo.NativeName}");
            }
            catch (InvalidOperationException)
            {
                command.ReplyToCommand("Language not found.");
            }
        }

        private void RegisterPluginCommands()
        {
            _commandManager.RegisterCommand(new("css", "Counter-Strike Sharp options.", OnCSSCommand)
            {
                ExecutableBy = CommandUsage.CLIENT_AND_SERVER,
            });
            _commandManager.RegisterCommand(new("css_plugins", "Counter-Strike Sharp plugin options.",
                OnCSSPluginCommand)
            {
                ExecutableBy = CommandUsage.CLIENT_AND_SERVER,
                MinArgs = 1,
                UsageHint = "[option]\n" +
                            "  list - List all plugins currently loaded.\n" +
                            "  start / load - Loads a plugin not currently loaded.\n" +
                            "  stop / unload - Unloads a plugin currently loaded.\n" +
                            "  restart / reload - Reloads a plugin currently loaded.",
            });
            _commandManager.RegisterCommand(new("css_lang", "Set Counter-Strike Sharp language.", OnLangCommand)
            {
                ExecutableBy = CommandUsage.CLIENT_AND_SERVER,
                UsageHint = "[language code, e.g. \"de\", \"pl\", \"en\"]",
            });
        }
    }
}