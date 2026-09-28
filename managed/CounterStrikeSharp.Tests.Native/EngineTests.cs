using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

public class EngineTests
{
    [Fact]
    public void GetMapName_ReturnsValidMapName()
    {
        var mapName = Server.MapName;

        Assert.NotNull(mapName);
        Assert.NotEmpty(mapName);
    }

    [Fact]
    public void GetGameDirectory_ReturnsValidPath()
    {
        var gameDir = Server.GameDirectory;

        Assert.NotNull(gameDir);
        Assert.NotEmpty(gameDir);
    }

    [Fact]
    public void IsMapValid_ReturnsTrueForCurrentMap()
    {
        var mapName = Server.MapName;
        var isValid = Server.IsMapValid(mapName);

        Assert.True(isValid, $"Current map '{mapName}' should be valid");
    }

    [Fact]
    public void IsMapValid_ReturnsFalseForInvalidMap()
    {
        var isValid = Server.IsMapValid("nonexistent_map_xyz_12345");

        Assert.False(isValid, "Nonexistent map should be invalid");
    }

    [Fact]
    public void GetTickCount_ReturnsPositiveValue()
    {
        var tickCount = Server.TickCount;

        Assert.True(tickCount > 0, $"TickCount should be positive, got {tickCount}");
    }

    [Fact]
    public async Task GetTickCount_IncrementsOverTime()
    {
        var tickCount1 = Server.TickCount;
        await WaitOneFrame();
        var tickCount2 = Server.TickCount;

        Assert.True(tickCount2 > tickCount1, $"TickCount should increment: {tickCount1} -> {tickCount2}");
    }

    [Fact]
    public void GetEngineTime_ReturnsPositiveValue()
    {
        var engineTime = Server.EngineTime;

        Assert.True(engineTime > 0, $"EngineTime should be positive, got {engineTime}");
    }

    [Fact]
    public async Task GetEngineTime_IncrementsOverTime()
    {
        var time1 = Server.EngineTime;
        await WaitOneFrame();
        var time2 = Server.EngineTime;

        Assert.True(time2 > time1, $"EngineTime should increment: {time1} -> {time2}");
    }

    [Fact]
    public void GetCurrentTime_ReturnsPositiveValue()
    {
        var currentTime = Server.CurrentTime;

        Assert.True(currentTime > 0, $"CurrentTime should be positive, got {currentTime}");
    }

    [Fact]
    public async Task GetCurrentTime_IncrementsOverTime()
    {
        var time1 = Server.CurrentTime;
        await WaitOneFrame();
        var time2 = Server.CurrentTime;

        Assert.True(time2 > time1, $"CurrentTime should increment: {time1} -> {time2}");
    }

    [Fact]
    public void GetMaxClients_ReturnsExpectedValue()
    {
        var maxPlayers = Server.MaxPlayers;

        Assert.True(maxPlayers > 0, $"MaxPlayers should be positive, got {maxPlayers}");
        Assert.True(maxPlayers <= 64, $"MaxPlayers should be <= 64 for CS2, got {maxPlayers}");
    }

    [Fact]
    public void GetTickInterval_ReturnsCorrectValue()
    {
        var tickInterval = NativeAPI.GetTickInterval();

        // CS2 is a 64 tick server, so tick interval should be 1/64 = 0.015625
        Assert.True(tickInterval > 0, $"TickInterval should be positive, got {tickInterval}");
        Assert.Equal(0.015625f, tickInterval, 6);
    }

    [Fact]
    public void GetGameFrameTime_ReturnsPositiveValue()
    {
        var frameTime = Server.FrameTime;

        Assert.True(frameTime > 0, $"FrameTime should be positive, got {frameTime}");
    }

    [Fact]
    public void GetTickedTime_ReturnsNonNegativeValue()
    {
        var tickedTime = Server.TickedTime;

        Assert.True(tickedTime >= 0, $"TickedTime should be non-negative, got {tickedTime}");
    }
}
