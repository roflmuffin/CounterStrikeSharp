using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.IO;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public class PermissionCharacters
    {
        // Example: "#css/admin"
        public const string GroupPermissionChar = "#";
        // Example: "@css/manipulate_players"
        public const string UserPermissionChar = "@";
    }

    public static partial class AdminManager
    {
        static AdminManager()
        {
            CommandUtils.AddStandaloneCommand("css_admins_reload", "Reloads the admin file.", ReloadAdminsCommand);
            CommandUtils.AddStandaloneCommand("css_admins_list", "List admins and their flags.", ListAdminsCommand);
        }

        [RequiresPermissions("@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            Admins.Clear();
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            LoadAdminData(Path.Combine(rootDir.FullName, "configs", "admins.json"));
        }

        [RequiresPermissions("@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ListAdminsCommand(CCSPlayerController? player, CommandInfo command)
        {
            foreach (var (steamId, data) in Admins)
            {
                command.ReplyToCommand($"{steamId.SteamId64}, {steamId.SteamId2} - {string.Join(", ", data.Flags)}");
            }
        }

        [RequiresPermissions("@css/generic")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private static void ReloadAdminGroupsCommand(CCSPlayerController? player, CommandInfo command)
        {
            Groups.Clear();
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent;
            LoadAdminGroups(Path.Combine(rootDir.FullName, "configs", "admin_groups.json"));
        }
    }
}
