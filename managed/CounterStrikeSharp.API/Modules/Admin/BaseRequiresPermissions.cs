using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public class BaseRequiresPermissions : Attribute
    {
        /// <summary>
        /// The permissions for the command.
        /// </summary>
        public string[] Permissions { get; }
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
            Permissions = permissions;
            Command = "";
        }

        public virtual bool CanExecuteCommand(CCSPlayerController? caller)
        {
            // If we have a command in the "command_overrides" section in "configs/admins.json",
            // we skip the checks below and just return the defined value.
            if (caller?.AuthorizedSteamID == null) return false;
            var adminData = AdminManager.GetPlayerAdminData(caller.AuthorizedSteamID);
            if (adminData == null) return false;
            if (adminData.CommandOverrides.ContainsKey(Command))
            {
                return adminData.CommandOverrides[Command];
            }

            return true;
        }
    }
}
