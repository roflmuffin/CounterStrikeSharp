﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System;
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

            if (!AdminManager.CommandIsOverriden(name))
            {
                // Do not execute command if we do not have the correct permissions.
                var permissions = methodInfo?.GetCustomAttributes<BaseRequiresPermissions>();
                if (permissions != null)
                {
                    foreach (var attr in permissions)
                    {
                        attr.Command = name;
                        if (!attr.CanExecuteCommand(caller))
                        {
                            command.ReplyToCommand("[CSS] You do not have the correct permissions to execute this command.");
                            return;
                        }
                    }
                }
            }
            // If this command has it's permissions overriden, we will do an AND check for all permissions.
            else
            {
                // I don't know if this is the most sane implementation of this, can be edited in code review.
                var data = AdminManager.GetCommandOverrideData(name);
                if (data != null) 
                {
                    var attrType = (data.CheckType == "all") ? typeof(RequiresPermissions) : typeof(RequiresPermissionsOr);
                    var attr = (BaseRequiresPermissions)Activator.CreateInstance(attrType, args: AdminManager.GetPermissionOverrides(name));
                    attr.Command = name;
                    if (!attr.CanExecuteCommand(caller))
                    {
                        command.ReplyToCommand("[CSS] You do not have the correct permissions to execute this command.");
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
                    command.ReplyToCommand($"[CSS] Expected usage: \"!{command.ArgByIndex(0)} {helperAttribute.Usage}\".");
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
