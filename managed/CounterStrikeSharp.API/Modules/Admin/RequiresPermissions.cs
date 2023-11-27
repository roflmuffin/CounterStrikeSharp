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
            if (AdminManager.PlayerHasCommandOverride(caller, Command))
            {
                return AdminManager.GetPlayerCommandOverrideState(caller, Command);
            }
            if (!base.CanExecuteCommand(caller)) return false;

            var adminData = AdminManager.GetPlayerAdminData((SteamID)caller.SteamID);
            if (adminData != null)
            {
                // Check to see if the caller has a root flag for any of the domains in our permissions.
                // If they do, remove all of the user flags and groups that belong to the domain
                // from our permission check.
                var domains = Permissions.Where(
                    flag => flag.StartsWith(PermissionCharacters.UserPermissionChar))
                    .Distinct()
                    .Select(domain => domain.Split('/').First()[1..]);

                foreach (var domain in domains)
                {
                    if (adminData.DomainHasRootFlag(domain))
                    {
                        Permissions.RemoveWhere(flag => flag.Contains(domain + '/'));
                    }
                }
            }

            var groupPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
            var userPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));

            if (!AdminManager.PlayerInGroup(caller, groupPermissions.ToArray())) return false;
            if (!AdminManager.PlayerHasPermissions(caller, userPermissions.ToArray())) return false;

            return true;
        }
    }
}
