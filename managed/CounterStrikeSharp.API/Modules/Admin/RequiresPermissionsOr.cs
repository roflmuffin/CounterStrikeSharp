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
            if (!base.CanExecuteCommand(caller)) return false;

            // this probably isn't the best thing to set for groupPermissions and userPermissions,
            // but it can always be changed ;)
            var groupPermissions = Enumerable.Empty<string>();
            var userPermissions = Enumerable.Empty<string>();

            if (AdminManager.CommandIsOverriden(Command))
            {
                var newPerms = AdminManager.GetPermissionOverrides(Command);
                groupPermissions = newPerms.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
                userPermissions = newPerms.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));
            }
            else
            {
                groupPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
                userPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));
            }

            var adminData = AdminManager.GetPlayerAdminData((SteamID)caller.SteamID);
            if (adminData == null) return false;
            return (groupPermissions.Intersect(adminData.Groups).Count() + userPermissions.Intersect(adminData.Flags).Count()) > 0;
        }
    }
}
