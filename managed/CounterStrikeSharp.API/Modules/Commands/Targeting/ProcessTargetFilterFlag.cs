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

namespace CounterStrikeSharp.API.Modules.Commands.Targeting;

/// <summary>
/// Specifies filters for processing command targets.
/// </summary>
[Flags]
public enum ProcessTargetFilterFlag
{
    /// <summary>
    /// No filter applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Only allow alive players as targets.
    /// </summary>
    FilterAlive = 1 << 0,

    /// <summary>
    /// Only allow dead players as targets.
    /// </summary>
    FilterDead = 1 << 1,

    /// <summary>
    /// Filter out targets that the command issuer cannot target due to immunity rules.
    /// </summary>
    FilterNoImmunity = 1 << 2,

    /// <summary>
    /// Do not allow multiple target patterns like @all, @ct, etc.
    /// </summary>
    FilterNoMulti = 1 << 3,

    /// <summary>
    /// Do not allow bots to be targeted.
    /// </summary>
    FilterNoBots = 1 << 4
}
