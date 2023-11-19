using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Commands;
using System.Reflection;
using System.Numerics;
using System.Linq;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public partial class AdminData
    {
        [JsonPropertyName("identity")] public required string Identity { get; init; }
        [JsonPropertyName("flags")] public required HashSet<string> Flags { get; init; }
        [JsonPropertyName("immunity")] public uint Immunity { get; set; } = 0;
        [JsonPropertyName("command_overrides")] public Dictionary<string, bool> CommandOverrides { get; init; } = new();
    }

    public static partial class AdminManager
    {
        private static Dictionary<SteamID, AdminData> Admins = new();

        public static void LoadAdminData(string adminDataPath)
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
                            if (adminDef.Immunity > Admins[steamId!].Immunity)
                            {
                                Admins[steamId!].Immunity = adminDef.Immunity;
                            }
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
        /// Removes a players admin data. This is not saved to "configs/admins.json"
        /// </summary>
        /// <param name="steamId">Steam ID remove admin data from.</param>
        public static void RemovePlayerAdminData(SteamID steamId)
        {
            Admins.Remove(steamId);
        }

        #region Command Permission Checks

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

        #endregion

        // This is placed here instead of in AdminCommandOverrides.cs as this all relates to admins that are
        // defined within the "configs/admins.json" file.
        #region Admin Specific Command Overrides
        /// <summary>
        /// Checks to see if a player has a command override. This does NOT return the actual
        /// state of the override.
        /// </summary>
        /// <param name="player">Player or server console.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <returns>True if override exists, false if not.</returns>
        public static bool PlayerHasCommandOverride(CCSPlayerController? player, string command)
        {
            // This is here for cases where the server console is attempting to call commands.
            // The server console should have access to all commands, regardless of permissions.
            if (player == null) return true;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return false; }
            var playerData = GetPlayerAdminData((SteamID)player.SteamID);
            return playerData?.CommandOverrides.ContainsKey(command) ?? false;
        }

        /// <summary>
        /// Checks to see if a player has a command override. This does NOT return the actual
        /// state of the override.
        /// </summary>
        /// <param name="steamId">Steam ID object.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <returns>True if override exists, false if not.</returns>
        public static bool PlayerHasCommandOverride(SteamID steamId, string command)
        {
            var playerData = GetPlayerAdminData(steamId);
            return playerData?.CommandOverrides.ContainsKey(command) ?? false;
        }

        /// <summary>
        /// Gets the value of a command override state.
        /// </summary>
        /// <param name="player">Player or server console.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <returns>True if override is active, false if not.</returns>
        public static bool GetPlayerCommandOverrideState(CCSPlayerController? player, string command)
        {
            // This is here for cases where the server console is attempting to call commands.
            // The server console should have access to all commands, regardless of permissions.
            if (player == null) return true;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return false; }
            var playerData = GetPlayerAdminData((SteamID)player.SteamID);
            return playerData?.CommandOverrides.GetValueOrDefault(command) ?? false;
        }

        /// <summary>
        /// Gets the value of a command override state.
        /// </summary>
        /// <param name="steamId">Steam ID object.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <returns>True if override is active, false if not.</returns>
        public static bool GetPlayerCommandOverrideState(SteamID steamId, string command)
        {
            var playerData = GetPlayerAdminData(steamId);
            return playerData?.CommandOverrides.GetValueOrDefault(command) ?? false;
        }

        /// <summary>
        /// Sets a player command override. This is not saved to "configs/admins.json".
        /// </summary>
        /// <param name="player">Player or server console.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <param name="state">New state of the command override.</param>
        public static void SetPlayerCommandOverride(CCSPlayerController? player, string command, bool state)
        {
            // This is here for cases where the server console is attempting to call commands.
            // The server console should have access to all commands, regardless of permissions.
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return; }
            SetPlayerCommandOverride((SteamID)player.SteamID, command, state);
        }

        /// <summary>
        /// Sets a player command override. This is not saved to "configs/admins.json".
        /// </summary>
        /// <param name="steamId">SteamID to add a flag to.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <param name="state">New state of the command override.</param>
        public static void SetPlayerCommandOverride(SteamID steamId, string command, bool state)
        {
            var data = GetPlayerAdminData(steamId);
            if (data == null)
            {
                data = new AdminData()
                {
                    Identity = steamId.SteamId64.ToString(),
                    Flags = new(),
                    Groups = new(),
                    CommandOverrides = new() { { command, state } }
                };

                Admins[steamId] = data;
                return;
            }

            data.CommandOverrides[command] = state;
            Admins[steamId] = data;
        }
        #endregion

        #region Manipulating Permissions
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
            AddPlayerPermissions((SteamID)player.SteamID, flags);
        }
        
        /// <summary>
        /// Temporarily adds a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamId">SteamID to add a flag to.</param>
        /// <param name="flags">Flags to add for the player.</param>
        public static void AddPlayerPermissions(SteamID steamId, params string[] flags)
        {
            var data = GetPlayerAdminData(steamId);
            if (data == null)
            {
                data = new AdminData()
                {
                    Identity = steamId.SteamId64.ToString(),
                    Flags = new(flags),
                    Groups = new()
                };

                Admins[steamId] = data;
                return;
            }
            
            foreach (var flag in flags)
            {
                data.Flags.Add(flag);
            }
            Admins[steamId] = data;
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

            RemovePlayerPermissions((SteamID)player.SteamID, flags);
        }

        /// <summary>
        /// Temporarily removes a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamId">Steam ID to remove flags from.</param>
        /// <param name="flags">Flags to remove from the player.</param>
        public static void RemovePlayerPermissions(SteamID steamId, params string[] flags)
        {
            var data = GetPlayerAdminData(steamId);
            if (data == null) return;
            
            data.Flags.ExceptWith(flags);
            Admins[steamId] = data;
        }

        /// <summary>
        /// Temporarily removes all permission flags from a player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="player">Player controller to remove flags from.</param>
        public static void ClearPlayerPermissions(CCSPlayerController? player)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            ClearPlayerPermissions((SteamID)player.SteamID);
        }

        /// <summary>
        /// Temporarily removes all permission flags from a player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamId">Steam ID to remove flags from.</param>
        public static void ClearPlayerPermissions(SteamID steamId)
        {
            var data = GetPlayerAdminData(steamId);
            if (data == null) return;

            data.Flags.Clear();
            Admins[steamId] = data;
        }
        #endregion

        #region Player Immunity
        /// <summary>
        /// Sets the immunity value for a player.
        /// </summary>
        /// <param name="player">Player controller.</param>
        /// <param name="value">New immunity value.</param>
        public static void SetPlayerImmunity(CCSPlayerController? player, uint value)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) return;

            SetPlayerImmunity((SteamID)player.SteamID, value);
        }

        /// <summary>
        /// Sets the immunity value for a player.
        /// </summary>
        /// <param name="steamId">Steam ID of the player.</param>
        /// <param name="value">New immunity value.</param>
        public static void SetPlayerImmunity(SteamID steamId, uint value)
        {
            var data = GetPlayerAdminData(steamId);
            if (data == null) return;

            data.Immunity = value;
            Admins[steamId] = data;
        }

        /// <summary>
        /// Checks to see if a player can target another player based on their immunity value.
        /// </summary>
        /// <param name="caller">Caller of the command.</param>
        /// <param name="target">Target of the command.</param>
        /// <returns></returns>
        public static bool CanPlayerTarget(CCSPlayerController? caller, CCSPlayerController? target)
        {
            // The server console should be able to target everyone.
            if (caller == null) return true;

            if (target == null) return false;
            if (!target.IsValid || target.Connected != PlayerConnectedState.PlayerConnected) return false;

            var callerData = GetPlayerAdminData((SteamID)caller.SteamID);
            if (callerData == null) return false;

            var targetData = GetPlayerAdminData((SteamID)target.SteamID);
            if (targetData == null) return true;

            return callerData.Immunity >= targetData.Immunity;
        }

        /// <summary>
        /// Checks to see if a player can target another player based on their immunity value.
        /// </summary>
        /// <param name="caller">Caller of the command.</param>
        /// <param name="target">Target of the command.</param>
        /// <returns></returns>
        public static bool CanPlayerTarget(SteamID caller, SteamID target)
        {
            var callerData = GetPlayerAdminData(caller);
            if (callerData == null) return false;

            var targetData = GetPlayerAdminData(caller);
            if (targetData == null) return true;

            return callerData.Immunity >= targetData.Immunity;
        }
        #endregion
    }
}
