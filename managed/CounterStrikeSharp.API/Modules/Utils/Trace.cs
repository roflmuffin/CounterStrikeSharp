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

using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils;

/// <summary>Controls which physics layers a trace interacts with.</summary>
public sealed class TraceOptions
{
    /// <summary>Which interaction layers the trace represents (analogous to Source 1 contents).</summary>
    public ulong InteractsAs { get; set; } = 0UL; // CONTENTS_EMPTY

    /// <summary>Which interaction layers the trace collides with (analogous to Source 1 trace mask).</summary>
    public ulong InteractsWith { get; set; } = ~0UL; // MASK_ALL

    /// <summary>Which interaction layers are explicitly excluded.</summary>
    public ulong InteractsExclude { get; set; } = 0UL; // CONTENTS_EMPTY
}

/// <summary>
/// Result of a ray or hull trace.
/// Plain unmanaged value type — stack-allocated by the <see cref="Trace"/> methods and returned
/// by value. No heap allocation, no disposal. Layout matches <c>CSSTraceResult</c> in
/// <c>natives_raytrace.cpp</c>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TraceResult
{
    private nint m_pSurfaceProperties; // 0   CPhysSurfaceProperties*
    private nint m_pEnt; // 8   CEntityInstance*
    private nint m_pHitbox; // 16  CHitBox*
    private nint m_hBody; // 24  HPhysicsBody
    private nint m_hShape; // 32  HPhysicsShape
    private ulong m_nContents; // 40
    private float m_vStartX, m_vStartY, m_vStartZ; // 48
    private float m_vEndX, m_vEndY, m_vEndZ; // 60
    private float m_vHitNormalX, m_vHitNormalY, m_vHitNormalZ; // 72
    private float m_vHitPointX, m_vHitPointY, m_vHitPointZ; // 84
    private float m_flHitOffset; // 96
    private float m_flFraction; // 100
    private int m_nTriangle; // 104
    private short m_nHitboxBoneIndex; // 108
    private byte m_eRayType; // 110  (0=Line 1=Sphere 2=Hull 3=Capsule)
    private byte m_bStartInSolid; // 111
    private byte m_bExactHitPoint; // 112

    public bool DidHit() => m_flFraction < 1.0f || m_bStartInSolid != 0;
    public bool IsAllSolid => m_bStartInSolid != 0;
    public bool HasExactHitPoint => m_bExactHitPoint != 0;

    public float Fraction => m_flFraction;
    public float HitOffset => m_flHitOffset;
    public ulong Contents => m_nContents;
    public int TriangleIndex => m_nTriangle;
    public short HitboxBoneIndex => m_nHitboxBoneIndex;
    public RayType_t RayType => (RayType_t)m_eRayType;

    public Vector StartPos => new(m_vStartX, m_vStartY, m_vStartZ);
    public Vector EndPos => new(m_vEndX, m_vEndY, m_vEndZ);
    public Vector Normal => new(m_vHitNormalX, m_vHitNormalY, m_vHitNormalZ);
    public Vector HitPoint => new(m_vHitPointX, m_vHitPointY, m_vHitPointZ);

    public CEntityInstance HitEntity() => new(m_pEnt);

    /// <summary>Native <c>CHitBox*</c> — opaque pointer.</summary>
    public nint Hitbox() => m_pHitbox;

    /// <summary>Native <c>HPhysicsBody</c> — opaque pointer.</summary>
    public nint Body() => m_hBody;

    /// <summary>Native <c>HPhysicsShape</c> — opaque pointer.</summary>
    public nint Shape() => m_hShape;

    /// <summary>Native <c>CPhysSurfaceProperties*</c> — opaque pointer.</summary>
    public nint Surface() => m_pSurfaceProperties;
}

/// <summary>
/// Static class for performing ray and hull traces against the game world.
/// </summary>
public static unsafe class Trace
{
    private static (ulong ia, ulong iw, ulong ie) Unpack(TraceOptions? opts)
        => opts is null ? (0UL, ~0UL, 0UL) : (opts.InteractsAs, opts.InteractsWith, opts.InteractsExclude);

    /// <summary>
    /// Performs a line trace from <paramref name="startPos"/> in the direction of
    /// <paramref name="angles"/> for up to 8192 units.
    /// </summary>
    public static TraceResult TraceShape(Vector startPos, QAngle angles, CBaseEntity? ignoreEntity = null, TraceOptions? options = null)
    {
        var (ia, iw, ie) = Unpack(options);
        TraceResult result = default;
        NativeAPI.TraceShape(startPos.Handle, angles.Handle, ignoreEntity?.Handle ?? nint.Zero, ia, iw, ie, (nint)(&result));
        return result;
    }

    /// <summary>Performs a line trace between two world-space points.</summary>
    public static TraceResult TraceEndShape(Vector startPos, Vector endPos, CBaseEntity? ignoreEntity = null, TraceOptions? options = null)
    {
        var (ia, iw, ie) = Unpack(options);
        TraceResult result = default;
        NativeAPI.TraceEndShape(startPos.Handle, endPos.Handle, ignoreEntity?.Handle ?? nint.Zero, ia, iw, ie, (nint)(&result));
        return result;
    }

    /// <summary>Performs a hull (box) trace between two world-space points.</summary>
    public static TraceResult TraceHullShape(Vector startPos, Vector endPos, Vector mins, Vector maxs, CBaseEntity? ignoreEntity = null, TraceOptions? options = null)
    {
        var (ia, iw, ie) = Unpack(options);
        TraceResult result = default;
        NativeAPI.TraceHullShape(startPos.Handle, endPos.Handle, mins.Handle, maxs.Handle, ignoreEntity?.Handle ?? nint.Zero, ia, iw, ie, (nint)(&result));
        return result;
    }

    /// <summary>Returns the contents bitmask at the given world position.</summary>
    public static ulong PointContents(Vector pos, ulong contentsMask = ~0UL)
        => NativeAPI.PointContents(pos.Handle, contentsMask);

    /// <summary>
    /// Returns whether a nav area overlaps with an entity's bounding box.
    /// <paramref name="area"/> is an opaque pointer to a <c>CNavArea</c> obtained from the nav system.
    /// </summary>
    public static bool CheckAreaOverlappingEntity(nint area, CBaseEntity entity, bool extrudeHullHeight = false)
        => NativeAPI.CheckAreaOverlappingEntity(area, entity.Handle, extrudeHullHeight);

    /// <summary>Returns the world-space axis-aligned bounding box of <paramref name="entity"/>.</summary>
    public static (Vector Mins, Vector Maxs) GetEntityWorldSpaceAABB(CBaseEntity entity)
    {
        var mins = new Vector();
        var maxs = new Vector();
        NativeAPI.GetEntityWorldSpaceAabb(entity.Handle, mins.Handle, maxs.Handle);
        return (mins, maxs);
    }
}
