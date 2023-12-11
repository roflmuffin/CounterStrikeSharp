using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Utils;

public class CommandUtils
{
    public static void AddStandaloneCommand(string name, string description, CommandInfo.CommandCallback handler)
    {
        var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
        {
            var caller = (i != -1) ? new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1)) : null;
            var command = new CommandInfo(ptr, caller);

            var methodInfo = handler?.GetMethodInfo();

            // We do not need to do permission checks on commands executed from the server console.
            // The server will always be allowed to execute commands (unless marked as client only like above)
            if (caller != null)
            {
                // Do not execute command if we do not have the correct permissions.
                var adminData = AdminManager.GetPlayerAdminData(caller!.AuthorizedSteamID);
                var permissionsToCheck = new List<BaseRequiresPermissions>();


                // If our command is overriden, we dynamically create a new permissions attribute
                // based on the data that is stored in admin_overrides.json.
                if (AdminManager.CommandIsOverriden(name))
                {
                    var data = AdminManager.GetCommandOverrideData(name);
                    if (data != null)
                    {
                        var attrType = (data.CheckType == "all") ? typeof(RequiresPermissions) : typeof(RequiresPermissionsOr);
                        var attr = (BaseRequiresPermissions)Activator.CreateInstance(attrType, args: AdminManager.GetPermissionOverrides(name));

                        if (attr != null) permissionsToCheck.Add(attr);
                    }
                }
                // The permissions for this command are not being overriden here, so we
                // grab the permissions to check straight from the attribute.
                else
                {
                    var permissions = methodInfo?.GetCustomAttributes<BaseRequiresPermissions>();
                    if (permissions != null) permissionsToCheck.AddRange(permissions);
                }

                foreach (var attr in permissionsToCheck)
                {
                    attr.Command = name;
                    if (!attr.CanExecuteCommand(caller))
                    {
                        var responseStr = (attr.GetType() == typeof(RequiresPermissions)) ?
                        "You are missing the correct permissions" : "You do not have one of the correct permissions";

                        var flags = attr.Permissions.Except(adminData?.GetAllFlags() ?? new HashSet<string>());
                        flags = flags.Except(adminData?.Groups ?? new HashSet<string>());
                        command.ReplyToCommand($"[CSS] {responseStr} ({string.Join(", ", flags)}) to execute this command.");

                        return;
                    }
                }
            }

            // Do not execute if we shouldn't be calling this command.
            var helperAttribute = methodInfo?.GetCustomAttribute<CommandHelperAttribute>();
            if (helperAttribute != null)
            {
                switch (helperAttribute.WhoCanExcecute)
                {
                    case CommandUsage.CLIENT_AND_SERVER: break; // Allow command through.
                    case CommandUsage.CLIENT_ONLY:
                        if (caller == null || !caller.IsValid) { command.ReplyToCommand("[CSS] This command can only be executed by clients."); return; }
                        break;
                    case CommandUsage.SERVER_ONLY:
                        if (caller != null && caller.IsValid) { command.ReplyToCommand("[CSS] This command can only be executed by the server."); return; }
                        break;
                    default: throw new ArgumentException("Unrecognised CommandUsage value passed in CommandHelperAttribute.");
                }

                // Technically the command itself counts as the first argument, 
                // but we'll just ignore that for this check.
                if (helperAttribute.MinArgs != 0 && command.ArgCount - 1 < helperAttribute.MinArgs)
                {
                    // Remove the "css_" from the beginning of the command name if it's present.
                    // Most of the time, users will be calling commands from chat.
                    var commandCalled = command.ArgByIndex(0);
                    var properCommandName = (commandCalled.StartsWith("css_")) ? commandCalled.Replace("css_", "") : commandCalled;

                    command.ReplyToCommand($"[CSS] Expected usage: \"!{properCommandName} {helperAttribute.Usage}\".");
                    return;
                }
            }

            handler?.Invoke(caller, command);
        });

        var methodInfo = handler?.GetMethodInfo();
        var helperAttribute = methodInfo?.GetCustomAttribute<CommandHelperAttribute>();

        var subscriber = new BasePlugin.CallbackSubscriber(handler, wrappedHandler, () => { });
        NativeAPI.AddCommand(name, description, (helperAttribute?.WhoCanExcecute == CommandUsage.SERVER_ONLY), 
            (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, subscriber.GetInputArgument());
    }
}
