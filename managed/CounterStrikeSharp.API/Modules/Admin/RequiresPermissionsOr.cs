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

            var adminData = AdminManager.GetPlayerAdminData(caller.AuthorizedSteamID);
            if (adminData == null) return false;
            
            // Check to see if the caller has a root flag for any of the domains in our permissions.
            // If they do, remove all of the user flags and groups that belong to the domain
            // from our permission check.
            var domains = Permissions.Where(
                flag => flag.StartsWith(PermissionCharacters.GroupPermissionChar))
                .Distinct()
                .Select(domain => domain.Split('/').First()[1..]);

            foreach (var domain in domains)
            {
                if (adminData.DomainHasRootFlag(domain))
                {
                    Permissions.RemoveWhere(flag => flag.Contains(domain + '/'));
                }
            }

            var groupPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
            var userPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));
            return (groupPermissions.Intersect(adminData.Groups).Count() + userPermissions.Intersect(adminData.GetAllFlags()).Count()) > 0;
        }
    }
}
