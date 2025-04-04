using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.ClientMessages;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
namespace WithClientMessages;

[MinimumApiVersion(80)]
public class WithClientMessagesPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Client Messages";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that hooks and sends Client Messages";

    public override void Load(bool hotReload)
    {
        // Hooks can be added using the client message ID. In this case it's the ID for `CClientMsg_CustomGameEvent`.
        HookClientMessage(280, cm =>
        {
            string event_name = cm.ReadString("event_name");
            byte[] data = cm.ReadBytes("data");

            // send back
            var message = ClientMessage.FromPartialName("CClientMsg_CustomGameEvent");
            message.SetString("event_name", event_name);
            message.SetBytes("data", data);

            var player = Utilities.GetPlayerFromSlot(cm.Sender)!;

            message.Send(player);

            return HookResult.Continue;
        }, HookMode.Pre);
    }
}
