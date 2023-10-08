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

using System;

namespace CounterStrikeSharp.API.Modules.Entities.Constants
{
    [Flags]
    public enum EntityFlags
    {
        Onground = (1 << 0),    // At rest / on the ground
        Ducking = (1 << 1), // Player flag -- Player is fully crouched
        Waterjump = (1 << 3),   // player jumping out of water
        Ontrain = (1 << 4), // Player is _controlling_ a train, so movement commands should be ignored on client during prediction.
        Inrain = (1 << 5),  // Indicates the entity is standing in rain
        Frozen = (1 << 6), // Player is frozen for 3rd person camera
        Atcontrols = (1 << 7), // Player can't move, but keeps key inputs for controlling another entity
        Client = (1 << 8),  // Is a player
        Fakeclient = (1 << 9),  // Fake client, simulated server side; don't send network messages to them
        Inwater = (1 << 10),    // In water
        Fly = (1 << 11),    // Changes the SV_Movestep() behavior to not need to be on ground
        Swim = (1 << 12),   // Changes the SV_Movestep() behavior to not need to be on ground (but stay in water)
        Conveyor = (1 << 13),
        Npc = (1 << 14),
        Godmode = (1 << 15),
        Notarget = (1 << 16),
        Aimtarget = (1 << 17),  // set if the crosshair needs to aim onto the entity
        Partialground = (1 << 18),  // not all corners are valid
        Staticprop = (1 << 19), // Eetsa static prop!		
        Graphed = (1 << 20), // worldgraph has this ent listed as something that blocks a connection
        Grenade = (1 << 21),
        Stepmovement = (1 << 22),   // Changes the SV_Movestep() behavior to not do any processing
        Donttouch = (1 << 23),  // Doesn't generate touch functions, generates Untouch() for anything it was touching when this flag was set
        Basevelocity = (1 << 24),   // Base velocity has been applied this frame (used to convert base velocity into momentum)
        Worldbrush = (1 << 25), // Not moveable/removeable brush entity (really part of the world, but represented as an entity for transparency or something)
        Object = (1 << 26), // Terrible name. This is an object that NPCs should see. Missiles, for example.
        Killme = (1 << 27), // This entity is marked for death -- will be freed by game DLL
        Onfire = (1 << 28), // You know...
        Dissolving = (1 << 29), // We're dissolving!
        Transragdoll = (1 << 30), // In the process of turning into a client side ragdoll.
        UnblockableByPlayer = (1 << 31) // pusher that can't be blocked by the player
    }
}