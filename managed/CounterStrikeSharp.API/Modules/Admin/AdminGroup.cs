using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public partial class AdminData
    {
        [JsonPropertyName("groups")] public HashSet<string> Groups { get; init; } = new();
    }
    public partial class AdminGroupData
    {
        [JsonPropertyName("flags")] public required HashSet<string> Flags { get; init; }
        [JsonPropertyName("immunity")] public uint Immunity { get; set; } = 0;
        [JsonPropertyName("command_overrides")] public Dictionary<string, bool> CommandOverrides { get; init; } = new();
    }

    public static partial class AdminManager
    {
        private static Dictionary<string, AdminGroupData> Groups = new();

        public static void LoadAdminGroups(string adminGroupsPath)
        {
            try
            {
                if (!File.Exists(adminGroupsPath))
                {
                    Console.WriteLine("Admin groups file not found. Skipping admin groups load.");
                    return;
                }

                var groupsFromFile = JsonSerializer.Deserialize<Dictionary<string, AdminGroupData>>(File.ReadAllText(adminGroupsPath), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
                if (groupsFromFile == null) { throw new FileNotFoundException(); }
                foreach (var (key, groupDef) in groupsFromFile)
                {
                    if (Groups.ContainsKey(key))
                    {
                        Groups[key].Flags.UnionWith(groupDef.Flags);
                        if (groupDef.Immunity > Groups[key].Immunity)
                        {
                            Groups[key].Immunity = groupDef.Immunity;
                        }
                    }
                    else
                    {
                        Groups.Add(key, groupDef);
                    }
                }

                Console.WriteLine($"Loaded {Groups.Count} admin groups.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load admin groups: {ex}");
            }

            // Loop over each of the admins. If one of our admins is in a group,
            // add the flags from the group to their admin definition and change
            // the admin's immunity if it's higher.
            foreach (var (admin, data) in Admins)
            {
                var groups = data.Groups;
                foreach (var group in groups)
                {
                    // roflmuffin is probably smart enough to condense this function down ;)
                    if (Groups.TryGetValue(group, out var groupData))
                    {
                        AddPlayerPermissions(admin, groupData.Flags.ToArray());
                        if (groupData.Immunity > data.Immunity)
                        {
                            data.Immunity = groupData.Immunity;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if the player is part of an admin group.
        /// </summary>
        /// <param name="player">Player controller.</param>
        /// <param name="groups">Groups to check for.</param>
        /// <returns>True if a player is part of all of the groups provided, false if not.</returns>
        public static bool PlayerInGroup(CCSPlayerController? player, params string[] groups)
        {
            // This is here for cases where the server console is attempting to call commands.
            // The server console should have access to all commands, regardless of groups.
            if (player == null) return true;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return false; }

            var playerData = GetPlayerAdminData(player.AuthorizedSteamID);
            if (playerData == null) return false;

            var playerGroups = groups.ToHashSet();
            foreach (var domain in playerData.Flags)
            {
                if (playerData.DomainHasRootFlag(domain.Key))
                {
                    playerGroups.ExceptWith(groups.Where(group => group.Contains(domain.Key + '/')));
                }
            }

            return playerData.Groups.IsSupersetOf(playerGroups);
        }

        /// <summary>
        /// Checks to see if the player is part of an admin group.
        /// </summary>
        /// <param name="steamId">SteamID of the player.</param>
        /// <param name="groups">Groups to check for.</param>
        /// <returns>True if a player is part of all of the groups provided, false if not.</returns>
        public static bool PlayerInGroup(SteamID? steamId, params string[] groups)
        {
            if (steamId == null) return false;
            var playerData = GetPlayerAdminData(steamId);
            if (playerData == null) return false;

            var playerGroups = groups.ToHashSet<string>();
            foreach (var domain in playerData.Flags)
            {
                if (playerData.DomainHasRootFlag(domain.Key))
                {
                    playerGroups.ExceptWith(groups.Where(group => group.Contains(domain.Key + '/')));
                }
            }

            return playerData.Groups.IsSupersetOf(playerGroups);
        }

        /// <summary>
        /// Adds a player to a group.
        /// </summary>
        /// <param name="player">Player controller.</param>
        /// <param name="groups">Groups to add the player to.</param>
        public static void AddPlayerToGroup(CCSPlayerController? player, params string[] groups)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return; }
            AddPlayerToGroup(player.AuthorizedSteamID, groups);
        }

        /// <summary>
        /// Adds a player to a group.
        /// </summary>
        /// <param name="steamId">SteamID of the player.</param>
        /// <param name="groups">Groups to add the player to.</param>
        public static void AddPlayerToGroup(SteamID? steamId, params string[] groups)
        {
            if (steamId == null) return;
            var data = GetPlayerAdminData(steamId);
            if (data == null)
            {
                data = new AdminData()
                {
                    Identity = steamId.SteamId64.ToString(),
                    Flags = new(),
                    Groups = new(groups)
                };
            }

            foreach (var group in groups)
            {
                if (Groups.TryGetValue(group, out var groupDef))
                {
                    AddPlayerPermissions(steamId, groupDef.Flags.ToArray());
                    groupDef.CommandOverrides.ToList().ForEach(x => data.CommandOverrides[x.Key] = x.Value);
                }
            }
            Admins[steamId] = data;
        }

        /// <summary>
        /// Removes a player from a group.
        /// </summary>
        /// <param name="player">Player controller.</param>
        /// <param name="removeInheritedFlags">If true, all of the flags that the player inherited from being in the group will be removed.</param>
        /// <param name="groups"></param>
        public static void RemovePlayerFromGroup(CCSPlayerController? player, bool removeInheritedFlags = true, params string[] groups)
        {
            if (player == null) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot) { return; }
            RemovePlayerFromGroup(player.AuthorizedSteamID, true, groups);
        }

        /// <summary>
        /// Removes a player from a group.
        /// </summary>
        /// <param name="steamId">SteamID of the player.</param>
        /// <param name="removeInheritedFlags">If true, all of the flags that the player inherited from being in the group will be removed.</param>
        /// <param name="groups"></param>
        public static void RemovePlayerFromGroup(SteamID? steamId, bool removeInheritedFlags = true, params string[] groups)
        {
            if (steamId == null) return;
            var data = GetPlayerAdminData(steamId);
            if (data == null) return;

            data.Groups.ExceptWith(groups);

            if (removeInheritedFlags)
            {
                foreach (var group in groups)
                {
                    if (Groups.TryGetValue(group, out var groupDef))
                    {
                        RemovePlayerPermissions(steamId, groupDef.Flags.ToArray());
                    }
                }
            }
            Admins[steamId] = data;
        }
    }
}
