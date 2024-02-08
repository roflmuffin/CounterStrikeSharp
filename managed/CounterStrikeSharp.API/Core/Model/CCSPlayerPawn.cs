using System;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerPawn
{
	/// <summary>
	/// Respawn player
	/// </summary>

	[Obsolete("Use CCSPlayerController.Respawn() instead")]
	public void Respawn()
	{
		if (!Controller.IsValid) return;
		if (!Controller.Value.IsValid) return;

		Application.Instance.Logger.LogWarning("CCSPawn.Respawn is deprecated and does nothing, use CCSPlayerController.Respawn instead");
	}
}
