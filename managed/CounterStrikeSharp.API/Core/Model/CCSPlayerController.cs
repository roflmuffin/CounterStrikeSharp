using System;

using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
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
            return NativeAPI.GetUseridFromIndex((int)this.Index);
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
        NativeAPI.PrintToConsole((int)Index, $"{message}\n\0");
    }

    public void PrintToChat(string message)
    {
        VirtualFunctions.ClientPrint(this.Handle, HudDestination.Chat, message, 0, 0, 0, 0);
    }

    public void PrintToCenter(string message)
    {
        VirtualFunctions.ClientPrint(this.Handle, HudDestination.Center, message, 0, 0, 0, 0);
    }

    public void PrintToCenterHtml(string message) => PrintToCenterHtml(message, 5);
    
    public void PrintToCenterHtml(string message, int duration)
    {
        var @event = new EventShowSurvivalRespawnStatus(true)
        {
            LocToken = message,
            Duration = duration,
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
    /// Get a ConVar value for given player
    /// </summary>
    /// <param name="conVar">Name of the convar to retrieve</param>
    /// <returns>ConVar string value</returns>
    public string GetConVarValue(string conVar)
    {
        return NativeAPI.GetClientConvarValue(this.Slot, conVar);
    }

    public string GetConVarValue(ConVar? conVar)
    {
        if (conVar == null)
        {
            throw new Exception("Invalid convar passed to 'GetConVarValue'");
        }

        return GetConVarValue(conVar.Name);
    }

    /// <summary>
    /// Sets a ConVar value on a fake client (bot).
    /// </summary>
    /// <param name="conVar">Console variable name</param>
    /// <param name="value">String value to set</param>
    /// <exception cref="InvalidOperationException">Player is not a bot</exception>
    public void SetFakeClientConVar(string conVar, string value)
    {
        if (!IsBot)
        {
            throw new InvalidOperationException("'SetFakeClientConVar' can only be called for fake clients (bots)");
        }

        NativeAPI.SetFakeClientConvarValue(this.Slot, conVar, value);
    }

    /// <summary>
    /// <inheritdoc cref="SetFakeClientConVar(string,string)"/>
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="conVar"/> is <see langword="null"/></exception>
    /// <inheritdoc cref="SetFakeClientConVar(string,string)" select="exception"/>
    public void SetFakeClientConVar(ConVar conVar, string value)
    {
        if (conVar == null)
        {
            throw new ArgumentException("Invalid convar passed to 'SetFakeClientConVar'");
        }

        SetFakeClientConVar(conVar.Name, value);
    }

    /// <summary>
    /// Gets the active pawns button state. Will work even if the player is dead or observing.
    /// </summary>
    public PlayerButtons Buttons => (PlayerButtons)Pawn.Value.MovementServices!.Buttons.ButtonStates[0];

    public void ExecuteClientCommand(string command) => NativeAPI.IssueClientCommand(Slot, command);

    /// <summary>
    /// Overrides who a player can hear in voice chat.
    /// </summary>
    /// <param name="sender">Player talking in the voice chat</param>
    /// <param name="override">Whether the talker should be heard</param>
    public void SetListenOverride(CCSPlayerController sender, ListenOverride @override)
    {
        NativeAPI.SetClientListening(Handle, sender.Handle, (Byte)@override);
    }
    
    public ListenOverride GetListenOverride(CCSPlayerController sender)
    {
        return NativeAPI.GetClientListening(Handle, sender.Handle);
    }

    public int Slot => (int)Index - 1;

    /// <summary>
    /// Returns the authorized SteamID of this user which has been validated with the SteamAPI.
    /// </summary>
    public SteamID? AuthorizedSteamID
    {
        get
        {
            if (!this.IsValid) return null;
            var authorizedSteamId = NativeAPI.GetPlayerAuthorizedSteamid(this.Slot);
            if ((long)authorizedSteamId == -1) return null;

            return (SteamID)authorizedSteamId;
        }
    }
    
    /// <summary>
    /// Returns the IP address (and possibly port) of this player.
    /// <remarks>Returns 127.0.0.1 if the player is a bot.</remarks>
    /// </summary>
    public string? IpAddress
    {
        get
        {
            if (!this.IsValid) return null;
            var ipAddress = NativeAPI.GetPlayerIpAddress(this.Slot);
            if (string.IsNullOrWhiteSpace(ipAddress)) return null;

            return ipAddress;
        }
    }

    /// <summary>
    /// Determines how the player interacts with voice chat.
    /// </summary>
    public VoiceFlags VoiceFlags
    {
        get => (VoiceFlags)NativeAPI.GetClientVoiceFlags(Handle);
        set
        {
            NativeAPI.SetClientVoiceFlags(Handle, (Byte)value);
        }
    }
}