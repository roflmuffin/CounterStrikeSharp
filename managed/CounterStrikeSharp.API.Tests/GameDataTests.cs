using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CounterStrikeSharp.API.Tests;

// Slot Binding keys are read through the process-wide Resolution Report, so this shares the collection of the other
// tests that install one.
[Collection(nameof(EngineFeaturesApiTests))]
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
        EngineFeatures.Reset();
    }

    // The Resolution Report native would produce for these Slot Bindings.
    private static void InstallReport(params BindingResult[] bindings) =>
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport("1.42.0.1/2000950", bindings, []), ManagedEngineFeatures.All);

    private static BindingResult SlotBinding(string key, long offset, bool healed = false, long statedIndex = -1) =>
        new(key, BindingKind.Slot, true, BindingFailureReason.None, 1, 0x1000, offset, "server", Healed: healed,
            Slot: new SlotEvidence("CBasePlayerPawn", statedIndex < 0 ? offset : statedIndex, healed ? "Heal" : "Window", 401, 401));

    private static string CommitSuicideEntry(int linux) => $$"""
        "CBasePlayerPawn_CommitSuicide": {
          "offsets": { "{{CurrentOs}}": {{linux}} },
          "vtable": { "class": "CBasePlayerPawn", "library": "server",
                      "{{CurrentOs}}": { "window": ["94fb8c66f9cd9341", "db8bada8adf78b97", "96b8b13a907b19f6"],
                                         "uniqueNearby": true, "size": 401 } } }
        """;

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
        InstallReport(SlotBinding("ISource2Server::GameFrame", 20));

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
        InstallReport(SlotBinding("CBasePlayerPawn_CommitSuicide", 387));
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

    [Fact]
    public void ASlotBindingReadByKeyIsTheHealedIndexNotTheGamedataOne()
    {
        // Gamedata states CommitSuicide one slot too low; the boot Vtable Check healed it at 387.
        LoadGameData($"{{ {CommitSuicideEntry(386)} }}");
        InstallReport(SlotBinding("CBasePlayerPawn_CommitSuicide", 387, healed: true, statedIndex: 386));

        Assert.Equal(387, GameData.GetOffset("CBasePlayerPawn_CommitSuicide"));
    }

    [Fact]
    public void ASlotBindingReadByKeyIsTheVerifiedIndex()
    {
        LoadGameData($"{{ {CommitSuicideEntry(387)} }}");
        InstallReport(SlotBinding("CBasePlayerPawn_CommitSuicide", 387));

        Assert.Equal(387, GameData.GetOffset("CBasePlayerPawn_CommitSuicide"));
    }

    [Fact]
    public void AFailedSlotBindingReadByKeyThrowsInsteadOfReturningAWrongIndex()
    {
        LoadGameData($"{{ {CommitSuicideEntry(387)} }}");
        InstallReport(new BindingResult("CBasePlayerPawn_CommitSuicide", BindingKind.Slot, false,
            BindingFailureReason.FingerprintMismatch, 1, 0x1000, 0, "server"));

        var ex = Assert.Throws<InvalidOperationException>(() => GameData.GetOffset("CBasePlayerPawn_CommitSuicide"));
        Assert.Equal("Offset CBasePlayerPawn_CommitSuicide failed its Vtable Check on this build (fingerprint mismatch)", ex.Message);
    }

    [Fact]
    public void ASlotBindingNativeDidNotResolveIsReadFromGamedata()
    {
        // e.g. a key with Slot Binding fields in a plugin's own gamedata file: native only resolves the core Gamedata.
        LoadGameData($"{{ {CommitSuicideEntry(387)} }}");
        InstallReport();

        Assert.Equal(387, GameData.GetOffset("CBasePlayerPawn_CommitSuicide"));
    }
}
