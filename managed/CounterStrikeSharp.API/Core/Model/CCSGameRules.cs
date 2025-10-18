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

using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CCSGameRules
{
    /// <summary>
    /// Terminate the round with the given delay and reason.
    /// </summary>
    public void TerminateRound(float delay, RoundEndReason roundEndReason)
    {
        VirtualFunctions.TerminateRound(Handle, roundEndReason, delay, 0, 0);
    }

    internal CBaseEntity? FindPickerEntityInternal(CBasePlayerController player)
    {
        // TODO: TEST!
        // the third parameter seems to be something like `CDefaultTypedEntityInstanceFilter<CBaseEntity>` but its optional (earlier it was a string?)

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            VirtualFunctionWithReturn<CCSGameRules, CBasePlayerController, IntPtr?, CBaseEntity?> CCSGameRules_FindPickerEntity = new(Handle, GameData.GetOffset("CCSGameRules_FindPickerEntity"));
            return CCSGameRules_FindPickerEntity.Invoke(this, player, null);
        }
        else
        {
            VirtualFunctionWithReturn<CCSGameRules, CBasePlayerController, IntPtr?, double, double, CBaseEntity?> CCSGameRules_FindPickerEntity = new(Handle, GameData.GetOffset("CCSGameRules_FindPickerEntity"));
            return CCSGameRules_FindPickerEntity.Invoke(this, player, null, 0, 0); // on linux we have a fourth and fifth parameter aswell, but they are unused because the condition for them is always unmet.
        }
    }

    public T? FindPickerEntity<T>(CBasePlayerController player)
        where T : CBaseEntity
    {
        CBaseEntity? entity = FindPickerEntityInternal(player);

        if (entity == null || !entity.IsValid)
        {
            return null;
        }

        return entity.As<T>();
    }

    public CCSPlayerController? GetClientAimTarget(CCSPlayerController player)
    {
        CCSPlayerPawn? pawn = FindPickerEntity<CCSPlayerPawn>(player);

        if (pawn == null || !pawn.IsValid)
            return null;

        if (pawn.DesignerName == "player")
        {
            return pawn.OriginalController.Value;
        }

        return null;
    }
}
