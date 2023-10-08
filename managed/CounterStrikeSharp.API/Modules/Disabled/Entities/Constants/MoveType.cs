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
    public enum MoveType
    {
        MOVETYPE_NONE = 0,  // never moves
        MOVETYPE_ISOMETRIC,         // For players -- in TF2 commander view, etc.
        MOVETYPE_WALK,              // Player only - moving on the ground
        MOVETYPE_STEP,              // gravity, special edge handling -- monsters use this
        MOVETYPE_FLY,               // No gravity, but still collides with stuff
        MOVETYPE_FLYGRAVITY,        // flies through the air + is affected by gravity
        MOVETYPE_VPHYSICS,          // uses VPHYSICS for simulation
        MOVETYPE_PUSH,              // no clip to world, push and crush
        MOVETYPE_NOCLIP,            // No gravity, no collisions, still do velocity/avelocity
        MOVETYPE_LADDER,            // Used by players only when going onto a ladder
        MOVETYPE_OBSERVER,          // Observer movement, depends on player's observer mode
        MOVETYPE_CUSTOM,            // Allows the entity to describe its own physics

        // should always be defined as the last item in the list
        MOVETYPE_LAST = MOVETYPE_CUSTOM,

        MOVETYPE_MAX_BITS = 4
    }
}