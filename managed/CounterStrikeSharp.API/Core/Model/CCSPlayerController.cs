using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerController
{
    public int? UserId
    {
        get
        {
            if (EntityIndex == null) return null;
            return NativeAPI.GetUseridFromIndex((int)this.EntityIndex.Value.Value);
        }
    }

    public void GiveNamedItem(string item)
    {
        if (!PlayerPawn.IsValid) return;
        if (!PlayerPawn.Value.IsValid) return;
        if (PlayerPawn.Value.ItemServices == null) return;

        VirtualFunctions.GiveNamedItem(PlayerPawn.Value.ItemServices.Handle, item, 0, 0, 0, 0);
    }

    public void SwitchTeam(CsTeam team)
    {
        VirtualFunctions.SwitchTeam(this.Handle, (byte)team);
    }

    public void PrintToConsole(string message)
    {
        NativeAPI.PrintToConsole((int)EntityIndex.Value.Value, message);
    }

    public void PrintToChat(string message)
    {
        VirtualFunctions.ClientPrint(this.Handle, HudDestination.Chat, message, 0, 0, 0, 0);
    }

    public void PrintToCenter(string message)
    {
        VirtualFunctions.ClientPrint(this.Handle, HudDestination.Center, message, 0, 0, 0, 0);
    }
}