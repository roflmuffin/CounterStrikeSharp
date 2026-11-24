using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// The core memory functions (VirtualFunctions) bind lazily, per member, from the Resolution Report (ADR-0002): a failed
// Binding only disables its own helpers, calling one throws the typed exception. Uses the process-wide report, so it
// shares the collection with the other tests that install one.
[Collection(nameof(EngineFeaturesApiTests))]
public class CoreFunctionTests : IDisposable
{
    private const string BuildId = "1.42.0.1/2000950";

    private static BindingResult Resolved(string key, nint address) =>
        new(key, BindingKind.Signature, true, BindingFailureReason.None, 1, address, 0, "server");

    private static BindingResult Failed(string key, BindingFailureReason reason, int matchCount = 0) =>
        new(key, BindingKind.Signature, false, reason, matchCount, 0, 0, "server");

    private static void InstallReport(params BindingResult[] bindings) =>
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport(BuildId, bindings, []), ManagedEngineFeatures.All);

    private readonly Func<nint, DataType, DataType[], IntPtr> _originalCreate = CoreFunction.CreateFunction;
    private readonly CapturingLogger _logger = new();
    private readonly List<nint> _createdAt = new();

    public CoreFunctionTests()
    {
        CoreFunction.Logger = _logger;
        CoreFunction.CreateFunction = (address, _, _) =>
        {
            _createdAt.Add(address);
            return address + 0x10;
        };
    }

    public void Dispose()
    {
        EngineFeatures.Reset();
        CoreFunction.CreateFunction = _originalCreate;
        CoreFunction.Logger = null; // back to the core logger
    }

    [Fact]
    public void CallingClientPrintWhileClientPrintIsDisabledThrowsTheTypedException()
    {
        InstallReport(Failed("ClientPrint", BindingFailureReason.Ambiguous, 2), Resolved("UTIL_ClientPrintAll", 0x2000));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            VirtualFunctions.ClientPrint(IntPtr.Zero, HudDestination.Chat, "hello", 0, 0, 0, 0));

        Assert.Equal("cssharp.chat/client-print", exception.EngineFeature);
        Assert.Equal("ClientPrint", exception.Binding);
        Assert.Equal(BindingFailureReason.Ambiguous, exception.Reason);
        Assert.Equal(2, exception.MatchCount);
    }

    [Fact]
    public void PrintToChatAllWhileClientPrintIsDisabledThrowsTheTypedException()
    {
        InstallReport(Resolved("ClientPrint", 0x1000), Failed("UTIL_ClientPrintAll", BindingFailureReason.NotFound));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => Server.PrintToChatAll("hello"));

        Assert.Equal("Engine Feature cssharp.chat/client-print is disabled (UTIL_ClientPrintAll: not found)", exception.Message);
        Assert.Empty(_createdAt);
    }

    [Fact]
    public void ACoreFunctionBindsToTheAddressInTheReportOnFirstUse()
    {
        InstallReport(Resolved("ClientPrint", 0x1000), Resolved("UTIL_ClientPrintAll", 0x2000));
        var clientPrintAll = new MemoryFunctionVoid<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>(
            CoreFunction.FromEngineFeature(ManagedEngineFeatures.ChatClientPrint, "UTIL_ClientPrintAll"));

        Assert.Empty(_createdAt);
        Assert.Equal((IntPtr)0x2010, clientPrintAll.Handle);
        Assert.Equal((IntPtr)0x2010, clientPrintAll.Handle);
        Assert.Equal([(nint)0x2000], _createdAt);
    }

    [Fact]
    public void OtherHelpersInTheTableStillWorkWhenClientPrintIsDisabled()
    {
        InstallReport(Failed("ClientPrint", BindingFailureReason.NotFound), Failed("UTIL_ClientPrintAll", BindingFailureReason.NotFound),
            Resolved("CBaseModelEntity_SetModel", 0x5000));

        Assert.Throws<EngineFeatureUnavailableException>(() =>
            VirtualFunctions.ClientPrintAll(HudDestination.Chat, "hello", 0, 0, 0, 0, 0));
        Assert.Equal((IntPtr)0x5010, VirtualFunctions.SetModelFunc.Handle);
    }

    [Fact]
    public void AKeyMissingFromGamedataOnlyBreaksItsOwnHelper()
    {
        // UTIL_Remove is absent from the report.
        InstallReport(Resolved("ClientPrint", 0x1000), Resolved("UTIL_ClientPrintAll", 0x2000),
            Resolved("UTIL_CreateEntityByName", 0x6000));

        var missing = Assert.Throws<EngineFeatureUnavailableException>(() => VirtualFunctions.UTIL_Remove(IntPtr.Zero));
        Assert.Equal("Engine Feature cssharp.entity/remove is disabled (UTIL_Remove: missing from gamedata)", missing.Message);
        Assert.Equal((IntPtr)0x6010, VirtualFunctions.UTIL_CreateEntityByNameFunc.Handle);
    }

    [Fact]
    public void HookingADisabledCoreFunctionRegistersLogsOnceAndInstallsNothing()
    {
        InstallReport(Failed("ClientPrint", BindingFailureReason.NotFound), Resolved("UTIL_ClientPrintAll", 0x2000));
        Func<DynamicHook, HookResult> handler = _ => HookResult.Continue;

        VirtualFunctions.ClientPrintFunc.Hook(handler, HookMode.Pre);
        VirtualFunctions.ClientPrintFunc.Hook(handler, HookMode.Post);
        VirtualFunctions.ClientPrintFunc.Unhook(handler, HookMode.Pre);

        var warning = Assert.Single(_logger.Entries);
        Assert.Equal(LogLevel.Warning, warning.Level);
        Assert.Equal("Memory function hook on ClientPrint registered on cssharp.chat/client-print disabled " +
                     "(ClientPrint: not found); it will never fire", warning.Message);
        Assert.Empty(_createdAt);
    }

    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Entries.Add((logLevel, formatter(state, exception)));
    }
}
