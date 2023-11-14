using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequiresPermissions : BaseRequiresPermissions
    {
        public RequiresPermissions(params string[] permissions) : base(permissions) { }

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

            if (!AdminManager.PlayerInGroup(caller, groupPermissions.ToArray())) return false;
            if (!AdminManager.PlayerHasPermissions(caller, userPermissions.ToArray())) return false;

            return true;
        }
    }
}
