using System;
using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CBasePlayerPawn
{
    /// <summary>
    /// Force player suicide
    /// </summary>
    /// <param name="explode"></param>
    /// <param name="force"></param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void CommitSuicide(bool explode, bool force)
    {
        Guard.IsValidEntity(this);

        VirtualFunction.CreateVoid<IntPtr, bool, bool>(Handle,  GameData.GetOffset("CBasePlayerPawn_CommitSuicide"))(Handle, explode, force);
    }

    /// <summary>
    /// Remove Player Item
    /// </summary>
    /// <param name="weapon"></param>
    /// <exception cref="InvalidOperationException">The pawn or weapon is invalid, or the pawn has no weapon services on Windows.</exception>
    public void RemovePlayerItem(CBasePlayerWeapon weapon)
    {
        Guard.IsValidEntity(this);
        Guard.IsValidEntity(weapon);

        // The Windows signature resolves to a method on the pawn's weapon services.
        var receiver = OperatingSystem.IsWindows()
            ? WeaponServices?.Handle ?? throw new InvalidOperationException("Pawn has no weapon services.")
            : Handle;
        VirtualFunctions.RemovePlayerItemVirtual(receiver, weapon.Handle);
    }
}