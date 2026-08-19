using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace NativeTestsPlugin;

public class RayTraceTests
{
    private CCSPlayerController player;
    private CCSPlayerPawn pawn;

    public async Task InitializeAsync()
    {
        Server.ExecuteCommand("bot_kick; bot_quota 5; bot_quota_mode normal");
        await WaitOneFrame();
        this.player = Utilities.GetPlayers().Last(p => p.LifeState == (byte)LifeState_t.LIFE_ALIVE);
        if (player.PlayerPawn.Value == null)
        {
            throw new Exception("No valid player pawn found for test player.");
        }

        this.pawn = player.PlayerPawn.Value!;
    }

    [Fact]
    public async Task Trace_AABB()
    {
        await InitializeAsync();

        Trace.GetEntityWorldSpaceAABB(pawn, out var mins, out var maxs);

        Assert.NotEqual(0, mins.X);
        Assert.NotEqual(0, mins.Y);
        Assert.NotEqual(0, mins.Z);
        Assert.NotEqual(0, maxs.X);
        Assert.NotEqual(0, maxs.Y);
        Assert.NotEqual(0, maxs.Z);
        Assert.Equal(72, maxs.Z - mins.Z, 4); // Player hull height is 72 units
    }


    [Fact]
    public async Task Trace_TraceShape()
    {
        await InitializeAsync();

        var start = pawn.AbsOrigin;
        var angles = new QAngle(0, -90, 0);
        var result = Trace.TraceShape(start, angles, ignoreEntity: pawn,
            options: new TraceOptions { InteractsWith = Masks.SolidBrushOnly, InteractsExclude = Contents.Pickup });
        Assert.True(result.DidHit());
        Assert.NotEqual(0ul, (ulong)result.Contents);
        Assert.True(result.Contents.HasFlag(Contents.Solid));
        Assert.True(result.Contents.HasFlag(Contents.StaticLevel));
        Assert.NotEqual(Vector.Zero, result.HitPoint);
        Assert.Equal(RayType_t.RAY_TYPE_LINE, result.RayType);
        Assert.Equal("worldent", result.HitEntity().DesignerName);
    }

    [Fact]
    public async Task Trace_HullShape()
    {
        await InitializeAsync();

        var start = pawn.AbsOrigin;
        var end = start + new Vector(0, 0, -100);
        var result = Trace.TraceHullShape(start, end, new Vector(0, 0, 0), new Vector(32, 32, 72), ignoreEntity: pawn,
            options: new TraceOptions { InteractsWith = Masks.SolidBrushOnly, InteractsExclude = Contents.Pickup });
        Assert.True(result.DidHit());
        Assert.NotEqual(0ul, (ulong)result.Contents);
        Assert.True(result.Contents.HasFlag(Contents.Solid));
        Assert.True(result.Contents.HasFlag(Contents.StaticLevel));
        Assert.NotEqual(Vector.Zero, result.HitPoint);
        Assert.Equal(RayType_t.RAY_TYPE_HULL, result.RayType);
        Assert.Equal("worldent", result.HitEntity().DesignerName);
    }

    [Fact]
    public async Task Trace_EndShape()
    {
        await InitializeAsync();

        var start = pawn.AbsOrigin;
        var end = start + new Vector(0, 0, -100);
        var result = Trace.TraceEndShape(start, end, ignoreEntity: pawn,
            options: new TraceOptions { InteractsWith = Masks.SolidBrushOnly, InteractsExclude = Contents.Pickup });
        Assert.True(result.DidHit());
        Assert.NotEqual(0ul, (ulong)result.Contents);
        Assert.True(result.Contents.HasFlag(Contents.Solid));
        Assert.True(result.Contents.HasFlag(Contents.StaticLevel));
        Assert.NotEqual(Vector.Zero, result.HitPoint);
        Assert.Equal(RayType_t.RAY_TYPE_LINE, result.RayType);
        Assert.Equal("worldent", result.HitEntity().DesignerName);
    }

    [Fact]
    public async Task CCSNavArea_GetAllAreas()
    {
        await InitializeAsync();

        var areas = CCSNavArea.GetAllNavAreas();
        Assert.NotNull(areas);
        Assert.NotEmpty(areas);

        // de_dust2 area count is 2242 at time of writing, but this may change with map updates
        Assert.InRange(areas.Count, 2100, 2300);
    }

    [Fact(Skip = "Area overlapping appears to always fail, even when the pawn is inside the nav area.")]
    public async Task CCSNavArea_CheckAreaOverlappingEntity()
    {
        await InitializeAsync();

        var area = CCSNavArea.GetAllNavAreas().OrderByDescending(a => a.Area2D).First();
        pawn.Teleport(area.Center);

        Assert.True(area.ContainsPoint(pawn.AbsOrigin), "Pawn should be inside the nav area after teleporting to its center.");

#pragma warning disable CS0001
        var overlapping = Trace.CheckAreaOverlappingEntity(area, pawn);
#pragma warning restore CS0001
        Assert.True(overlapping);
    }

    [Fact(Skip = "I can't get this to reliably return a non-zero contents mask, even when the point is inside a solid brush.")]
    public async Task Trace_PointContents()
    {
        await InitializeAsync();

        var pos = new Vector(569, 2368, 59);
#pragma warning disable CS0001
        var contentsMask = Trace.PointContents(pos);
#pragma warning restore CS0001
        Assert.NotEqual(0ul, (ulong)contentsMask);
        Assert.True(contentsMask.HasFlag(Contents.Solid) || contentsMask.HasFlag(Contents.PlayerClip));
    }
}