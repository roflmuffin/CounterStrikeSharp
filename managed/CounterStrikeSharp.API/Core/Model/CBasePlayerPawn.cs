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
    public void CommitSuicide(bool explode, bool force)
    {
        VirtualFunction.CreateVoid<IntPtr, bool, bool>(Handle,  GameData.GetOffset("CBasePlayerPawn_CommitSuicide"))(Handle, explode, force);
    }
    
    /// <summary>
    /// Remove Player Item
    /// </summary>
    /// <param name="weapon"></param>
    public void RemovePlayerItem(CBasePlayerWeapon weapon)
    {
        VirtualFunctions.RemovePlayerItemVirtual(Handle, weapon.Handle);
    }
}