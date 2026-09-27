/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace CounterStrikeSharp.API.Core;

public partial class CCSGameRules
{
    private static readonly Lazy<bool> IsWindows = new(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

    /// <summary>
    /// Terminate the round with the given delay and reason.
    /// </summary>
    public void TerminateRound(float delay, RoundEndReason roundEndReason)
    {
        if (IsWindows.Value)
        {
            VirtualFunctions.TerminateRoundWindows(Handle, delay, roundEndReason, 0, 0);
        }
        else
        {
            VirtualFunctions.TerminateRoundLinux(Handle, roundEndReason, delay, 0, 0);
        }
    }

    internal CBaseEntity? FindPickerEntityInternal(CBasePlayerController player)
    {
        // TODO: TEST!
        // the third parameter seems to be something like `CDefaultTypedEntityInstanceFilter<CBaseEntity>` but its optional (earlier it was a string?)
        if (IsWindows.Value)
        {
            VirtualFunctionWithReturn<CCSGameRules, CBasePlayerController, IntPtr, CBaseEntity> CCSGameRules_FindPickerEntity = new(Handle, GameData.GetOffset("CCSGameRules_FindPickerEntity"));
            return CCSGameRules_FindPickerEntity.Invoke(this, player, IntPtr.Zero);
        }
        else
        {
            VirtualFunctionWithReturn<CCSGameRules, CBasePlayerController, IntPtr, double, double, CBaseEntity> CCSGameRules_FindPickerEntity = new(Handle, GameData.GetOffset("CCSGameRules_FindPickerEntity"));
            return CCSGameRules_FindPickerEntity.Invoke(this, player, IntPtr.Zero, 0, 0); // on linux we have a fourth and fifth parameter aswell, but they are unused because the condition for them is always unmet.
        }
    }

    public T? FindPickerEntity<T>(CBasePlayerController player) where T : CBaseEntity
    {
        CBaseEntity? entity = FindPickerEntityInternal(player);

        return (entity?.IsValid ?? false) ? entity.As<T>() : null;
    }

    public CCSPlayerController? GetClientAimTarget(CCSPlayerController player)
    {
        CCSPlayerPawn? pawn = FindPickerEntity<CCSPlayerPawn>(player);

        return pawn?.DesignerName == "player" ? pawn.OriginalController.Value : null;
    }
}
