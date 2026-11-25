using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// Interfaces that never shifted a slot we use: verified Slot Bindings whose direct calls stay compiled. A compiled call
// cannot follow a Heal, so its Binding fails when verified at another index (CompiledSlotMoved).
[Collection(nameof(EngineFeaturesApiTests))]
public class NeverMovedEngineFeatureTests : IDisposable
{
    private const string BuildId = "1.42.0.1/2000950";
    private const string FireEventKey = "IGameEventManager2::FireEvent";

    public void Dispose() => EngineFeatures.Reset();

    private sealed class CapturingLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    // Shape written by bindings::ResolutionReportToJson: FireEvent healed from 8 to 9, but its compiled call uses 8.
    private const string MovedFireEventJson = """
        {"buildId":"1.42.0.1/2000950",
         "bindings":[
           {"key":"IGameEventManager2::FireEvent","kind":"Slot","resolved":false,"status":"Failed",
            "reason":"CompiledSlotMoved","matchCount":1,"address":4096,"offset":9,"library":"server",
            "slot":{"class":"CGameEventManager","statedIndex":8,"match":"Heal","vtableSize":17,"recordedVtableSize":17,
                    "window":["878a822908b0c27a","e3790165e6bf1695","f98decce9427247e"]}},
           {"key":"INetworkMessages::FindNetworkMessagePartial","kind":"Slot","resolved":true,"status":"Unverified",
            "reason":"None","matchCount":0,"address":0,"offset":10,"library":"networksystem",
            "slot":{"class":"CNetworkMessages","statedIndex":10,"match":"None","vtableSize":0,"recordedVtableSize":null,
                    "window":null}},
           {"key":"CSchemaSystem::FindTypeScopeForModule","kind":"Slot","resolved":true,"status":"Unverified",
            "reason":"None","matchCount":0,"address":0,"offset":13,"library":"schemasystem",
            "slot":{"class":"CSchemaSystem","statedIndex":13,"match":"None","vtableSize":0,"recordedVtableSize":null,
                    "window":null}}],
         "features":[{"name":"cssharp.events/game-events","bindings":["IGameEventManager2::FireEvent"]}]}
        """;

    [Fact]
    public void ACompiledCallsMovedSlotDisablesItsEngineFeatureInTheStartupReport()
    {
        var features = new EngineFeatureSet(ResolutionReport.FromJson(MovedFireEventJson), []);

        var fireEvent = features.Report.Find(FireEventKey)!;
        Assert.Equal(BindingFailureReason.CompiledSlotMoved, fireEvent.Reason);
        Assert.False(fireEvent.Resolved);
        Assert.False(fireEvent.Healed);
        Assert.False(features.IsAvailable(EngineFeatureNames.EventsGameEvents));
        Assert.Equal(
            $"Engine Features for build {BuildId}: 0 of 1 available, 1 disabled:\n" +
            "  cssharp.events/game-events disabled (IGameEventManager2::FireEvent: compiled call's slot moved)\n" +
            "  2 Slot Binding(s) unverified (no Vtable Check on this platform or no fingerprint window in Gamedata): " +
            "INetworkMessages::FindNetworkMessagePartial, CSchemaSystem::FindTypeScopeForModule",
            features.FormatStartupReport());
    }

    [Fact]
    public void CallingIntoTheDisabledEngineFeatureThrowsTheTypedExceptionWithTheReason()
    {
        EngineFeatures.Current = new EngineFeatureSet(ResolutionReport.FromJson(MovedFireEventJson), []);

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            EngineFeatures.Current.ThrowIfUnavailable(EngineFeatureNames.EventsGameEvents));

        Assert.Equal(FireEventKey, exception.Binding);
        Assert.Equal(BindingFailureReason.CompiledSlotMoved, exception.Reason);
        Assert.Equal(
            "Engine Feature cssharp.events/game-events is disabled (IGameEventManager2::FireEvent: compiled call's slot moved)",
            exception.Message);
    }

    [Fact]
    public void OnMapStartListenerOnADisabledStartupWarnsOnce()
    {
        var startup = new EngineFeatureDeclaration(EngineFeatureNames.ServerStartup,
            [BindingDeclaration.Any("INetworkServerService::StartupServer")]);
        var report = new ResolutionReport(BuildId,
            [
                new BindingResult("INetworkServerService::StartupServer", BindingKind.Slot, false,
                    BindingFailureReason.FingerprintMismatch, 1, 0x1000, 27, "engine")
            ],
            [startup]);
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(new EngineFeatureSet(report, []), "OnMapStart", logger));

        Assert.Equal(
            "OnMapStart listener registered on cssharp.server/startup disabled " +
            "(INetworkServerService::StartupServer: fingerprint mismatch); it will never fire",
            Assert.Single(logger.Messages));
    }

    [Fact]
    public void TheNeverMovedEngineFeaturesHaveStableNames()
    {
        Assert.Equal("cssharp.server/startup", EngineFeatureNames.ServerStartup);
        Assert.Equal("cssharp.server/loop-mode", EngineFeatureNames.ServerLoopMode);
        Assert.Equal("cssharp.server/global-vars", EngineFeatureNames.ServerGlobalVars);
        Assert.Equal("cssharp.network/user-messages", EngineFeatureNames.NetworkUserMessages);
    }
}
