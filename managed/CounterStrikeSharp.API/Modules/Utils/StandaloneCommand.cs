using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Utils;

public class CommandUtils
{
    public static void AddStandaloneCommand(string name, string description, CommandInfo.CommandCallback handler, bool serverOnly)
    {
        var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
        {
            if (i == -1)
            {
                handler?.Invoke(null, new CommandInfo(ptr, null));
                return;
            }

            if (serverOnly) return;

            var entity = new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1));
            var command = new CommandInfo(ptr, entity);

            // Do not execute command if we do not have the correct permissions.
            var methodInfo = handler?.GetMethodInfo();
            var attr = methodInfo?.GetCustomAttribute(typeof(PermissionHelperAttribute), true) as PermissionHelperAttribute;
            if (attr != null && !AdminManager.PlayerHasPermissions(entity, attr.RequiredPermissions))
            {
                entity.PrintToChat("[CSS] You do not have the correct permissions to execute this command.");
                return;
            }

            handler?.Invoke(entity.IsValid ? entity : null, command);
        });

        var subscriber = new BasePlugin.CallbackSubscriber(handler, wrappedHandler, () => { });
        NativeAPI.AddCommand(name, description, serverOnly, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND,
            subscriber.GetInputArgument());
    }
}
