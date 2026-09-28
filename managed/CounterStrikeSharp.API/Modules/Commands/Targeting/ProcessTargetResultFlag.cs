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
/// Represents the result of a target processing operation.
/// </summary>
public enum ProcessTargetResultFlag
{
    /// <summary>
    /// Target(s) were successfully found and filtered.
    /// </summary>
    TargetFound,

    /// <summary>
    /// No target was found matching the initial pattern.
    /// </summary>
    TargetNone,

    /// <summary>
    /// A single target was found, but they were not alive as required by the filter.
    /// Or a multi-target filter resulted in no alive players.
    /// </summary>
    TargetNotAlive,

    /// <summary>
    /// A single target was found, but they were not dead as required by the filter.
    /// Or a multi-target filter resulted in no dead players.
    /// </summary>
    TargetNotDead,

    /// <summary>
    /// The target is immune and cannot be targeted by the command issuer.
    /// </summary>
    TargetImmune,

    /// <summary>
    /// A multi-target filter (like @all) resulted in no players after filtering.
    /// </summary>
    TargetEmptyFilter,

    /// <summary>
    /// The target was found, but it was not a human player as required by the filter.
    /// </summary>
    TargetNotHuman,

    /// <summary>
    /// The target string was ambiguous and matched more than one player when a single target was expected.
    /// </summary>
    TargetAmbiguous
}
