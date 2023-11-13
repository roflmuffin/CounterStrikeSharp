using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Admin
{

    public static partial class AdminManager
    {
        static AdminManager()
        {
            CommandUtils.AddStandaloneCommand("css_admins_reload", "Reloads the admin file.", ReloadAdminsCommand);
            CommandUtils.AddStandaloneCommand("css_admins_list", "List admins and their flags.", ListAdminsCommand);
            CommandUtils.AddStandaloneCommand("css_groups_reload", "Reloads the admin groups file.", ReloadAdminGroupsCommand);
            CommandUtils.AddStandaloneCommand("css_groups_list", "List admin groups and their flags.", ListAdminGroupsCommand);
        }

        public static void MergeGroupPermsIntoAdmins()
        {
            foreach (var (steamID, adminDef) in Admins)
            {
                AddPlayerToGroup(steamID, adminDef.Groups.ToArray());
            }
        }

        [RequiresPermissions(permissions:"@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            Admins.Clear();
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            LoadAdminData(Path.Combine(rootDir.FullName, "configs", "admins.json"));
        }

        [RequiresPermissions(permissions:"@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ListAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            foreach (var (steamId, data) in Admins)
            {
                command.ReplyToCommand($"{steamId.SteamId64}, {steamId.SteamId2} - {string.Join(", ", data.Flags)}");
            }
        }

        [RequiresPermissions(permissions:"@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadAdminGroupsCommand(CCSPlayerController? player, CommandInfo command)
        {
            Groups.Clear();
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            LoadAdminGroups(Path.Combine(rootDir.FullName, "configs", "admin_groups.json"));
        }

        [RequiresPermissions(permissions: "@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ListAdminGroupsCommand(CCSPlayerController? player, CommandInfo command)
        {
            foreach (var (groupName, groupDef) in Groups)
            {
                command.ReplyToCommand($"{groupName} - {string.Join(", ", groupDef.Flags)}");
            }
        }
    }
}
