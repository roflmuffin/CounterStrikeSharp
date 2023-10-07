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
    public enum CollisionGroup
    {
		COLLISION_GROUP_NONE  = 0,
        COLLISION_GROUP_DEBRIS,			// Collides with nothing but world and static stuff
        COLLISION_GROUP_DEBRIS_TRIGGER, // Same as debris, but hits triggers
        COLLISION_GROUP_INTERACTIVE_DEBRIS,	// Collides with everything except other interactive debris or debris
        COLLISION_GROUP_INTERACTIVE,	// Collides with everything except interactive debris or debris
        COLLISION_GROUP_PLAYER,
        COLLISION_GROUP_BREAKABLE_GLASS,
        COLLISION_GROUP_VEHICLE,
        COLLISION_GROUP_PLAYER_MOVEMENT,  // For HL2, same as Collision_Group_Player, for
        // TF2, this filters out other players and CBaseObjects
        COLLISION_GROUP_NPC,			// Generic NPC group
        COLLISION_GROUP_IN_VEHICLE,		// for any entity inside a vehicle
        COLLISION_GROUP_WEAPON,			// for any weapons that need collision detection
        COLLISION_GROUP_VEHICLE_CLIP,	// vehicle clip brush to restrict vehicle movement
        COLLISION_GROUP_PROJECTILE,		// Projectiles!
        COLLISION_GROUP_DOOR_BLOCKER,	// Blocks entities not permitted to get near moving doors
        COLLISION_GROUP_PASSABLE_DOOR,	// Doors that the player shouldn't collide with
        COLLISION_GROUP_DISSOLVING,		// Things that are dissolving are in this group
        COLLISION_GROUP_PUSHAWAY,		// Nonsolid on client and server, pushaway in player code

        COLLISION_GROUP_NPC_ACTOR,		// Used so NPCs in scripts ignore the player.
        COLLISION_GROUP_NPC_SCRIPTED,	// USed for NPCs in scripts that should not collide with each other
        COLLISION_GROUP_PZ_CLIP,



        COLLISION_GROUP_DEBRIS_BLOCK_PROJECTILE, // Only collides with bullets

        LAST_SHARED_COLLISION_GROUP
	}
}