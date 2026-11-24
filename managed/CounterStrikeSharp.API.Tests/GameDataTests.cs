using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CounterStrikeSharp.API.Tests;

public class GameDataTests : IDisposable
{
    private static readonly string CurrentOs = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "windows";
    private static readonly string OtherOs = CurrentOs == "linux" ? "windows" : "linux";

    private readonly string _gameDataDirectory;

    public GameDataTests()
    {
        _gameDataDirectory = Path.Combine(Path.GetTempPath(), "cssharp-gamedata-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(_gameDataDirectory);
    }

    public void Dispose()
    {
        Directory.Delete(_gameDataDirectory, true);
    }

    private void LoadGameData(string json)
    {
        File.WriteAllText(Path.Combine(_gameDataDirectory, "gamedata.json"), json);

        var configuration = new Mock<IScriptHostConfiguration>();
        configuration.SetupGet(x => x.GameDataPath).Returns(_gameDataDirectory);

        var provider = new GameDataProvider(configuration.Object, NullLogger<GameDataProvider>.Instance);
        provider.Load();
        GameData.GameDataProvider = provider;
    }

    [Fact]
    public void GetOffsetReturnsTheOffsetForTheCurrentOs()
    {
        LoadGameData($$"""
                       { "Foo": { "offsets": { "{{CurrentOs}}": 42, "{{OtherOs}}": 7 } } }
                       """);

        Assert.Equal(42, GameData.GetOffset("Foo"));
    }

    [Fact]
    public void GetOffsetReturnsAnExplicitZeroOffset()
    {
        LoadGameData($$"""
                       { "Foo": { "offsets": { "{{CurrentOs}}": 0 } } }
                       """);

        Assert.Equal(0, GameData.GetOffset("Foo"));
    }

    [Fact]
    public void GetOffsetThrowsWhenTheOffsetIsMissingForTheCurrentOs()
    {
        LoadGameData($$"""
                       { "Foo": { "offsets": { "{{OtherOs}}": 7 } } }
                       """);

        var ex = Assert.Throws<InvalidOperationException>(() => GameData.GetOffset("Foo"));
        Assert.Contains("Foo", ex.Message);
        Assert.Contains("missing for this OS", ex.Message);
    }
}
