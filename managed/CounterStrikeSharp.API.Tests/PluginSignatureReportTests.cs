using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// The plugin signatures report: one entry per plugin, module and signature with its match count and outcome, a block
// listing only the ambiguous and not-found ones, and the query API. Against a faked scan, as in PluginSignatureTests.
[Collection(nameof(PluginSignatureTests))]
public sealed class PluginSignatureReportTests : IDisposable
{
    private const string BinaryPath = "/game/csgo/bin/linuxsteamrt64/libserver.so";
    private static readonly IntPtr FirstMatch = new(0x1234);

    private readonly Func<string, string, int> _originalCount = SignatureFunctions.CountMatches;
    private readonly Func<string, string, DataType, DataType[], IntPtr> _originalCreate = SignatureFunctions.CreateFunction;
    private readonly Func<string?> _originalPlugin = PluginSignatures.CurrentPlugin;
    private readonly CapturingLogger _logger = new();
    private readonly Dictionary<string, int> _matchCounts = new();
    private string? _plugin = "MyPlugin";

    // Unique per test so the process-wide caches never leak between tests.
    private readonly string _id = Guid.NewGuid().ToString("N")[..8];
    private string Sig(string name) => $"55 48 {name} {_id}";

    public PluginSignatureReportTests()
    {
        PluginSignatures.Reset();
        PluginSignatures.CurrentPlugin = () => _plugin;
        SignatureFunctions.Logger = _logger;
        SignatureFunctions.CountMatches = (_, signature) => _matchCounts.TryGetValue(signature, out var count) ? count : 1;
        SignatureFunctions.CreateFunction = (_, signature, _, _) =>
            _matchCounts.TryGetValue(signature, out var count) && count == 0
                ? throw new NativeException($"Could not find signature {signature}")
                : FirstMatch;
    }

    public void Dispose()
    {
        SignatureFunctions.CountMatches = _originalCount;
        SignatureFunctions.CreateFunction = _originalCreate;
        SignatureFunctions.Logger = null;
        PluginSignatures.CurrentPlugin = _originalPlugin;
        PluginSignatures.Reset();
    }

    private void Resolve(string signature, int matches, bool strict = false)
    {
        _matchCounts[signature] = matches;
        _ = new MemoryFunctionVoid<int>(signature, BinaryPath, strict);
    }

    [Fact]
    public void EachPluginSignatureIsRecordedWithPluginModuleMatchCountAndOutcome()
    {
        Resolve(Sig("unique"), 1);
        Resolve(Sig("ambiguous"), 3);
        Resolve(Sig("strict"), 2, strict: true);
        Resolve(Sig("missing"), 0);

        var entries = EngineFeatures.GetPluginSignatures();

        Assert.Equal(4, entries.Count);
        Assert.All(entries, e => Assert.Equal(("MyPlugin", "libserver.so"), (e.Plugin, e.Module)));
        Assert.Equal(
            [
                (Sig("ambiguous"), 3, PluginSignatureOutcome.Ambiguous),
                (Sig("missing"), 0, PluginSignatureOutcome.NotFound),
                (Sig("strict"), 2, PluginSignatureOutcome.AmbiguousRejected),
                (Sig("unique"), 1, PluginSignatureOutcome.Resolved),
            ],
            entries.Select(e => (e.Signature, e.MatchCount, e.Outcome)));
    }

    [Fact]
    public void TheBlockListsOnlyAmbiguousAndNotFoundSignaturesWithPluginAndMatchCount()
    {
        Resolve(Sig("unique"), 1);
        Resolve(Sig("ambiguous"), 3);
        Resolve(Sig("missing"), 0);

        PluginSignatures.LogReport(_logger);

        var block = _logger.Entries.Last();
        Assert.Equal(LogLevel.Warning, block.Level);
        Assert.Equal(
            "Plugin signatures: 2 of 3 ambiguous or not found:\n" +
            $"  MyPlugin: \"{Sig("ambiguous")}\" in libserver.so: ambiguous (3 matches), first match used\n" +
            $"  MyPlugin: \"{Sig("missing")}\" in libserver.so: not found", block.Message);
    }

    [Fact]
    public void NoBlockWhenEveryPluginSignatureIsUnique()
    {
        Resolve(Sig("unique"), 1);

        Assert.Null(PluginSignatures.FormatReport(PluginSignatures.All));
        PluginSignatures.LogReport(_logger);
        Assert.Empty(_logger.Entries);
    }

    [Fact]
    public void ASignatureReadFromGamedataIsReportedByItsKey()
    {
        PluginSignatures.NoteGamedataKey(Sig("keyed"), "MyPlugin_Function");
        Resolve(Sig("keyed"), 0);

        var entry = Assert.Single(EngineFeatures.GetPluginSignatures());
        Assert.Equal("MyPlugin_Function", entry.GamedataKey);
        Assert.Equal($"MyPlugin: MyPlugin_Function \"{Sig("keyed")}\" in libserver.so: not found", entry.ToString());
    }

    [Fact]
    public void CounterStrikeSharpsOwnSignaturesAreNotPluginSignatures()
    {
        _plugin = null;

        Resolve(Sig("core"), 3);

        Assert.Empty(EngineFeatures.GetPluginSignatures());
    }

    [Fact]
    public void EachPluginGetsItsOwnEntryForTheSameSignature()
    {
        Resolve(Sig("shared"), 3);
        _plugin = "OtherPlugin";
        Resolve(Sig("shared"), 3);

        Assert.Equal(["MyPlugin", "OtherPlugin"], EngineFeatures.GetPluginSignatures().Select(e => e.Plugin));
    }

    [Fact]
    public void APluginLoadingAgainReplacesItsEntries()
    {
        Resolve(Sig("old"), 3);
        _plugin = "OtherPlugin";
        Resolve(Sig("other"), 0);

        PluginSignatures.BeginPluginLoad("MyPlugin", typeof(PluginSignatureReportTests).Assembly);
        _plugin = "MyPlugin";
        Resolve(Sig("old"), 3); // cached function, recorded again

        Assert.Equal([("MyPlugin", Sig("old")), ("OtherPlugin", Sig("other"))],
            EngineFeatures.GetPluginSignatures().Select(e => (e.Plugin, e.Signature)));
    }

    [Fact]
    public void StrictRejectsAnAmbiguousSignatureEvenAfterANonStrictCallerBoundIt()
    {
        Resolve(Sig("ambiguous"), 3);
        _matchCounts[Sig("ambiguous")] = 3;

        var strict = new MemoryFunctionVoid<int>(Sig("ambiguous"), BinaryPath, strict: true);

        Assert.Equal(IntPtr.Zero, strict.Handle);
        Assert.Equal(PluginSignatureOutcome.AmbiguousRejected, Assert.Single(EngineFeatures.GetPluginSignatures()).Outcome);
    }

    [Fact]
    public void OutsideAPluginTheCallerIsNotAttributed()
    {
        // Unit tests run in the default load context, like CounterStrikeSharp itself.
        Assert.Null(_originalPlugin());
    }

    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }
}
