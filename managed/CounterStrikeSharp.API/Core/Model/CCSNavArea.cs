using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public class CCSNavArea : NativeObject
{
    public CCSNavArea(IntPtr pointer) : base(pointer)
    {
    }
    private static readonly Lazy<bool> IsWindows = new(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

    /// <summary>
    /// Gets the nav area identifier.
    /// </summary>
    public uint Id => Data.Id;

    /// <summary>
    /// Gets the center position of the nav area.
    /// </summary>
    public Vector Center => new(Data.CenterX, Data.CenterY, Data.CenterZ);

    /// <summary>
    /// Gets the surface normal of the nav area.
    /// </summary>
    public Vector Normal => new(Data.NormalX, Data.NormalY, Data.NormalZ);

    /// <summary>
    /// Gets the minimum bounds of the nav area.
    /// </summary>
    public Vector Min => new(Math.Min(Data.MinX, Data.MaxX), Math.Min(Data.MinY, Data.MaxY), Math.Min(Data.MinZ, Data.MaxZ));

    /// <summary>
    /// Gets the maximum bounds of the nav area.
    /// </summary>
    public Vector Max => new(Math.Max(Data.MinX, Data.MaxX), Math.Max(Data.MinY, Data.MaxY), Math.Max(Data.MinZ, Data.MaxZ));

    /// <summary>
    /// Gets the width of the nav area on the X axis.
    /// </summary>
    public float Width
    {
        get
        {
            var min = Min;
            var max = Max;
            return Math.Abs(max.X - min.X);
        }
    }

    /// <summary>
    /// Gets the height of the nav area on the Y axis.
    /// </summary>
    public float Height
    {
        get
        {
            var min = Min;
            var max = Max;
            return Math.Abs(max.Y - min.Y);
        }
    }

    /// <summary>
    /// Gets the two-dimensional area size.
    /// </summary>
    public float Area2D => Width * Height;

    /// <summary>
    /// Returns whether the specified position is inside this nav area.
    /// </summary>
    /// <param name="position">World position to test.</param>
    /// <param name="zTolerance">Allowed Z distance from the nav area surface.</param>
    /// <returns><see langword="true"/> if the position is inside this nav area; otherwise, <see langword="false"/>.</returns>
    public bool ContainsPoint(Vector position, float zTolerance = 32f)
    {
        var min = Min;
        var max = Max;

        return position.X >= min.X && position.X <= max.X && position.Y >= min.Y && position.Y <= max.Y && Math.Abs(position.Z - GetHeightAtPosition(position.X, position.Y)) <= zTolerance;
    }

    /// <summary>
    /// Returns whether the specified box is fully contained inside this nav area.
    /// </summary>
    /// <param name="mins">Minimum bounds of the box.</param>
    /// <param name="maxs">Maximum bounds of the box.</param>
    /// <param name="zTolerance">Allowed Z distance from the nav area surface.</param>
    /// <returns><see langword="true"/> if the box is contained inside this nav area; otherwise, <see langword="false"/>.</returns>
    public bool ContainsBox(Vector mins, Vector maxs, float zTolerance = 32f)
    {
        var min = Min;
        var max = Max;
        var boxMinX = Math.Min(mins.X, maxs.X);
        var boxMaxX = Math.Max(mins.X, maxs.X);
        var boxMinY = Math.Min(mins.Y, maxs.Y);
        var boxMaxY = Math.Max(mins.Y, maxs.Y);
        var boxBottomZ = Math.Min(mins.Z, maxs.Z);

        return boxMinX >= min.X && boxMaxX <= max.X && boxMinY >= min.Y && boxMaxY <= max.Y
                && Math.Abs(boxBottomZ - GetHeightAtPosition(boxMinX, boxMinY)) <= zTolerance
                && Math.Abs(boxBottomZ - GetHeightAtPosition(boxMinX, boxMaxY)) <= zTolerance
                && Math.Abs(boxBottomZ - GetHeightAtPosition(boxMaxX, boxMinY)) <= zTolerance
                && Math.Abs(boxBottomZ - GetHeightAtPosition(boxMaxX, boxMaxY)) <= zTolerance;
    }

    /// <summary>
    /// Returns whether the specified box intersects this nav area.
    /// </summary>
    /// <param name="mins">Minimum bounds of the box.</param>
    /// <param name="maxs">Maximum bounds of the box.</param>
    /// <param name="zTolerance">Allowed Z distance from the nav area surface.</param>
    /// <returns><see langword="true"/> if the box intersects this nav area; otherwise, <see langword="false"/>.</returns>
    public bool IntersectsBox(Vector mins, Vector maxs, float zTolerance = 32f)
    {
        var min = Min;
        var max = Max;
        var boxMinX = Math.Min(mins.X, maxs.X);
        var boxMaxX = Math.Max(mins.X, maxs.X);
        var boxMinY = Math.Min(mins.Y, maxs.Y);
        var boxMaxY = Math.Max(mins.Y, maxs.Y);

        if (boxMaxX < min.X || boxMinX > max.X || boxMaxY < min.Y || boxMinY > max.Y)
        {
            return false;
        }

        var x = Math.Clamp((boxMinX + boxMaxX) * 0.5f, min.X, max.X);
        var y = Math.Clamp((boxMinY + boxMaxY) * 0.5f, min.Y, max.Y);
        var z = GetHeightAtPosition(x, y);

        return z >= Math.Min(mins.Z, maxs.Z) - zTolerance && z <= Math.Max(mins.Z, maxs.Z) + zTolerance;
    }

    /// <summary>
    /// Gets the closest point on this nav area to the specified position.
    /// </summary>
    /// <param name="position">World position to compare against.</param>
    /// <returns>The closest point on this nav area.</returns>
    public Vector GetClosestPoint(Vector position)
    {
        var min = Min;
        var max = Max;
        var x = Math.Clamp(position.X, min.X, max.X);
        var y = Math.Clamp(position.Y, min.Y, max.Y);

        return new Vector(x, y, GetHeightAtPosition(x, y));
    }

    /// <summary>
    /// Gets the distance from this nav area to the specified position.
    /// </summary>
    /// <param name="position">World position to compare against.</param>
    /// <returns>The distance to this nav area.</returns>
    public float GetDistanceToPoint(Vector position)
    {
        return MathF.Sqrt(GetDistanceSquaredToPoint(position));
    }

    /// <summary>
    /// Gets the closest nav area to the specified position.
    /// </summary>
    /// <param name="position">World position to compare against.</param>
    /// <param name="maximumDistance">Maximum search distance. A value less than or equal to zero disables the limit.</param>
    /// <returns>The closest nav area, or <see langword="null"/> if none was found.</returns>
    public static CCSNavArea? GetClosestNavArea(Vector position, float maximumDistance = -1)
    {
        return GetClosestNavArea(position, out _, maximumDistance);
    }

    /// <summary>
    /// Gets the closest nav area to the specified position and outputs its distance.
    /// </summary>
    /// <param name="position">World position to compare against.</param>
    /// <param name="distance">Distance to the closest nav area, or <see cref="float.MaxValue"/> if none was found.</param>
    /// <param name="maximumDistance">Maximum search distance. A value less than or equal to zero disables the limit.</param>
    /// <returns>The closest nav area, or <see langword="null"/> if none was found.</returns>
    public static CCSNavArea? GetClosestNavArea(Vector position, out float distance, float maximumDistance = -1)
    {
        var navAreas = GetAllNavAreas();
        var maximumDistanceSquared = maximumDistance > 0 ? maximumDistance * maximumDistance : -1;
        var closestDistanceSquared = float.MaxValue;
        CCSNavArea? closestNavArea = null;
        distance = float.MaxValue;

        foreach (var navArea in navAreas)
        {
            var distanceSquared = navArea.GetDistanceSquaredToPoint(position);
            if (maximumDistanceSquared > 0 && distanceSquared > maximumDistanceSquared)
            {
                continue;
            }

            if (distanceSquared < closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closestNavArea = navArea;
            }
        }

        if (closestNavArea != null)
        {
            distance = MathF.Sqrt(closestDistanceSquared);
        }

        return closestNavArea;
    }

    /// <summary>
    /// Gets all nav areas from the current nav mesh.
    /// </summary>
    /// <returns>All nav areas, or an empty list if the nav mesh is unavailable.</returns>
    public static IReadOnlyList<CCSNavArea> GetAllNavAreas()
    {
        var navMeshAddress = GetNavMeshAddress();
        if (navMeshAddress == IntPtr.Zero)
        {
            return Array.Empty<CCSNavArea>();
        }

        var navMeshData = Marshal.PtrToStructure<CCSNavMeshData>(navMeshAddress);
        if (navMeshData.Count <= 0 || navMeshData.Areas == IntPtr.Zero)
        {
            return Array.Empty<CCSNavArea>();
        }

        var navAreas = new List<CCSNavArea>(navMeshData.Count);
        for (var index = 0; index < navMeshData.Count; index++)
        {
            var navAreaAddress = Marshal.ReadIntPtr(navMeshData.Areas, index * IntPtr.Size);
            if (navAreaAddress != IntPtr.Zero)
            {
                navAreas.Add(new CCSNavArea(navAreaAddress));
            }
        }

        return navAreas;
    }

    private float GetHeightAtPosition(float x, float y)
    {
        var normal = Normal;
        if (Math.Abs(normal.Z) <= 0.0001f)
        {
            return Center.Z;
        }

        var center = Center;
        return center.Z - ((normal.X * (x - center.X)) + (normal.Y * (y - center.Y))) / normal.Z;
    }

    private float GetDistanceSquaredToPoint(Vector position)
    {
        return (position - GetClosestPoint(position)).LengthSqr();
    }

    private static IntPtr GetNavMeshAddress()
    {
        var signature = GameData.GetSignature("CCSNavArea_IsValidNavMesh");
        if (string.IsNullOrWhiteSpace(signature))
        {
            return IntPtr.Zero;
        }

        var functionAddress = NativeAPI.FindSignature(Addresses.ServerPath, signature);
        if (functionAddress == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        var relativeOffset = Marshal.ReadInt32(functionAddress + 3);
        var navMeshPointerAddress = functionAddress + relativeOffset + (IsWindows.Value ? 8 : 7);
        return navMeshPointerAddress == IntPtr.Zero ? IntPtr.Zero : Marshal.ReadIntPtr(navMeshPointerAddress);
    }

    private unsafe ref CCSNavAreaData Data => ref Unsafe.AsRef<CCSNavAreaData>((void*)Handle);

    [StructLayout(LayoutKind.Explicit)]
    private struct CCSNavAreaData
    {
        [FieldOffset(0x08)]
        public uint Id;

        [FieldOffset(0x0C)]
        public float CenterX;

        [FieldOffset(0x10)]
        public float CenterY;

        [FieldOffset(0x14)]
        public float CenterZ;

        [FieldOffset(0x18)]
        public float NormalX;

        [FieldOffset(0x1C)]
        public float NormalY;

        [FieldOffset(0x20)]
        public float NormalZ;

        [FieldOffset(0x24)]
        public float MinX;

        [FieldOffset(0x28)]
        public float MinY;

        [FieldOffset(0x2C)]
        public float MinZ;

        [FieldOffset(0x30)]
        public float MaxX;

        [FieldOffset(0x34)]
        public float MaxY;

        [FieldOffset(0x38)]
        public float MaxZ;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct CCSNavMeshData
    {
        [FieldOffset(0x08)]
        public int Count;

        [FieldOffset(0x10)]
        public IntPtr Areas;
    }
}
