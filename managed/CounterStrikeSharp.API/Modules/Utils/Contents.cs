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
[Flags]
public enum Contents : ulong
{
    Empty = 0ul,

    Solid = 1ul << 0,
    Hitbox = 1ul << 1,
    Trigger = 1ul << 2,
    Sky = 1ul << 3,

    PlayerClip = 1ul << 4,
    NpcClip = 1ul << 5,
    BlockLos = 1ul << 6,
    BlockLight = 1ul << 7,
    Ladder = 1ul << 8,
    Pickup = 1ul << 9,
    BlockSound = 1ul << 10,
    NoDraw = 1ul << 11,
    Window = 1ul << 12,
    PassBullets = 1ul << 13,
    WorldGeometry = 1ul << 14,
    Water = 1ul << 15,
    Slime = 1ul << 16,
    TouchAll = 1ul << 17,
    Player = 1ul << 18,
    Npc = 1ul << 19,
    Debris = 1ul << 20,
    PhysicsProp = 1ul << 21,
    NavIgnore = 1ul << 22,
    NavLocalIgnore = 1ul << 23,
    PostProcessingVolume = 1ul << 24,
    UnusedLayer3 = 1ul << 25,
    CarriedObject = 1ul << 26,
    Pushaway = 1ul << 27,
    ServerEntityOnClient = 1ul << 28,
    CarriedWeapon = 1ul << 29,
    StaticLevel = 1ul << 30,

    CsgoTeam1 = 1ul << 31,
    CsgoTeam2 = 1ul << 32,
    CsgoGrenadeClip = 1ul << 33,
    CsgoDroneClip = 1ul << 34,
    CsgoMoveable = 1ul << 35,
    CsgoOpaque = 1ul << 36,
    CsgoMonster = 1ul << 37,
    CsgoUnusedLayer = 1ul << 38,
    CsgoThrownGrenade = 1ul << 39,
}

/// <summary>
/// Composite trace masks (bspflags.h MASK_* defines).
/// </summary>
public static class Masks
{
    public const Contents All = (Contents)~0ul;

    /// <summary>Everything that is normally solid.</summary>
    public const Contents Solid = Contents.Solid | Contents.Window | Contents.Player | Contents.Npc |
                                  Contents.PassBullets;

    /// <summary>Everything that blocks player movement.</summary>
    public const Contents PlayerSolid =
        Contents.Solid | Contents.PlayerClip | Contents.Window | Contents.Player | Contents.Npc |
        Contents.PassBullets;

    /// <summary>Blocks NPC movement.</summary>
    public const Contents NpcSolid =
        Contents.Solid | Contents.NpcClip | Contents.Window | Contents.Player | Contents.Npc |
        Contents.PassBullets;

    /// <summary>Blocks fluid movement.</summary>
    public const Contents NpcFluid =
        Contents.Solid | Contents.NpcClip | Contents.Window | Contents.Player | Contents.Npc;

    /// <summary>Water physics.</summary>
    public const Contents Water = Contents.Water | Contents.Slime;

    /// <summary>Bullets see these as solid.</summary>
    public const Contents Shot = Contents.Solid | Contents.Player | Contents.Npc | Contents.Window |
                                 Contents.Debris | Contents.Hitbox;

    /// <summary>Bullets see these as solid, except monsters (world + brush only).</summary>
    public const Contents ShotBrushOnly = Contents.Solid | Contents.Window | Contents.Debris;

    /// <summary>Non-raycasted weapons see this as solid (includes grates).</summary>
    public const Contents ShotHull =
        Contents.Solid | Contents.Player | Contents.Npc | Contents.Window | Contents.Debris |
        Contents.PassBullets;

    /// <summary>Hits solids (not grates) and passes through everything else.</summary>
    public const Contents ShotPortal = Contents.Solid | Contents.Window | Contents.Player | Contents.Npc;

    /// <summary>Everything normally solid, except monsters (world + brush only).</summary>
    public const Contents SolidBrushOnly = Contents.Solid | Contents.Window | Contents.PassBullets;

    /// <summary>Everything normally solid for player movement, except monsters (world + brush only).</summary>
    public const Contents PlayerSolidBrushOnly =
        Contents.Solid | Contents.Window | Contents.PlayerClip | Contents.PassBullets;

    /// <summary>Everything normally solid for NPC movement, except monsters (world + brush only).</summary>
    public const Contents NpcSolidBrushOnly =
        Contents.Solid | Contents.Window | Contents.NpcClip | Contents.PassBullets;
}

public enum RayType_t : uint
{
    RAY_TYPE_LINE = 0x0,
    RAY_TYPE_SPHERE = 0x1,
    RAY_TYPE_HULL = 0x2,
    RAY_TYPE_CAPSULE = 0x3,
    RAY_TYPE_MESH = 0x4
};
