using System;

using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerController
{
    public int? UserId => NativeAPI.GetUseridFromIndex((int)Index);
    public CsTeam Team => (CsTeam)TeamNum;

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public IntPtr GiveNamedItem(string item)
    {
        Guard.IsValidEntity(this);

        if (!PlayerPawn.IsValid) return 0;
        if (PlayerPawn.Value == null) return 0;
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

        return GiveNamedItem(itemString);
    }

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void PrintToConsole(string message)
    {
        Guard.IsValidEntity(this);

        NativeAPI.PrintToConsole((int)Index, $"{message}\n\0");
    }

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void PrintToChat(string message)
    {
        Guard.IsValidEntity(this);

        VirtualFunctions.ClientPrint(Handle, HudDestination.Chat, message, 0, 0, 0, 0);
    }

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void PrintToCenter(string message)
    {
        Guard.IsValidEntity(this);

        VirtualFunctions.ClientPrint(Handle, HudDestination.Center, message, 0, 0, 0, 0);
    }

    public void PrintToCenterHtml(string message) => PrintToCenterHtml(message, 5);

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void PrintToCenterHtml(string message, int duration)
    {
        Guard.IsValidEntity(this);

        var @event = new EventShowSurvivalRespawnStatus(true)
        {
            LocToken = message,
            Duration = duration,
            Userid = this
        };
        @event.FireEventToClient(this);
        @event.Free();
    }

    /// <summary>
    /// Drops the active player weapon on the ground.
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void DropActiveWeapon()
    {
        Guard.IsValidEntity(this);
        if (!PlayerPawn.IsValid) return;
        if (PlayerPawn.Value == null) return;
        if (!PlayerPawn.Value.IsValid) return;
        if (PlayerPawn.Value.ItemServices == null) return;
        if (PlayerPawn.Value.WeaponServices == null) return;
        if (!PlayerPawn.Value.WeaponServices.ActiveWeapon.IsValid) return;

        CCSPlayer_ItemServices itemServices = new CCSPlayer_ItemServices(PlayerPawn.Value.ItemServices.Handle);
        CCSPlayer_WeaponServices weaponServices = new CCSPlayer_WeaponServices(PlayerPawn.Value.WeaponServices.Handle);
        
        itemServices.DropActivePlayerWeapon(weaponServices.ActiveWeapon.Value);
    }

    /// <summary>
    /// Removes every weapon from the player.
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void RemoveWeapons()
    {
        Guard.IsValidEntity(this);
        if (!PlayerPawn.IsValid) return;
        if (PlayerPawn.Value == null) return;
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
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void CommitSuicide(bool explode, bool force)
    {
        Guard.IsValidEntity(this);
        if (!PlayerPawn.IsValid) return;
        if (PlayerPawn.Value == null) return;
        if (!PlayerPawn.Value.IsValid) return;

        PlayerPawn.Value.CommitSuicide(explode, force);
    }

    /// <summary>
    /// Respawn player
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void Respawn()
    {
        Guard.IsValidEntity(this);
        if (!PlayerPawn.IsValid) return;
        if (PlayerPawn.Value == null) return;
        if (!PlayerPawn.Value.IsValid) return;

        // The Call To Arms update appears to have invalidated the need for CCSPlayerPawn_Respawn.
        SetPawn(PlayerPawn.Value);
        VirtualFunction.CreateVoid<IntPtr>(Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(Handle);
    }

    public bool IsBot => ((PlayerFlags)Flags).HasFlag(PlayerFlags.FL_FAKECLIENT);

    /// <summary>
    /// Forcibly switches the team of the player, the player will remain alive and keep their weapons.
    /// </summary>
    /// <param name="team">The team to switch to</param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void SwitchTeam(CsTeam team)
    {
        Guard.IsValidEntity(this);

        VirtualFunctions.SwitchTeam(Handle, (byte)team);
    }

    /// <summary>
    /// Switches the team of the player, has the same effect as the "jointeam" console command.
    /// <remarks>
    /// This follows gamemode rules, so this will usually cause a player suicide/loss of weapons.
    /// </remarks>
    /// </summary>
    /// <param name="team">The team to change to</param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void ChangeTeam(CsTeam team)
    {
        Guard.IsValidEntity(this);

        VirtualFunction.CreateVoid<IntPtr, CsTeam>(Handle, GameData.GetOffset("CCSPlayerController_ChangeTeam"))(Handle,
            team);
    }

    /// <summary>
    /// Get a ConVar value for given player
    /// </summary>
    /// <param name="conVar">Name of the convar to retrieve</param>
    /// <returns>ConVar string value</returns>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public string GetConVarValue(string conVar)
    {
        Guard.IsValidEntity(this);

        return NativeAPI.GetClientConvarValue(Slot, conVar);
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
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    /// <exception cref="InvalidOperationException">Player is not a bot</exception>
    public void SetFakeClientConVar(string conVar, string value)
    {
        Guard.IsValidEntity(this);
        if (!IsBot) throw new InvalidOperationException("'SetFakeClientConVar' can only be called for fake clients (bots)");

        NativeAPI.SetFakeClientConvarValue(Slot, conVar, value);
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

    /// <summary>
    /// Issue the specified command to the specified client (mimics that client typing the command at the console).
    /// Note: Only works for some commands, marked with the FCVAR_CLIENT_CAN_EXECUTE flag (not many).
    /// </summary>
    /// <param name="command"></param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void ExecuteClientCommand(string command)
    {
        Guard.IsValidEntity(this);

        NativeAPI.IssueClientCommand(Slot, command);
    }

    /// <summary>
    /// Issue the specified command directly from the server (mimics the server executing the command with the given player context).
    /// <remarks>Works with server commands like `kill`, `explode`, `noclip`, etc. </remarks>
    /// </summary>
    /// <param name="command"></param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void ExecuteClientCommandFromServer(string command)
    {
        Guard.IsValidEntity(this);

        NativeAPI.IssueClientCommandFromServer(Slot, command);
    }

    /// <summary>
    /// Overrides who a player can hear in voice chat.
    /// </summary>
    /// <param name="sender">Player talking in the voice chat</param>
    /// <param name="override">Whether the talker should be heard</param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void SetListenOverride(CCSPlayerController sender, ListenOverride @override)
    {
        Guard.IsValidEntity(this);

        NativeAPI.SetClientListening(Handle, sender.Handle, (Byte)@override);
    }

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public ListenOverride GetListenOverride(CCSPlayerController sender)
    {
        Guard.IsValidEntity(this);

        return NativeAPI.GetClientListening(Handle, sender.Handle);
    }

    public int Slot => (int)Index - 1;

    /// <summary>
    /// Returns the authorized SteamID of this user which has been validated with the SteamAPI.
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public SteamID? AuthorizedSteamID
    {
        get
        {
            Guard.IsValidEntity(this);

            var authorizedSteamId = NativeAPI.GetPlayerAuthorizedSteamid(Slot);
            if ((long)authorizedSteamId == -1) return null;

            return (SteamID)authorizedSteamId;
        }
    }

    /// <summary>
    /// Returns the IP address (and possibly port) of this player.
    /// <remarks>Returns 127.0.0.1 if the player is a bot.</remarks>
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public string? IpAddress
    {
        get
        {
            Guard.IsValidEntity(this);

            var ipAddress = NativeAPI.GetPlayerIpAddress(Slot);
            if (string.IsNullOrWhiteSpace(ipAddress)) return null;

            return ipAddress;
        }
    }

    /// <summary>
    /// Determines how the player interacts with voice chat.
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public VoiceFlags VoiceFlags
    {
        get
        {
            Guard.IsValidEntity(this);

            return (VoiceFlags)NativeAPI.GetClientVoiceFlags(Handle);
        }
        set
        {
            Guard.IsValidEntity(this);

            NativeAPI.SetClientVoiceFlags(Handle, (Byte)value);
        }
    }
}
