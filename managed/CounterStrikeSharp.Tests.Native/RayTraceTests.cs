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

    [Fact(Skip = "Ignore for now")]
    public async Task Trace_PointContents()
    {
        await InitializeAsync();

        var pos = pawn.AbsOrigin - new Vector(0, 0, 100);
        var contentsMask = Trace.PointContents(pos, Masks.All);
        Assert.NotEqual(0ul, (ulong)contentsMask);
        Assert.True(contentsMask.HasFlag(Contents.Solid) || contentsMask.HasFlag(Contents.PlayerClip));
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
}