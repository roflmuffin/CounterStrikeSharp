using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public class BaseRequiresPermissions : Attribute
    {
        public string[] Permissions { get; }
        public string Command { get; set; }

        public BaseRequiresPermissions(params string[] permissions)
        {
            Permissions = permissions;
            Command = "";
        }

        public virtual bool CanExecuteCommand(CCSPlayerController? caller)
        {
            // If we have a command in the "command_overrides" section in "configs/admins.json",
            // we skip the checks below and just return the defined value.
            var adminData = AdminManager.GetPlayerAdminData((SteamID)caller.SteamID);
            if (adminData == null) return false;
            if (adminData.CommandOverrides.ContainsKey(Command))
            {
                return adminData.CommandOverrides[Command];
            }

            return true;
        }
    }
}
