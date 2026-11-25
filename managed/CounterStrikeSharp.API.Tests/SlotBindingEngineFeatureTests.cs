using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// Slot Bindings in the Resolution Report (ADR-0003), with cssharp.server/game-frame as the example.
public class SlotBindingEngineFeatureTests
{
    private const string BuildId = "1.42.0.1/2000950";
    private const string GameFrameKey = "ISource2Server::GameFrame";

    private static readonly EngineFeatureDeclaration GameFrame =
        new(EngineFeatureNames.ServerGameFrame, [BindingDeclaration.Any(GameFrameKey)]);

    private sealed class CapturingLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    private static BindingResult Slot(bool resolved, BindingFailureReason reason = BindingFailureReason.None,
        bool unverified = false, long vtableSize = 105, long? recordedVtableSize = 105) =>
        new(GameFrameKey, BindingKind.Slot, resolved, reason, 1, 0x1000, 20, "server", unverified,
            new SlotEvidence("CSource2Server", 20, resolved ? "Window" : "None", vtableSize, recordedVtableSize));

    private static EngineFeatureSet Features(BindingResult gameFrame) =>
        new(new ResolutionReport(BuildId, [gameFrame], [GameFrame]), []);

    [Fact]
    public void TheReportReadsASlotBindingWithItsStatusAndEvidence()
    {
        // Shape written by bindings::ResolutionReportToJson (src/core/binding_resolver.h).
        const string json = """
            {"buildId":"1.42.0.1/2000950",
             "bindings":[
               {"key":"ISource2Server::GameFrame","kind":"Slot","resolved":false,"status":"Failed",
                "reason":"FingerprintMismatch","matchCount":1,"address":4096,"offset":0,"library":"server",
                "slot":{"class":"CSource2Server","statedIndex":20,"match":"None","vtableSize":105,"recordedVtableSize":105}},
               {"key":"Other::Slot","kind":"Slot","resolved":true,"status":"Unverified","reason":"None","matchCount":0,
                "address":0,"offset":7,"library":"",
                "slot":{"class":"","statedIndex":7,"match":"None","vtableSize":0,"recordedVtableSize":null}}],
             "features":[{"name":"cssharp.server/game-frame","bindings":["ISource2Server::GameFrame"]}]}
            """;

        var report = ResolutionReport.FromJson(json);

        var gameFrame = report.Find(GameFrameKey)!;
        Assert.Equal(BindingKind.Slot, gameFrame.Kind);
        Assert.Equal(BindingFailureReason.FingerprintMismatch, gameFrame.Reason);
        Assert.False(gameFrame.Unverified);
        Assert.Equal(new SlotEvidence("CSource2Server", 20, "None", 105, 105), gameFrame.Slot);
        var other = report.Find("Other::Slot")!;
        Assert.True(other.Resolved);
        Assert.True(other.Unverified);
        Assert.Null(other.Slot!.RecordedVtableSize);

        var status = new EngineFeatureSet(report, []).GetStatus(EngineFeatureNames.ServerGameFrame);
        Assert.False(status.IsAvailable);
        Assert.Equal("ISource2Server::GameFrame: fingerprint mismatch", status.Describe());
    }

    [Fact]
    public void AFingerprintMismatchDisablesGameFrameInTheStartupReport()
    {
        Assert.Equal(
            $"Engine Features for build {BuildId}: 0 of 1 available, 1 disabled:\n" +
            "  cssharp.server/game-frame disabled (ISource2Server::GameFrame: fingerprint mismatch)",
            Features(Slot(false, BindingFailureReason.FingerprintMismatch)).FormatStartupReport());
    }

    [Fact]
    public void AVtableSizeChangeIsAWarningInTheStartupReportNotAFailure()
    {
        var features = Features(Slot(true, vtableSize: 106));

        Assert.True(features.IsAvailable(EngineFeatureNames.ServerGameFrame));
        Assert.Equal(
            $"Engine Features for build {BuildId}: all 1 available\n" +
            "  Vtable of CSource2Server (server) has 106 slots, Gamedata recorded 105 (ISource2Server::GameFrame)",
            features.FormatStartupReport());
    }

    [Fact]
    public void AnUnverifiedSlotBindingIsAvailableAndListedInTheStartupReport()
    {
        var features = Features(Slot(true, unverified: true, vtableSize: 0, recordedVtableSize: null));

        Assert.True(features.IsAvailable(EngineFeatureNames.ServerGameFrame));
        Assert.Equal(
            $"Engine Features for build {BuildId}: all 1 available\n" +
            "  1 Slot Binding(s) unverified (no Vtable Check on this platform or no fingerprint window in Gamedata): " +
            "ISource2Server::GameFrame",
            features.FormatStartupReport());
    }

    [Fact]
    public void OnTickOnADisabledGameFrameRegistersAndWarnsOnce()
    {
        var features = Features(Slot(false, BindingFailureReason.FingerprintMismatch));
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(features, "OnTick", logger));
        Assert.False(ListenerEngineFeatures.CheckRegistration(features, "OnTick", logger));

        Assert.Equal(
            "OnTick listener registered on cssharp.server/game-frame disabled (ISource2Server::GameFrame: fingerprint mismatch); " +
            "it will never fire",
            Assert.Single(logger.Messages));
    }
}
