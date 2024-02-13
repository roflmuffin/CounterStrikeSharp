using System;

using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CBasePlayerController
{
	public void SetPawn(CBasePlayerPawn? pawn)
	{
		if (pawn is null) return;
		if (!pawn.IsValid) return;
		VirtualFunctions.CBasePlayerController_SetPawnFunc.Invoke(this, pawn, true, false);
	}
}
