using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public partial class AdminData
    {
        [JsonPropertyName("auth_type")] public string AuthType { get; set; }
        [JsonPropertyName("identity")] public string Identity { get; set; }
        [JsonPropertyName("flags")] public uint Flags { get; set; }
    }

    public static class AdminManager
    {
        private static Dictionary<string, AdminData> _admins;
        public const uint AllFlags = uint.MaxValue;

        public static void Load(string adminDataPath)
        {
            try
            {
                _admins = JsonSerializer.Deserialize<Dictionary<string, AdminData>>(File.ReadAllText(adminDataPath));

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
        /// <param name="player">Player controller entity. Passing null will treat the player as the server console instead.</param>
        /// <returns>AdminData class if data found, null if not.</returns>
        public static AdminData? GetPlayerAdminData(CCSPlayerController? player)
        {
            // This is here for cases where the server console is attempting to call commands.
            // The server console should have access to all commands, regardless of permissions.
            if (player == null)
            {
                var adminData = new AdminData();
                adminData.Flags = AllFlags;
                return adminData;
            }

            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected) { return null; }

            foreach (var admin in _admins)
            {
                var authType = admin.Value.AuthType;
                switch (authType)
                {
                    case "steamid64": if (new SteamID(admin.Value.Identity).SteamId64 == new SteamID(player.SteamID).SteamId64) return admin.Value; break;
                    case "steamid32": if (new SteamID(admin.Value.Identity).SteamId32 == new SteamID(player.SteamID).SteamId32) return admin.Value; break;
                    case "steamid3": if (new SteamID(admin.Value.Identity).SteamId3 == new SteamID(player.SteamID).SteamId3) return admin.Value; break;
                    case "steamid2": if (new SteamID(admin.Value.Identity).SteamId2 == new SteamID(player.SteamID).SteamId2) return admin.Value; break;
                    case "name": if (player.PlayerName == admin.Value.Identity) return admin.Value; break;
                }
            }

            // For players who have no admin data, we just return null.
            return null;
        }

        /// <summary>
        /// Grabs the admin data for a player that was loaded from "configs/admins.json".
        /// </summary>
        /// <param name="adminName">Name of the admin. This value is set by the key of the AdminData object in the JSON file.</param>
        /// <returns>AdminData class if data found, null if not.</returns>
        public static AdminData? GetPlayerAdminData(string adminName)
        {
            return _admins.GetValueOrDefault(adminName);
        }

        /// <summary>
        /// Checks to see if a player has access to a certain set of permission flags.
        /// </summary>
        /// <param name="player">Player or server console.</param>
        /// <param name="flags">Bit flags to look for in the players permission flags.</param>
        /// <returns>True if flags are present, false if not.</returns>
        public static bool PlayerHasPermissions(CCSPlayerController? player, int flags)
        {
            if (player != null && (player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)) { return false; }
            var data = GetPlayerAdminData(player);
            if (data == null) return false;
            return (flags & data.Flags) != 0;
        }
    }
}
