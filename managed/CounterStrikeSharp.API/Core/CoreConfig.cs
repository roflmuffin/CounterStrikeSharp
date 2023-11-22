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
using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// Serializable instance of the CoreConfig
    /// </summary>
    internal sealed partial class CoreConfigData
    {
        [JsonPropertyName("PublicChatTrigger")] public IEnumerable<string> PublicChatTrigger { get; set; } = new HashSet<string>() { "!" };

        [JsonPropertyName("SilentChatTrigger")] public IEnumerable<string> SilentChatTrigger { get; set; } = new HashSet<string>() { "/" };

        [JsonPropertyName("FollowCS2ServerGuidelines")] public bool FollowCS2ServerGuidelines { get; set; } = true;
    }

    /// <summary>
    /// Configuration related to the Core API.
    /// </summary>
    public static partial class CoreConfig
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
    }

    public static partial class CoreConfig
    {
        private static CoreConfigData _coreConfig = new CoreConfigData();
        
        // TODO: ServiceCollection
        private static ILogger _logger = CoreLogging.Factory.CreateLogger("CoreConfig");

        static CoreConfig()
        {
            CommandUtils.AddStandaloneCommand("css_core_reload", "Reloads the core configuration file.", ReloadCoreConfigCommand);
        }

        [RequiresPermissions("@css/config")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadCoreConfigCommand(CCSPlayerController? player, CommandInfo command)
        {
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            Load(Path.Combine(rootDir.FullName, "configs", "core.json"));
        }

        public static void Load(string coreConfigPath)
        {
            if (!File.Exists(coreConfigPath))
            {
                _logger.LogWarning("Core configuration could not be found at path \"{CoreConfigPath}\", fallback values will be used.", coreConfigPath);
                return;
            }

            try
            {
                var data = JsonSerializer.Deserialize<CoreConfigData>(File.ReadAllText(coreConfigPath), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });

                if (data != null)
                {
                    _coreConfig = data;
                }
                
                _logger.LogInformation("Successfully loaded core configuration.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load core configuration, fallback values will be used");
            }
        }
    }
}
