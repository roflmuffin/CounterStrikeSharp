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
    public void SlotBindingFieldsDoNotChangeHowAnOffsetIsReadByKey()
    {
        // The additive Slot Binding shape (src/core/binding_resolver.h): class, library and fingerprint window under
        // "vtable"; the slot index stays the per-OS offset.
        LoadGameData($$"""
                       { "ISource2Server::GameFrame": {
                           "offsets": { "{{CurrentOs}}": 20 },
                           "vtable": { "class": "CSource2Server", "library": "server",
                                       "{{CurrentOs}}": { "window": [null, "8573f973ef825f3f", "dc3e8eb9a1a8f4c4"],
                                                          "uniqueNearby": true, "size": 105 } } },
                         "Foo": { "offsets": { "{{CurrentOs}}": 42 } } }
                       """);

        Assert.Equal(20, GameData.GetOffset("ISource2Server::GameFrame"));
        Assert.Equal(42, GameData.GetOffset("Foo"));
    }

    [Fact]
    public void TheShippedGamedataLoads()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "configs"))) directory = directory.Parent;
        Assert.NotNull(directory);
        LoadGameData(File.ReadAllText(Path.Combine(directory!.FullName, "configs", "addons", "counterstrikesharp", "gamedata",
            "gamedata.json")));

        Assert.Contains("ISource2Server::GameFrame", GameData.GameDataProvider.Methods.Keys);
        Assert.Equal(387, GameData.GetOffset("CBasePlayerPawn_CommitSuicide"));
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
