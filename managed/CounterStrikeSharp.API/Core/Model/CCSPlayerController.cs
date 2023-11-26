using System;
using CounterStrikeSharp.API.Modules.Entities.Constants;
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

    public IntPtr GiveNamedItem(string item)
    {
        if (!PlayerPawn.IsValid) return 0;
        if (!PlayerPawn.Value.IsValid) return 0;
        if (PlayerPawn.Value.ItemServices == null) return 0;

        return VirtualFunctions.GiveNamedItem(PlayerPawn.Value.ItemServices.Handle, item, 0, 0, 0, 0);
    }

    public IntPtr GiveNamedItem(CsItem item) 
    {
        string? itemString = EnumUtils.GetEnumMemberAttributeValue(item);
        if (string.IsNullOrWhiteSpace(itemString))
        {
            return IntPtr.Zero;
        }

        return this.GiveNamedItem(itemString);
    }

    public void PrintToConsole(string message)
    {
        NativeAPI.PrintToConsole((int)EntityIndex.Value.Value, $"{message}\n\0");
    }

    public void PrintToChat(string message)
    {
        VirtualFunctions.ClientPrint(this.Handle, HudDestination.Chat, message, 0, 0, 0, 0);
    }

    public void PrintToCenter(string message)
    {
        VirtualFunctions.ClientPrint(this.Handle, HudDestination.Center, message, 0, 0, 0, 0);
    }
    
    public void PrintToCenterHtml(string message)
    {
        var @event = new EventShowSurvivalRespawnStatus(true)
        {
            LocToken = message,
            Duration = 5,
            Userid = this
        };
        @event.FireEventToClient(this);
    }

    /// <summary>
    /// Drops the active player weapon on the ground.
    /// </summary>
    public void DropActiveWeapon()
    {
        if (!PlayerPawn.IsValid) return;
        if (!PlayerPawn.Value.IsValid) return;
        if (PlayerPawn.Value.ItemServices == null) return;
        if (PlayerPawn.Value.WeaponServices == null) return;
        if (!PlayerPawn.Value.WeaponServices.ActiveWeapon.IsValid) return;

        CCSPlayer_ItemServices itemServices = new CCSPlayer_ItemServices(PlayerPawn.Value.ItemServices.Handle);
        CCSPlayer_WeaponServices weponServices = new CCSPlayer_WeaponServices(PlayerPawn.Value.WeaponServices.Handle);
        itemServices.DropActivePlayerWeapon(weponServices.ActiveWeapon.Value);
    }

    /// <summary>
    /// Removes every weapon from the player.
    /// </summary>
    public void RemoveWeapons()
    {
        if (!PlayerPawn.IsValid) return;
        if (!PlayerPawn.Value.IsValid) return;
        if (PlayerPawn.Value.ItemServices == null) return;

        CCSPlayer_ItemServices itemServices = new CCSPlayer_ItemServices(PlayerPawn.Value.ItemServices.Handle);
        itemServices.RemoveWeapons();
    }

    /// <summary>
    /// Force player suicide
    /// </summary>
    /// <param name="explode"></param>
    /// <param name="force"></param>
    public void CommitSuicide(bool explode, bool force)
    {
        if (!PlayerPawn.IsValid) return;
        if (!PlayerPawn.Value.IsValid) return;

        PlayerPawn.Value.CommitSuicide(explode, force);
    }

    /// <summary>
    /// Respawn player
    /// </summary>
    public void Respawn()
    {
        if (!PlayerPawn.IsValid) return;
        if (!PlayerPawn.Value.IsValid) return;

        VirtualFunctions.CCSPlayerPawn_Respawn(PlayerPawn.Value.Handle);
        VirtualFunction.CreateVoid<IntPtr>(Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(Handle);
    }

    public bool IsBot => ((PlayerFlags)Flags).HasFlag(PlayerFlags.FL_FAKECLIENT);

    /// <summary>
    /// Forcibly switches the team of the player, the player will remain alive and keep their weapons.
    /// </summary>
    /// <param name="team">The team to switch to</param>
    public void SwitchTeam(CsTeam team)
    {
        VirtualFunctions.SwitchTeam(this.Handle, (byte)team);
    }

    /// <summary>
    /// Switches the team of the player, has the same effect as the "jointeam" console command.
    /// <remarks>
    /// This follows gamemode rules, so this will usually cause a player suicide/loss of weapons.
    /// </remarks>
    /// </summary>
    /// <param name="team">The team to change to</param>
    public void ChangeTeam(CsTeam team)
    {
        VirtualFunction.CreateVoid<IntPtr, CsTeam>(Handle, GameData.GetOffset("CCSPlayerController_ChangeTeam"))(Handle,
            team);
    }

    /// <summary>
    /// Gets the active pawns button state. Will work even if the player is dead or observing.
    /// </summary>
    public PlayerButtons Buttons => (PlayerButtons)Pawn.Value.MovementServices!.Buttons.ButtonStates[0];
}