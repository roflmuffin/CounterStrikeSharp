using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace WithCommands;

[MinimumApiVersion(80)]
public class WithCommandsPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Commands";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that registers some commands";

    public override void Load(bool hotReload)
    {
        // All commands that are prefixed with "css_" will automatically be registered as a chat command without the prefix.
        // i.e. `css_ping` can be called with `!ping` or `/ping`.
        // Commands can be registered using the instance `AddCommand` method.
        AddCommand("css_ping", "Responds to the caller with \"pong\"", (player, commandInfo) =>
        {
            // The player is null, then the command has been called by the server console.
            if (player == null)
            {
                commandInfo.ReplyToCommand("pong server");
                return;
            }

            commandInfo.ReplyToCommand("pong");
        });
    }

    // Commands can also be registered using the `Command` attribute.
    [ConsoleCommand("css_hello", "Responds to the caller with \"pong\"")]
    // The `CommandHelper` attribute can be used to provide additional information about the command.
    [CommandHelper(minArgs: 1, usage: "[name]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/cvar")]
    public void OnHelloCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        // The first argument is the command name, in this case "css_hello".
        commandInfo.GetArg(0); // css_hello

        // The second argument is the first argument passed to the command, in this case "name".
        // The `minArgs` helper parameter is used to ensure that the second argument is present.
        var name = commandInfo.GetArg(1);

        commandInfo.ReplyToCommand($"Hello {name}");
    }
    
    // Permissions can be added to commands using the `RequiresPermissions` attribute.
    // See the admin documentation for more information on permissions.
    [RequiresPermissions("@css/kick")]
    [CommandHelper(minArgs: 1, usage: "[id]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnSpecialCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        var id = commandInfo.GetArg(1);
        
        Server.ExecuteCommand($"kick {id}");
    }
}