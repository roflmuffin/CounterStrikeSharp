using System;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerPawn
{
	/// <summary>
	/// Sets the head scale of this pawn and notifies clients of the change.
	/// </summary>
	/// <remarks>
	/// Requires <see cref="CCSGameRules.SetBigHeadEnabled"/> to be enabled.
	/// Applies only to this pawn; the scale is not preserved across respawns.
	/// </remarks>
	/// <param name="scale">A finite, positive multiplier. A value of 1 is the normal head size.</param>
	/// <exception cref="InvalidOperationException">The pawn is not valid.</exception>
	/// <exception cref="ArgumentOutOfRangeException">The scale is not finite or is not positive.</exception>
	public void SetBigHeadScale(float scale)
	{
		Guard.IsValidEntity(this);
		if (!float.IsFinite(scale) || scale <= 0)
			throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be a finite positive number.");

		if (BigHeadScale == scale) return;
		BigHeadScale = scale;
		Utilities.SetStateChanged(this, "CCSPlayerPawn", "m_flBigHeadScale");
	}

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
