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

namespace CounterStrikeSharp.API.Modules.Entities.Constants
{
    public enum RoundEndReason : uint
    {
        Unknown = 0x0u,
        TargetBombed = 0x1u,
        TerroristsEscaped = 0x4u,
        CTsPreventEscape = 0x5u,
        EscapingTerroristsNeutralized = 0x6u,
        BombDefused = 0x7u,
        CTsWin = 0x8u,
        TerroristsWin = 0x9u,
        RoundDraw = 0xAu,
        AllHostageRescued = 0xBu,
        TargetSaved = 0xCu,
        HostagesNotRescued = 0xDu,
        TerroristsNotEscaped = 0xEu,
        GameCommencing = 0x10u,

        TerroristsSurrender = 0x11u, // this also triggers match cancelled
        CTsSurrender = 0x12u, // this also triggers match cancelled

        TerroristsPlanted = 0x13u,
        CTsReachedHostage = 0x14u,
        SurvivalWin = 0x15u,
        SurvivalDraw = 0x16u,
        
        [Obsolete("Use RoundEndReason.TerroristsPlanted instead.")]
        TerroristsPlanned = 0x13u
    }
}
