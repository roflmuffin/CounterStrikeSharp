using System;
using CounterStrikeSharp.API.Core.Bindings;
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
    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.player/commit-suicide</c> is disabled</exception>
    public void CommitSuicide(bool explode, bool force)
    {
        var offset = EngineFeatures.Current.GetOffset(ManagedEngineFeatures.PlayerCommitSuicide, "CBasePlayerPawn_CommitSuicide");
        Guard.IsValidEntity(this);

        VirtualFunction.CreateVoid<IntPtr, bool, bool>(Handle, offset)(Handle, explode, force);
    }

    /// <summary>
    /// Remove Player Item
    /// </summary>
    /// <param name="weapon"></param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.items/remove-player-item</c> is disabled</exception>
    public void RemovePlayerItem(CBasePlayerWeapon weapon)
    {
        Guard.IsValidEntity(this);
        Guard.IsValidEntity(weapon);

        VirtualFunctions.RemovePlayerItemVirtual(Handle, weapon.Handle);
    }
}