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

using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CBaseModelEntity
{
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void SetModel(string model)
    {
        Guard.IsValidEntity(this);

        VirtualFunctions.SetModel(Handle, model);
    }

    /// <summary>
    /// Changes the value of a named bodygroup on this entity.
    /// <example>
    /// <code>
    /// pawn.SetBodygroup("default_gloves", 1);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="groupName">The name of the bodygroup to modify (e.g. "default_gloves")</param>
    /// <param name="value">The value to set for the bodygroup</param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void SetBodygroup(string groupName, int value)
    {
        Guard.IsValidEntity(this);

        VirtualFunctions.CBaseModelEntity_SetBodygroup(Handle, groupName, value);
    }
}
