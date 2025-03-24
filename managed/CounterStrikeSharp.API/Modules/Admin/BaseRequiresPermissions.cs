﻿using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public class BaseRequiresPermissions : Attribute
    {
        /// <summary>
        /// The permissions for the command.
        /// </summary>
        public HashSet<string> Permissions { get; }
        /// <summary>
        /// The name of the command that is attached to this attribute.
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// Whether this attribute should be used for permission checks.
        /// </summary>
        public bool Enabled { get; set; }

        public BaseRequiresPermissions(params string[] permissions)
        {
            Permissions = permissions.ToHashSet();
            Command = "";
        }

        public virtual bool CanExecuteCommand(SteamID? steamID)
        {
            if (steamID is null) return false;

            var adminData = AdminManager.GetPlayerAdminData(steamID);
            if (adminData is null) return false;

            // If we have a command in the "command_overrides" section in "configs/admins.json",
            // we skip the checks below and just return the defined value.
            if (adminData.CommandOverrides.TryGetValue(Command, out var command))
            {
                return command;
            }

            return true;
        }

        public virtual bool CanExecuteCommand(CCSPlayerController? caller)
        {
            if ( caller is null || caller is { IsValid: false } ) return false;
            if ( caller.AuthorizedSteamID is null ) return false;

            return CanExecuteCommand(caller.AuthorizedSteamID);
        }
    }
}
