using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequiresPermissionsOr : BaseRequiresPermissions
    {
        public RequiresPermissionsOr(params string[] permissions) : base(permissions) { }

        public override bool CanExecuteCommand(CCSPlayerController? caller)
        {
            if (caller == null) return true;
            if (AdminManager.PlayerHasCommandOverride(caller, Command))
            {
                return AdminManager.GetPlayerCommandOverrideState(caller, Command);
            }
            if (!base.CanExecuteCommand(caller)) return false;

            var groupPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
            var userPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));

            var adminData = AdminManager.GetPlayerAdminData((SteamID)caller.SteamID);
            if (adminData == null) return false;
            return (groupPermissions.Intersect(adminData.Groups).Count() + userPermissions.Intersect(adminData.Flags).Count()) > 0;
        }
    }
}
