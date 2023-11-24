using System;
using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerPawn
{
    /// <summary>
    /// Respawn player
    /// </summary>
    public void Respawn()
    {
        if (!Controller.IsValid) return;
        if (!Controller.Value.IsValid) return;

        VirtualFunctions.CCSPlayerPawn_Respawn(Handle);
        VirtualFunction.CreateVoid<IntPtr>(Controller.Value.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(Controller.Value.Handle);
    }
}