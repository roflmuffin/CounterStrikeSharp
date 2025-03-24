using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequiresPermissions : BaseRequiresPermissions
    {
        public RequiresPermissions(params string[] permissions) : base(permissions) { }

        public override bool CanExecuteCommand(SteamID? steamID)
        {
            if (steamID == null) return true;
            if (AdminManager.PlayerHasCommandOverride(steamID, Command))
            {
                return AdminManager.GetPlayerCommandOverrideState(steamID, Command);
            }
            if (!base.CanExecuteCommand(steamID)) return false;

            var groupPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.GroupPermissionChar));
            var userPermissions = Permissions.Where(perm => perm.StartsWith(PermissionCharacters.UserPermissionChar));

            if (!AdminManager.PlayerHasPermissions(steamID, userPermissions.ToArray()))
            {
                return false;
            }
            if (!AdminManager.PlayerInGroup(steamID, groupPermissions.ToArray()))
            {
                return false;
            }

            return true;
        }
    }
}
