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
    public enum SolidType
    {
		SOLID_NONE = 0, // no solid model
        SOLID_BSP = 1,  // a BSP tree
        SOLID_BBOX = 2, // an AABB
        SOLID_OBB = 3,  // an OBB (not implemented yet)
        SOLID_OBB_YAW = 4,  // an OBB, constrained so that it can only yaw
        SOLID_CUSTOM = 5,   // Always call into the entity for tests
        SOLID_VPHYSICS = 6, // solid vphysics object, get vcollide from the model and collide with that
        SOLID_LAST,
	}
}