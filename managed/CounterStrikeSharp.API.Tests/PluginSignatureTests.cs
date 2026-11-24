using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// Plugin raw signatures against a faked scan: first match kept, ambiguity warned once (or rejected in strict mode),
// not-found reported at creation and on call.
public class PluginSignatureTests : IDisposable
{
    private const string BinaryPath = "/game/csgo/bin/linuxsteamrt64/libserver.so";
    private static readonly IntPtr FirstMatch = new(0x1234);

    private readonly Func<string, string, int> _originalCount = SignatureFunctions.CountMatches;
    private readonly Func<string, string, DataType, DataType[], IntPtr> _originalCreate = SignatureFunctions.CreateFunction;

    private readonly CapturingLogger _logger = new();
    private readonly Dictionary<string, int> _matchCounts = new();
    private int _createCalls;

    // Unique per test so the process-wide caches never leak between tests.
    private readonly string _signature = "55 48 89 E5 " + Guid.NewGuid().ToString("N")[..8];

    public PluginSignatureTests()
    {
        SignatureFunctions.Logger = _logger;
        SignatureFunctions.CountMatches = (_, signature) => _matchCounts.TryGetValue(signature, out var count) ? count : 1;
        SignatureFunctions.CreateFunction = (_, signature, _, _) =>
        {
            _createCalls++;
            if (_matchCounts.TryGetValue(signature, out var count) && count == 0)
                throw new NativeException($"Could not find signature {signature}");
            return FirstMatch;
        };
    }

    public void Dispose()
    {
        SignatureFunctions.CountMatches = _originalCount;
        SignatureFunctions.CreateFunction = _originalCreate;
        SignatureFunctions.Logger = null; // back to the core logger
    }

    [Fact]
    public void UniqueSignatureBindsWithoutLogging()
    {
        _matchCounts[_signature] = 1;

        var function = new MemoryFunctionVoid<int>(_signature, BinaryPath);

        Assert.Equal(FirstMatch, function.Handle);
        Assert.Empty(_logger.Entries);
    }

    [Fact]
    public void AmbiguousSignatureBindsTheFirstMatchAndWarnsOnceWithTheCount()
    {
        _matchCounts[_signature] = 3;

        var first = new MemoryFunctionVoid<int>(_signature, BinaryPath);
        var second = new MemoryFunctionWithReturn<int, bool>(_signature, BinaryPath);

        Assert.Equal(FirstMatch, first.Handle);
        Assert.Equal(FirstMatch, second.Handle);
        var warning = Assert.Single(_logger.Entries);
        Assert.Equal(LogLevel.Warning, warning.Level);
        Assert.Contains(_signature, warning.Message);
        Assert.Contains("3 matches", warning.Message);
    }

    [Fact]
    public void AmbiguousSignatureInStrictModeFailsAsAmbiguous()
    {
        _matchCounts[_signature] = 3;

        var function = new MemoryFunctionVoid<int>(_signature, BinaryPath, strict: true);

        Assert.Equal(IntPtr.Zero, function.Handle);
        Assert.Equal(0, _createCalls);
        var error = Assert.Single(_logger.Entries);
        Assert.Equal(LogLevel.Error, error.Level);
        Assert.Contains(_signature, error.Message);
        Assert.Contains("ambiguous (3 matches)", error.Message);

        var ex = Assert.Throws<NativeException>(() => function.Invoke(1));
        Assert.Contains(_signature, ex.Message);
        Assert.Contains("ambiguous (3 matches)", ex.Message);
    }

    [Fact]
    public void UniqueSignatureInStrictModeBinds()
    {
        _matchCounts[_signature] = 1;

        var function = new MemoryFunctionVoid<int>(_signature, BinaryPath, strict: true);

        Assert.Equal(FirstMatch, function.Handle);
        Assert.Empty(_logger.Entries);
    }

    [Fact]
    public void SignatureNotFoundLogsAtCreationAndThrowsClearlyWhenCalledOrHooked()
    {
        _matchCounts[_signature] = 0;

        var function = new MemoryFunctionVoid<int>(_signature, BinaryPath);

        Assert.Equal(IntPtr.Zero, function.Handle);
        var error = Assert.Single(_logger.Entries);
        Assert.Equal(LogLevel.Error, error.Level);
        Assert.Contains(_signature, error.Message);
        Assert.Contains("not found", error.Message);

        var invoke = Assert.Throws<NativeException>(() => function.Invoke(1));
        Assert.Contains(_signature, invoke.Message);
        Assert.Contains("not found", invoke.Message);
        var hook = Assert.Throws<NativeException>(() => function.Hook(_ => HookResult.Continue, HookMode.Pre));
        Assert.Contains("not found", hook.Message);
    }

    [Fact]
    public void ScanFailureDoesNotChangeFirstMatchBehaviour()
    {
        SignatureFunctions.CountMatches = (_, _) => throw new NativeException("scan unavailable");

        var function = new MemoryFunctionVoid<int>(_signature, BinaryPath);

        Assert.Equal(FirstMatch, function.Handle);
        Assert.Empty(_logger.Entries);
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
