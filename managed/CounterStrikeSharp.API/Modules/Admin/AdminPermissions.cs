using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Entities;
using System.Linq;
using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using System.Numerics;
using CounterStrikeSharp.API.Modules.Utils;
using System.Diagnostics.Eventing.Reader;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public partial class AdminData
    {
        [JsonPropertyName("identity")] public required string Identity { get; init; }
        // Flags loaded from file. Do not use this for actual comparisons.
        [JsonPropertyName("flags")] public HashSet<string> _flags { get; init; } = new();

        [JsonPropertyName("immunity")] public uint Immunity { get; set; } = 0;
        [JsonPropertyName("command_overrides")] public Dictionary<string, bool> CommandOverrides { get; init; } = new();

        // Key is the domain of the flag "e.g "css, os, kzsurf"). This should NOT include the @ character.
        // Value is a hashmap of the flags inside of the domain (e.g "@css/generic")
        public Dictionary<string, HashSet<string>> Flags { get; init; } = new();

        public void InitalizeFlags()
        {
            AddFlags(_flags);
        }

        /// <summary>
        /// Checks to see if a domain has a root flag inside of it.
        /// </summary>
        /// <param name="domain">Domain to check for.</param>
        /// <returns>True if "@{domain}/root" or "@{domain}/*" is present, false if not.</returns>
        public bool DomainHasRootFlag(string domain)
        {
            if (!Flags.ContainsKey(domain)) return false;
            if (Flags[domain].Contains("@" + domain + "/root")) return true;
            else if (Flags[domain].Contains("@" + domain + "/*")) return true;
            else return false;
        }

        /// <summary>
        /// Returns a list of all domains for flags.
        /// </summary>
        /// <returns></returns>
        public string[] GetFlagDomains()
        {
            return Flags.Keys.ToArray();
        }

        /// <summary>
        /// Returns a HashSet of all flags.
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetAllFlags()
        {
            var flags = new HashSet<string>();
            foreach (var domainFlags in Flags.Values)
            {
                flags.UnionWith(domainFlags);
            }
            return flags;
        }

        public void AddFlags(HashSet<string> flags)
        {
            var domains = flags.Where(
               flag => flag.StartsWith(PermissionCharacters.UserPermissionChar))
               .Distinct()
               .Select(domain => domain.Split('/').First()[1..]);

            foreach (var domain in domains)
            {
                if (!Flags.ContainsKey(domain))
                {
                    Flags[domain] = new HashSet<string>();
                }
                Flags[domain].UnionWith(flags.Where(flag => flag.StartsWith(PermissionCharacters.UserPermissionChar + domain + '/')).ToHashSet());
            }
        }

        public void RemoveFlags(HashSet<string> flags)
        {
            var domains = flags.Where(
               flag => flag.StartsWith(PermissionCharacters.UserPermissionChar))
               .Distinct()
               .Select(domain => domain.Split('/').First()[1..]);

            foreach (var domain in domains)
            {
                if (!Flags.ContainsKey(domain)) continue;
                var domainFlags = flags.Where(flag => flag.StartsWith(PermissionCharacters.UserPermissionChar + domain + '/')).ToHashSet();
                Flags[domain].ExceptWith(flags);
                if (Flags[domain].Count() == 0) Flags.Remove(domain);
            }
        }

        public bool DomainHasFlags(string domain, string[] flags, bool ignoreRoot = false)
        {
            if (!Flags.ContainsKey(domain)) return false;
            if (DomainHasRootFlag(domain) && !ignoreRoot) return true;
            return Flags[domain].IsSupersetOf(flags);
        }
    }

    public static partial class AdminManager
    {
        private static Dictionary<SteamID, AdminData> Admins = new();
        
        // TODO: ServiceCollection
        private static ILogger _logger = CoreLogging.Factory.CreateLogger("AdminManager");

        public static void LoadAdminData(string adminDataPath)
        {
            try
            {
                if (!File.Exists(adminDataPath))
                {
                    _logger.LogWarning("Admin data file not found. Skipping admin data load.");
                    return;
                }
                var settings = new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip };
                var adminsFromFile = JsonSerializer.Deserialize<Dictionary<string, AdminData>>(File.ReadAllText(adminDataPath), settings);
                if (adminsFromFile == null) { throw new FileNotFoundException(); }

                foreach (var adminDef in adminsFromFile.Values)
                {
                    adminDef.InitalizeFlags();

                    if (SteamID.TryParse(adminDef.Identity, out var steamId))
                    {
                        if (Admins.ContainsKey(steamId!))
                        {
                            // Merge domains together if we already have pre-existing values.
                            foreach (var (domain, flags) in adminDef.Flags)
                            {
                                if (Admins[steamId!].Flags.ContainsKey(domain))
                                {
                                    Admins[steamId!].Flags[domain].UnionWith(flags);
                                }
                            }

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

                _logger.LogInformation("Loaded admin data with {Count} admins.", Admins.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load admin data");
            }
        }

		/// <summary>
		/// Grabs the admin data for a player that was loaded from "configs/admins.json" and "configs/admins_groups.json".
		/// </summary>
		/// <param name="player">Player controller</param>
		/// <returns>AdminData class if data found, null if not.</returns>
		public static AdminData? GetPlayerAdminData(CCSPlayerController? player)
		{
			if (player == null) return null;
            return GetPlayerAdminData(player.AuthorizedSteamID);
		}

		/// <summary>
		/// Grabs the admin data for a player that was loaded from "configs/admins.json" and "configs/admins_groups.json".
		/// </summary>
		/// <param name="steamId">SteamID object of the player.</param>
		/// <returns>AdminData class if data found, null if not.</returns>
		public static AdminData? GetPlayerAdminData(SteamID? steamId)
        {
            if (steamId == null) return null;
            return Admins.GetValueOrDefault(steamId);
        }

		/// <summary>
		/// Removes a players admin data. This is not saved to "configs/admins.json"
		/// </summary>
		/// <param name="player">Player controller</param>
		public static void RemovePlayerAdminData(CCSPlayerController? player)
		{
			if (player == null) return;
			RemovePlayerAdminData(player.AuthorizedSteamID);
		}

		/// <summary>
		/// Removes a players admin data. This is not saved to "configs/admins.json"
		/// </summary>
		/// <param name="steamId">Steam ID remove admin data from.</param>
		public static void RemovePlayerAdminData(SteamID? steamId)
        {
            if (steamId == null) return;
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) { return false; }
            return PlayerHasPermissions(player.AuthorizedSteamID, flags);
        }

        /// <summary>
        /// Checks to see if a player has access to a certain set of permission flags.
        /// </summary>
        /// <param name="steamId">Steam ID object.</param>
        /// <param name="flags">Flags to look for in the players permission flags.</param>
        /// <returns>True if flags are present, false if not.</returns>
        public static bool PlayerHasPermissions(SteamID? steamId, params string[] flags)
        {
            if (steamId == null) return false;
            var playerData = GetPlayerAdminData(steamId);
            if (playerData == null) return false;

            // Check to see that all of the domains in the flags that we're checking are
            // present in our player data.
            var localDomains = flags.Where(
               flag => flag.StartsWith(PermissionCharacters.UserPermissionChar))
               .Distinct()
               .Select(domain => domain.Split('/').First()[1..])
               .ToHashSet();
            var playerFlagDomains = playerData.GetFlagDomains().ToHashSet();
            if (!playerFlagDomains.IsSupersetOf(localDomains)) return false;

            // Loop through all of the domains and see if we have the required flags
            // for every domain.
            bool returnValue = true;
            foreach (var domain in playerData.Flags)
            {
                if (!playerData.DomainHasFlags(domain.Key,
                    flags
                    .Where(flag => flag.StartsWith(PermissionCharacters.UserPermissionChar + domain.Key + '/'))
                    .ToArray()))
                {
                    returnValue = false; break;
                }
            }
            return returnValue;
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) { return false; }
            var playerData = GetPlayerAdminData(player.AuthorizedSteamID);
            return playerData?.CommandOverrides.ContainsKey(command) ?? false;
        }

        /// <summary>
        /// Checks to see if a player has a command override. This does NOT return the actual
        /// state of the override.
        /// </summary>
        /// <param name="steamId">Steam ID object.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <returns>True if override exists, false if not.</returns>
        public static bool PlayerHasCommandOverride(SteamID? steamId, string command)
        {
            if (steamId == null) return false;
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) { return false; }
            var playerData = GetPlayerAdminData(player.AuthorizedSteamID);
            return playerData?.CommandOverrides.GetValueOrDefault(command) ?? false;
        }

        /// <summary>
        /// Gets the value of a command override state.
        /// </summary>
        /// <param name="steamId">Steam ID object.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <returns>True if override is active, false if not.</returns>
        public static bool GetPlayerCommandOverrideState(SteamID? steamId, string command)
        {
            if (steamId == null) return false;
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) { return; }
            SetPlayerCommandOverride(player.AuthorizedSteamID, command, state);
        }

        /// <summary>
        /// Sets a player command override. This is not saved to "configs/admins.json".
        /// </summary>
        /// <param name="steamId">SteamID to add a flag to.</param>
        /// <param name="command">Name of the command to check for.</param>
        /// <param name="state">New state of the command override.</param>
        public static void SetPlayerCommandOverride(SteamID? steamId, string command, bool state)
        {
            if (steamId == null) return;
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) return;
            AddPlayerPermissions(player.AuthorizedSteamID, flags);
        }
        
        /// <summary>
        /// Temporarily adds a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamId">SteamID to add a flag to.</param>
        /// <param name="flags">Flags to add for the player.</param>
        public static void AddPlayerPermissions(SteamID? steamId, params string[] flags)
        {
            if (steamId == null) return;
            var data = GetPlayerAdminData(steamId);
            if (data == null)
            {
                data = new AdminData()
                {
                    Identity = steamId.SteamId64.ToString(),
                    Flags = new(),
                    Groups = new()
                };

                Admins[steamId] = data;
            }

            Admins[steamId].AddFlags(flags.ToHashSet<string>());
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) return;

            RemovePlayerPermissions(player.AuthorizedSteamID, flags);
        }

        /// <summary>
        /// Temporarily removes a permission flag to the player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamId">Steam ID to remove flags from.</param>
        /// <param name="flags">Flags to remove from the player.</param>
        public static void RemovePlayerPermissions(SteamID? steamId, params string[] flags)
        {
            if (steamId == null) return;
            var data = GetPlayerAdminData(steamId);
            if (data == null) return;
            Admins[steamId].RemoveFlags(flags.ToHashSet<string>());
        }

        /// <summary>
        /// Temporarily removes all permission flags from a player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="player">Player controller to remove flags from.</param>
        public static void ClearPlayerPermissions(CCSPlayerController? player)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) return;

            ClearPlayerPermissions(player.AuthorizedSteamID);
        }

        /// <summary>
        /// Temporarily removes all permission flags from a player. These flags are not saved to
        /// "configs/admins.json".
        /// </summary>
        /// <param name="steamId">Steam ID to remove flags from.</param>
        public static void ClearPlayerPermissions(SteamID? steamId)
        {
            if (steamId == null) return;
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
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) return;

            SetPlayerImmunity(player.AuthorizedSteamID, value);
        }

        /// <summary>
        /// Sets the immunity value for a player.
        /// </summary>
        /// <param name="steamId">Steam ID of the player.</param>
        /// <param name="value">New immunity value.</param>
        public static void SetPlayerImmunity(SteamID? steamId, uint value)
        {
            if (steamId == null) return;
            var data = GetPlayerAdminData(steamId);
            if (data == null) return;

            data.Immunity = value;
            Admins[steamId] = data;
        }

		/// <summary>
		/// Returns the immunity value for a player.
		/// </summary>
		/// <param name="player">Player controller.</param>
		/// <returns> If an immunity value is present in "configs/admins_groups.json" 
        /// and in "configs/admins.json", the returned value will be the greater of the two.
        /// If the value is overriden with SetPlayerImmunity, that value is returned instead.</returns>
		public static uint GetPlayerImmunity(CCSPlayerController? player)
        {
            if (player == null) return 0;
			if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) return 0;

            return GetPlayerImmunity(player.AuthorizedSteamID);
		}

		/// <summary>
		/// Returns the immunity value for a player.
		/// </summary>
		/// <param name="steamId">Steam ID of the player.</param>
		/// <returns> If an immunity value is present in "configs/admins_groups.json" 
		/// and in "configs/admins.json", the returned value will be the greater of the two.
		/// If the value is overriden with SetPlayerImmunity, that value is returned instead.</returns>
		public static uint GetPlayerImmunity(SteamID? steamId)
        {
			if (steamId == null) return 0;
			var data = GetPlayerAdminData(steamId);
			if (data == null) return 0;

            return data.Immunity;
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

            var callerData = GetPlayerAdminData(caller.AuthorizedSteamID);
            if (callerData == null) return false;

            var targetData = GetPlayerAdminData(target.AuthorizedSteamID);
            if (targetData == null) return true;

            return callerData.Immunity >= targetData.Immunity;
        }

        /// <summary>
        /// Checks to see if a player can target another player based on their immunity value.
        /// </summary>
        /// <param name="caller">Caller of the command.</param>
        /// <param name="target">Target of the command.</param>
        /// <returns></returns>
        public static bool CanPlayerTarget(SteamID? caller, SteamID? target)
        {
            if (caller == null) return false;
            if (target == null) return false;

            var callerData = GetPlayerAdminData(caller);
            if (callerData == null) return false;

            var targetData = GetPlayerAdminData(target);
            if (targetData == null) return true;

            return callerData.Immunity >= targetData.Immunity;
        }
        #endregion
    }
}
