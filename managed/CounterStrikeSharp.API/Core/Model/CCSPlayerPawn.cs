using System;
using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerPawn
{
	/// <summary>
	/// Respawn player
	/// </summary>

	[Obsolete]
	public void Respawn()
	{
		if (!Controller.IsValid) return;
		if (!Controller.Value.IsValid) return;

		// bit of a bodge ¯\_(ツ)_/¯
		Controller.Value.As<CCSPlayerController>().Respawn();
	}
}
