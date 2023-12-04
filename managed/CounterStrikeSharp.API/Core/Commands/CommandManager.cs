using System.Collections.Generic;
using System.Reflection;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Commands;

public class CommandManager : ICommandManager
{
    private readonly Dictionary<string, IList<CommandDefinition>> _commandDefinitions =
        new(StringComparer.InvariantCultureIgnoreCase);

    private readonly ILogger<CommandManager> _logger;
    private readonly FunctionReference _internalFunctionReference;

    public CommandManager(ILogger<CommandManager> logger)
    {
        _logger = logger;
        _internalFunctionReference = FunctionReference.Create(HandleCommandInternal);
    }

    public void RegisterCommand(CommandDefinition definition)
    {
        bool isRegistered = true;
        if (!_commandDefinitions.ContainsKey(definition.Name))
        {
            _commandDefinitions.Add(definition.Name, new List<CommandDefinition>());
            isRegistered = false;
        }

        _commandDefinitions[definition.Name].Add(definition);

        _logger.LogDebug("Registering standalone command {Command}", definition.Name);

        if (!isRegistered)
        {
            NativeAPI.AddCommand(definition.Name, definition.Description,
                definition.ExecutableBy == CommandUsage.SERVER_ONLY,
                (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, _internalFunctionReference);
        }
    }

    public void RemoveCommand(CommandDefinition definition)
    {
        _logger.LogDebug("Removing standalone command {Command}", definition.Name);

        if (_commandDefinitions.TryGetValue(definition.Name, out var commandDefinition))
        {
            commandDefinition.Remove(definition);
        }

        if (_commandDefinitions[definition.Name].Count == 0)
        {
            NativeAPI.RemoveCommand(definition.Name, _internalFunctionReference);
            _commandDefinitions.Remove(definition.Name);
        }
    }

    private void HandleCommandInternal(int playerSlot, IntPtr commandInfo)
    {
        var caller = (playerSlot != -1) ? Utilities.GetPlayerFromSlot(playerSlot) : null;
        var info = new CommandInfo(commandInfo, caller);

        var name = info.GetArg(0).ToLower();

        if (_commandDefinitions.TryGetValue(name, out var handler))
        {
            foreach (var command in handler)
            {
                var methodInfo = command.Callback?.GetMethodInfo();

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
                                info.ReplyToCommand(
                                    "[CSS] You do not have the correct permissions to execute this command.");
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
                        var attrType = (data.CheckType == "all")
                            ? typeof(RequiresPermissions)
                            : typeof(RequiresPermissionsOr);
                        var attr = (BaseRequiresPermissions)Activator.CreateInstance(attrType,
                            args: AdminManager.GetPermissionOverrides(name));
                        attr.Command = name;
                        if (!attr.CanExecuteCommand(caller))
                        {
                            info.ReplyToCommand(
                                "[CSS] You do not have the correct permissions to execute this command.");
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
                            if (caller == null || !caller.IsValid)
                            {
                                info.ReplyToCommand("[CSS] This command can only be executed by clients.");
                                return;
                            }

                            break;
                        case CommandUsage.SERVER_ONLY:
                            if (caller != null && caller.IsValid)
                            {
                                info.ReplyToCommand("[CSS] This command can only be executed by the server.");
                                return;
                            }

                            break;
                        default:
                            throw new ArgumentException(
                                "Unrecognised CommandUsage value passed in CommandHelperAttribute.");
                    }

                    // Technically the command itself counts as the first argument, 
                    // but we'll just ignore that for this check.
                    if (command.MinArgs != 0 && info.ArgCount - 1 < command.MinArgs)
                    {
                        info.ReplyToCommand(
                            $"[CSS] Expected usage: \"!{name} {command.UsageHint}\".");
                        return;
                    }
                }

                command.Callback?.Invoke(caller, info);
            }
        }
    }
}