using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Commands;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public partial class AdminData
    {
        [JsonPropertyName("identity")] public required string Identity { get; init; }
        [JsonPropertyName("flags")] public required HashSet<string> Flags { get; init; }
    }

    public static class AdminManager
    {
        private static readonly Dictionary<SteamID, AdminData> Admins = new();

        static AdminManager()
        {
            CommandUtils.AddStandaloneCommand("css_admins_reload", "Reloads the admin file.", ReloadAdminsCommand);
            CommandUtils.AddStandaloneCommand("css_admins_list", "List admins and their flags.", ListAdminsCommand);
        }

        [PermissionHelper("can_reload_admins")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            Admins.Clear();
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            Load(Path.Combine(rootDir.FullName, "configs", "admins.json"));
        }
        
        [PermissionHelper("can_reload_admins")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ListAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            foreach (var (steamId, data) in Admins)
            {
                command.ReplyToCommand($"{steamId.SteamId64}, {steamId.SteamId2} - {string.Join(", ", data.Flags)}");
            }
        }

        public static void Load(string adminDataPath)
        {
            try
            {
                if (!File.Exists(adminDataPath))
                {
                    Console.WriteLine("Admin data file not found. Skipping admin data load.");
                    return;
                }
                
                var adminsFromFile = JsonSerializer.Deserialize<Dictionary<string, AdminData>>(File.ReadAllText(adminDataPath), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
                if (adminsFromFile == null) { throw new FileNotFoundException(); }
                foreach (var adminDef in adminsFromFile.Values)
                {
                    if (SteamID.TryParse(adminDef.Identity, out var steamId))
                    {
                        if (Admins.ContainsKey(steamId!))
                        {
                            Admins[steamId!].Flags.UnionWith(adminDef.Flags);
                        }
                        else
                        {
                            Admins.Add(steamId!, adminDef);
                        }
                    }
                }

                Console.WriteLine($"Loaded admin data with {Admins.Count} admins.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load admin data: {ex}");
            }
        }

        /// <summary>
        /// Grabs the admin data for a player that was loaded from "configs/admins.json".
        /// </summary>
        /// <param name="steamId">SteamID object of the player.</param>
        /// <returns>AdminData class if data found, null if not.</returns>
        public static AdminData? GetPlayerAdminData(SteamID steamId)
        {
            return Admins.GetValueOrDefault(steamId);
        }

        /// <summary>
        /// Checks to see if a player has access to a certain set of permission flags.
        /// </summary>
        /// <param name="player">Player or server console.</param>
        /// <param name="flags">Flags to look for in the players permission flags.</param>
        /// <returns>True if flags are present, false if not.</returns>
        public static bool PlayerHasPermissions(CCSPlayerController? player, params string[] flags)
        {
            // This is here for cases where the server console is attempting to call commands.
            // The server console should have access to all commands, regardless of permissions.
            if (player == null) return true;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return false; }
            var playerData = GetPlayerAdminData((SteamID)player.SteamID);
            return playerData?.Flags.IsSupersetOf(flags) ?? false;
        }

        /// <summary>
        /// Checks to see if a player has access to a certain set of permission flags.
        /// </summary>
        /// <param name="steamId">Steam ID object.</param>
        /// <param name="flags">Flags to look for in the players permission flags.</param>
        /// <returns>True if flags are present, false if not.</returns>
        public static bool PlayerHasPermissions(SteamID steamId, params string[] flags)
        {
            var playerData = GetPlayerAdminData(steamId);
            return playerData?.Flags.IsSupersetOf(flags) ?? false;
        }
        
        /// <summary>
        /// Temporarily adds a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="player">Player controller to add a flag to.</param>
        /// <param name="flags">Flags to add for the player.</param>
        public static void AddPlayerPermissions(CCSPlayerController? player, params string[] flags)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            var steamId = new SteamID(player.SteamID);
            var data = GetPlayerAdminData(steamId);
            if (data == null)
            {
                data = new AdminData()
                {
                    Identity = steamId.SteamId64.ToString(),
                    Flags = new(flags)
                };

                Admins[steamId] = data;
                return;
            }
            
            foreach (var flag in flags)
            {
                data.Flags.Add(flag);
            }
        }

        /// <summary>
        /// Temporarily removes a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="player">Player controller to add a flag to.</param>
        /// <param name="flags">Flags to remove from the player.</param>
        public static void RemovePlayerPermissions(CCSPlayerController? player, params string[] flags)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            var data = GetPlayerAdminData(new SteamID(player.SteamID));
            if (data == null) return;
            
            data.Flags.ExceptWith(flags);
        }

        /// <summary>
        /// Removes a players admin data. This is not saved to "configs/admins.json"
        /// </summary>
        /// <param name="player">Player controller to remove admin data from.</param>
        public static void RemovePlayerAdminData(CCSPlayerController? player)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            RemovePlayerAdminData((SteamID)player.SteamID);
        }

        /// <summary>
        /// Removes a players admin data. This is not saved to "configs/admins.json"
        /// </summary>
        /// <param name="steamId">Steam ID remove admin data from.</param>
        public static void RemovePlayerAdminData(SteamID steamId)
        {
            Admins.Remove(steamId);
        }
    }
}
