using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CBasePlayerController
{
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void SetPawn(CBasePlayerPawn? pawn)
    {
        Guard.IsValidEntity(this);

        if (pawn is null) return;
        if (!pawn.IsValid) return;
        VirtualFunctions.CBasePlayerController_SetPawnFunc.Invoke(this, pawn, true, false);
    }
}
