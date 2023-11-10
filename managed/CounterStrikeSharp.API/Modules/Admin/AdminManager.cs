﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Commands;
using System.Linq;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public partial class AdminData
    {
        [JsonPropertyName("auth_type")] public string AuthType { get; set; }
        [JsonPropertyName("identity")] public string Identity { get; set; }
        [JsonPropertyName("flags")] public List<string> Flags { get; set; }
    }

    public static class AdminManager
    {
        private static Dictionary<SteamID, AdminData> _admins;

        static AdminManager()
        {
            _admins = new Dictionary<SteamID, AdminData>();
            CommandUtils.AddStandaloneCommand("css_admins_reload", "Reloads the admin file.", ReloadAdminsCommand);
        }

        [PermissionHelper("can_reload_admins")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            _admins.Clear();
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            Load(Path.Combine(rootDir.FullName, "configs", "admins.json"));
        }

        public static void Load(string adminDataPath)
        {
            try
            {
                var adminsFromFile = JsonSerializer.Deserialize<Dictionary<string, AdminData>>(File.ReadAllText(adminDataPath));
                if (adminsFromFile == null) { throw new FileNotFoundException(); }
                foreach (var adminDef in adminsFromFile.Values)
                {
                    var authType = adminDef.AuthType;
                    var steamidFromFile = (authType == "steamid64") ? new SteamID(ulong.Parse(adminDef.Identity)) : new SteamID(adminDef.Identity);
                    _admins.Add(steamidFromFile, adminDef);
                }

                Console.WriteLine($"Loaded admin data with {_admins.Count} admins.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load admin data: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Grabs the admin data for a player that was loaded from "configs/admins.json".
        /// </summary>
        /// <param name="steamid">SteamID object of the player.</param>
        /// <returns>AdminData class if data found, null if not.</returns>
        public static AdminData? GetPlayerAdminData(SteamID steamid)
        {
            return _admins.GetValueOrDefault(steamid);
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
            return PlayerHasPermissions(new SteamID(player.SteamID));
        }

        /// <summary>
        /// Checks to see if a player has access to a certain set of permission flags.
        /// </summary>
        /// <param name="steamid">Steam ID object.</param>
        /// <param name="flags">Flags to look for in the players permission flags.</param>
        /// <returns>True if flags are present, false if not.</returns>
        public static bool PlayerHasPermissions(SteamID steamid, params string[] flags)
        {
            var data = GetPlayerAdminData(steamid);
            if (data == null) return false;

            return data.Flags.Intersect(flags).Count() == flags.Count();
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
            AddPlayerPermissions(new SteamID(player.SteamID), flags);
        }

        /// <summary>
        /// Temporarily adds a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="player">SteamID to add a flag to.</param>
        /// <param name="flags">Flags to add for the player.</param>
        public static void AddPlayerPermissions(SteamID steamID, params string[] flags)
        {
            var data = GetPlayerAdminData(steamID);
            if (data == null)
            {
                data = new AdminData();
                data.Flags = new List<string>();
                data.Flags.AddRange(flags);

                data.AuthType = "steamid64";
                data.Identity = steamID.SteamId64.ToString();

                _admins[steamID] = data;
                return;
            }
            else
            {
                foreach (var flag in flags)
                {
                    if (!data.Flags.Contains(flag)) data.Flags.Add(flag);
                }
            }
        }

        /// <summary>
        /// Temporarily removes a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="player">Player controller to remove flags from.</param>
        /// <param name="flags">Flags to remove from the player.</param>
        public static void RemovePlayerPermissions(CCSPlayerController? player, params string[] flags)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            RemovePlayerPermissions(new SteamID(player.SteamID));
        }

        /// <summary>
        /// Temporarily removes a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamiD">Steam ID to remove flags from.</param>
        /// <param name="flags">Flags to remove from the player.</param>
        public static void RemovePlayerPermissions(SteamID steamID, params string[] flags)
        {
            var data = GetPlayerAdminData(steamID);
            if (data == null) return;
            foreach (var flag in flags)
            {
                if (data.Flags.Contains(flag)) data.Flags.Remove(flag);
            }
        }

        /// <summary>
        /// Removes a players admin data. This is not saved to "configs/admins.json"
        /// </summary>
        /// <param name="player">Player controller to remove admin data from.</param>
        public static void RemovePlayerAdminData(CCSPlayerController? player)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            RemovePlayerAdminData(new SteamID(player.SteamID));
            return;
        }

        /// <summary>
        /// Removes a players admin data. This is not saved to "configs/admins.json"
        /// </summary>
        /// <param name="steamid">Steam ID remove admin data from.</param>
        public static void RemovePlayerAdminData(SteamID steamID)
        {
            if (_admins.ContainsKey(steamID)) _admins.Remove(steamID);
            return;
        }
    }
}
