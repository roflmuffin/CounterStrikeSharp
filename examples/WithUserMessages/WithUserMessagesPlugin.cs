using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.UserMessages;
using Microsoft.Extensions.Logging;

namespace WithUserMessages;

[MinimumApiVersion(80)]
public class WithUserMessagesPlugin : BasePlugin
{
    public override string ModuleName => "Example: With User Messages";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that hooks and sends User Messages";

    public override void Load(bool hotReload)
    {
        // Hooks can be added using the user message ID. In this case it's the ID for `CMsgTEFireBullets`.
        HookUserMessage(452, um =>
        {
            // Sets all weapon sounds to the sound of a silenced usp.
            um.SetUInt("weapon_id", 0);
            um.SetInt("sound_type", 9);
            um.SetUInt("item_def_index", 61);

            return HookResult.Continue;
        }, HookMode.Pre);

        HookUserMessage(118, um =>
        {
            var author = um.ReadString("param1");
            var message = um.ReadString("param2");
            Logger.LogInformation("Chat message from {Author}: {Message}", author, message);

            for (var i = 0; i < um.Recipients.Count; i++)
            {
                Logger.LogInformation("Recipient {Index}: {Name}", i, um.Recipients[i].PlayerName);
            }

            if (message.Contains("stop"))
            {
                return HookResult.Stop;
            }

            if (message.Contains("skip"))
            {
                um.Recipients.Clear();
            }

            return HookResult.Continue;
        });
    }

    [ConsoleCommand("css_shake")]
    public void OnCommandShake(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null) return;

        // UserMessage.FromPartialName is a helper method that creates a UserMessage object from a partial network name.
        // In this case, it will resolve to `CUserMessageShake`.
        var message = UserMessage.FromPartialName("Shake");

        message.SetFloat("duration", 2);
        message.SetFloat("amplitude", 5);
        message.SetFloat("frequency", 10f);
        message.SetInt("command", 0);

        if (command.GetArg(1) == "all")
        {
            message.Recipients.AddAllPlayers();
        }
        else
        {
            message.Recipients.Add(player);
        }

        message.Send();

        // You can also use an overload of `Send` to send the message to a specific player without manually creating a recipient filter.
        // message.Send(player);
    }
}
