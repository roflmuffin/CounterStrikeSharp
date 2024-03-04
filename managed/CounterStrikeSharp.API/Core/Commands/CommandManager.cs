using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CounterStrikeSharp.API.Core.Translations;
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

        _logger.LogDebug("Registering command {Command}", definition.Name);

        if (!isRegistered)
        {
            NativeAPI.AddCommand(definition.Name, definition.Description,
                definition.ExecutableBy == CommandUsage.SERVER_ONLY,
                (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, _internalFunctionReference);
        }
    }

    public void RemoveCommand(CommandDefinition definition)
    {
        _logger.LogDebug("Removing command {Command}", definition.Name);

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
        
        using var temporaryCulture = new WithTemporaryCulture(caller.GetLanguage());

        if (_commandDefinitions.TryGetValue(name, out var handler))
        {
            foreach (var command in handler)
            {
                var methodInfo = command.Callback?.GetMethodInfo();

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
                        if (attr.Permissions.Count == 0)
                        {
                            continue;
                        }

                        attr.Command = name;
                        if (!attr.CanExecuteCommand(caller))
                        {
                            var responseStr = (attr.GetType() == typeof(RequiresPermissions)) ?
                            "You are missing the correct permissions" : "You do not have one of the correct permissions";

                            var flags = attr.Permissions.Except(adminData?.GetAllFlags() ?? new HashSet<string>());
                            flags = flags.Except(adminData?.Groups ?? new HashSet<string>());
                            info.ReplyToCommand($"[CSS] {responseStr} ({string.Join(", ", flags)}) to execute this command.");

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
                            if (caller == null || !caller.IsValid) { info.ReplyToCommand("[CSS] This command can only be executed by clients."); return; }
                            break;
                        case CommandUsage.SERVER_ONLY:
                            if (caller != null && caller.IsValid) { info.ReplyToCommand("[CSS] This command can only be executed by the server."); return; }
                            break;
                        default: throw new ArgumentException("Unrecognised CommandUsage value passed in CommandHelperAttribute.");
                    }

                    // Technically the command itself counts as the first argument, 
                    // but we'll just ignore that for this check.
                    if (helperAttribute.MinArgs != 0 && info.ArgCount - 1 < helperAttribute.MinArgs)
                    {
                        // Remove the "css_" from the beginning of the command name if it's present.
                        // Most of the time, users will be calling commands from chat.
                        var commandCalled = info.ArgByIndex(0);
                        var properCommandName = (commandCalled.StartsWith("css_")) ? commandCalled.Replace("css_", "") : commandCalled;

                        info.ReplyToCommand($"[CSS] Expected usage: \"!{properCommandName} {helperAttribute.Usage}\".");
                        return;
                    }
                }

                command.Callback?.Invoke(caller, info);
            }
        }
    }
}