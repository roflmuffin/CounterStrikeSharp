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
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>.
 */

namespace CounterStrikeSharp.API.Modules.Utils;

/// <summary>
/// Contents flags for physics interaction layers (bspflags.h / const.h).
/// Used with <see cref="TraceOptions"/> and <see cref="Trace.PointContents"/>.
/// </summary>
public static class Contents
{
    public const ulong Empty = 0ul;

    public const ulong Solid = 1ul << 0;
    public const ulong Hitbox = 1ul << 1;
    public const ulong Trigger = 1ul << 2;
    public const ulong Sky = 1ul << 3;

    public const ulong PlayerClip = 1ul << 4;
    public const ulong NpcClip = 1ul << 5;
    public const ulong BlockLos = 1ul << 6;
    public const ulong BlockLight = 1ul << 7;
    public const ulong Ladder = 1ul << 8;
    public const ulong Pickup = 1ul << 9;
    public const ulong BlockSound = 1ul << 10;
    public const ulong NoDraw = 1ul << 11;
    public const ulong Window = 1ul << 12;
    public const ulong PassBullets = 1ul << 13;
    public const ulong WorldGeometry = 1ul << 14;
    public const ulong Water = 1ul << 15;
    public const ulong Slime = 1ul << 16;
    public const ulong TouchAll = 1ul << 17;
    public const ulong Player = 1ul << 18;
    public const ulong Npc = 1ul << 19;
    public const ulong Debris = 1ul << 20;
    public const ulong PhysicsProp = 1ul << 21;
    public const ulong NavIgnore = 1ul << 22;
    public const ulong NavLocalIgnore = 1ul << 23;
    public const ulong PostProcessingVolume = 1ul << 24;
    public const ulong UnusedLayer3 = 1ul << 25;
    public const ulong CarriedObject = 1ul << 26;
    public const ulong Pushaway = 1ul << 27;
    public const ulong ServerEntityOnClient = 1ul << 28;
    public const ulong CarriedWeapon = 1ul << 29;
    public const ulong StaticLevel = 1ul << 30;

    public const ulong CsgoTeam1 = 1ul << 31;
    public const ulong CsgoTeam2 = 1ul << 32;
    public const ulong CsgoGrenadeClip = 1ul << 33;
    public const ulong CsgoDroneClip = 1ul << 34;
    public const ulong CsgoMoveable = 1ul << 35;
    public const ulong CsgoOpaque = 1ul << 36;
    public const ulong CsgoMonster = 1ul << 37;
    public const ulong CsgoUnusedLayer = 1ul << 38;
    public const ulong CsgoThrownGrenade = 1ul << 39;
}

/// <summary>
/// Composite trace masks (bspflags.h MASK_* defines).
/// </summary>
public static class Mask
{
    public const ulong All = ~0ul;

    /// <summary>Everything that is normally solid.</summary>
    public const ulong Solid = Contents.Solid | Contents.Window | Contents.Player | Contents.Npc | Contents.PassBullets;

    /// <summary>Everything that blocks player movement.</summary>
    public const ulong PlayerSolid = Contents.Solid | Contents.PlayerClip | Contents.Window | Contents.Player | Contents.Npc | Contents.PassBullets;

    /// <summary>Blocks NPC movement.</summary>
    public const ulong NpcSolid = Contents.Solid | Contents.NpcClip | Contents.Window | Contents.Player | Contents.Npc | Contents.PassBullets;

    /// <summary>Blocks fluid movement.</summary>
    public const ulong NpcFluid = Contents.Solid | Contents.NpcClip | Contents.Window | Contents.Player | Contents.Npc;

    /// <summary>Water physics.</summary>
    public const ulong Water = Contents.Water | Contents.Slime;

    /// <summary>Bullets see these as solid.</summary>
    public const ulong Shot = Contents.Solid | Contents.Player | Contents.Npc | Contents.Window | Contents.Debris | Contents.Hitbox;

    /// <summary>Bullets see these as solid, except monsters (world + brush only).</summary>
    public const ulong ShotBrushOnly = Contents.Solid | Contents.Window | Contents.Debris;

    /// <summary>Non-raycasted weapons see this as solid (includes grates).</summary>
    public const ulong ShotHull = Contents.Solid | Contents.Player | Contents.Npc | Contents.Window | Contents.Debris | Contents.PassBullets;

    /// <summary>Hits solids (not grates) and passes through everything else.</summary>
    public const ulong ShotPortal = Contents.Solid | Contents.Window | Contents.Player | Contents.Npc;

    /// <summary>Everything normally solid, except monsters (world + brush only).</summary>
    public const ulong SolidBrushOnly = Contents.Solid | Contents.Window | Contents.PassBullets;

    /// <summary>Everything normally solid for player movement, except monsters (world + brush only).</summary>
    public const ulong PlayerSolidBrushOnly = Contents.Solid | Contents.Window | Contents.PlayerClip | Contents.PassBullets;

    /// <summary>Everything normally solid for NPC movement, except monsters (world + brush only).</summary>
    public const ulong NpcSolidBrushOnly = Contents.Solid | Contents.Window | Contents.NpcClip | Contents.PassBullets;
}

public enum RayType_t : uint
{
	RAY_TYPE_LINE = 0x0,
	RAY_TYPE_SPHERE = 0x1,
	RAY_TYPE_HULL = 0x2,
	RAY_TYPE_CAPSULE = 0x3,
	RAY_TYPE_MESH = 0x4
};
