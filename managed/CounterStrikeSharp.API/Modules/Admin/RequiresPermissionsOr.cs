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

        public override bool CanExecuteCommand(SteamID? steamID)
        {
            if (steamID == null) return true;
            if (AdminManager.PlayerHasCommandOverride(steamID, Command))
            {
                return AdminManager.GetPlayerCommandOverrideState(steamID, Command);
            }
            if (!base.CanExecuteCommand(steamID)) return false;

            var adminData = AdminManager.GetPlayerAdminData(steamID);
            if (adminData == null) return false;
            
            var domains = Permissions
                .Select(domain => domain.Split('/').First()[1..])
                .Distinct();

            // If we have a root flag in any of the domains, return true early.
            foreach (var domain in domains)
            {
                if (adminData.DomainHasRootFlag(domain))
                {
                    return true;
                }
            }

            var groupPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
            var userPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));
            return (groupPermissions.Intersect(adminData.Groups).Count() + userPermissions.Intersect(adminData.GetAllFlags()).Count()) > 0;
        }
    }
}
