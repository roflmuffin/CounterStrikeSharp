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
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// Serializable instance of the CoreConfig
    /// </summary>
    internal sealed partial class CoreConfigData
    {
        [JsonPropertyName("PublicChatTrigger")]
        public IEnumerable<string> PublicChatTrigger { get; set; } = new HashSet<string>() { "!" };

        [JsonPropertyName("SilentChatTrigger")]
        public IEnumerable<string> SilentChatTrigger { get; set; } = new HashSet<string>() { "/" };

        [JsonPropertyName("FollowCS2ServerGuidelines")]
        public bool FollowCS2ServerGuidelines { get; set; } = true;

        [JsonPropertyName("PluginHotReloadEnabled")]
        public bool PluginHotReloadEnabled { get; set; } = true;

        [JsonPropertyName("PluginAutoLoadEnabled")]
        public bool PluginAutoLoadEnabled { get; set; } = true;

        [JsonPropertyName("PluginResolveNugetPackages")]
        public bool PluginResolveNugetPackages { get; set; }

        [JsonPropertyName("ServerLanguage")]
        public string ServerLanguage { get; set; } = "en";

        [JsonPropertyName("UnlockConCommands")]
        public bool UnlockConCommands { get; set; } = true;

        [JsonPropertyName("UnlockConVars")]
        public bool UnlockConVars { get; set; } = true;

        [JsonPropertyName("AutoUpdateEnabled")]
        public bool AutoUpdateEnabled { get; set; } = true;

        [JsonPropertyName("AutoUpdateURL")]
        public string AutoUpdateURL { get; set; } = "http://gamedata.cssharp.dev";
    }

    /// <summary>
    /// Configuration related to the Core API.
    /// </summary>
    public partial class CoreConfig
    {
        /// <summary>
        /// List of characters to use for public chat triggers.
        /// </summary>
        public static IEnumerable<string> PublicChatTrigger => _coreConfig.PublicChatTrigger;

        /// <summary>
        /// List of characters to use for silent chat triggers.
        /// </summary>
        public static IEnumerable<string> SilentChatTrigger => _coreConfig.SilentChatTrigger;

        /// <summary>
        /// <para>
        /// Per <see href="http://blog.counter-strike.net/index.php/server_guidelines/"/>, certain plugin
        /// functionality will trigger all of the game server owner's Game Server Login Tokens
        /// (GSLTs) to get banned when executed on a Counter-Strike 2 game server.
        /// </para>
        ///
        /// <para>
        /// Enabling this option will block plugins from using functionality that is known to cause this.
        ///
        /// Note that this does NOT guarantee that you cannot
        ///
        /// receive a ban.
        /// </para>
        ///
        /// <para>
        /// Disable this option at your own risk.
        /// </para>
        /// </summary>
        public static bool FollowCS2ServerGuidelines => _coreConfig.FollowCS2ServerGuidelines;

        /// <summary>
        /// When enabled, plugins are automatically reloaded when their .dll file is updated.
        /// </summary>
        public static bool PluginHotReloadEnabled => _coreConfig.PluginHotReloadEnabled;

        /// <summary>
        /// When enabled, plugins are automatically loaded from the plugins directory on server start.
        /// </summary>
        public static bool PluginAutoLoadEnabled => _coreConfig.PluginAutoLoadEnabled;

        public static bool PluginResolveNugetPackages => _coreConfig.PluginResolveNugetPackages;

        public static string ServerLanguage => _coreConfig.ServerLanguage;

        public static bool UnlockConCommands => _coreConfig.UnlockConCommands;

        public static bool UnlockConVars => _coreConfig.UnlockConVars;

    }

    public partial class CoreConfig : IStartupService
    {
        private static CoreConfigData _coreConfig = new CoreConfigData();

        private readonly ICommandManager _commandManager;
        private readonly ILogger<CoreConfig> _logger;

        private readonly string _coreConfigPath;
        private bool _commandsRegistered = false;

        public CoreConfig(IScriptHostConfiguration scriptHostConfiguration, ICommandManager commandManager, ILogger<CoreConfig> logger)
        {
            _commandManager = commandManager;
            _logger = logger;
            _coreConfigPath = Path.Join(scriptHostConfiguration.ConfigsPath, "core.json");
        }

        [RequiresPermissions("@css/config")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private void ReloadCoreConfigCommand(CCSPlayerController? player, CommandInfo command)
        {
            Load();
        }

        public void Load()
        {
            if (!_commandsRegistered)
            {
                _commandManager.RegisterCommand(new CommandDefinition("css_core_reload",
                    "Reloads the core configuration file.",
                    ReloadCoreConfigCommand));
                _commandsRegistered = true;
            }

            if (File.Exists(_coreConfigPath))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<CoreConfigData>(File.ReadAllText(_coreConfigPath),
                        new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });

                    if (data != null)
                    {
                        _coreConfig = data;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load core configuration, fallback values will be used");
                }
            }
            else
            {
                _logger.LogWarning(
                    "Core configuration could not be found at path \"{CoreConfigPath}\", fallback values will be used.",
                    _coreConfigPath);
            }

            var serverCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(x => x.Name == ServerLanguage);
            if (serverCulture == null)
            {
                try
                {
                    _logger.LogWarning("Server Language \"{ServerLanguage}\" is not supported, falling back to \"en\"",
                        ServerLanguage);
                    _coreConfig.ServerLanguage = "en";
                    serverCulture = new CultureInfo("en");
                }
                catch (Exception)
                {
                    _logger.LogWarning("Server is running in invariant mode, translations will not be available.");
                    serverCulture = CultureInfo.InvariantCulture;
                }
            }

            CultureInfo.DefaultThreadCurrentUICulture = serverCulture;
            CultureInfo.DefaultThreadCurrentCulture = serverCulture;
            CultureInfo.CurrentUICulture = serverCulture;
            CultureInfo.CurrentCulture = serverCulture;

            _logger.LogInformation("Successfully loaded core configuration");
        }
    }
}
