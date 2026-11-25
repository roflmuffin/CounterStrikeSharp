using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// The ISource2Server Engine Features (hooks and direct calls by verified slot index): listeners and frame tasks on a
// disabled one register, warn once and never fire; each fails on its own.
public class ISource2ServerEngineFeatureTests
{
    private const string BuildId = "1.42.0.1/2000950";

    // Engine Feature -> its Slot Bindings, as the native catalog declares them.
    private static readonly (string Name, string[] Keys)[] Features =
    [
        (EngineFeatureNames.ServerGameFrame, ["ISource2Server::GameFrame"]),
        (EngineFeatureNames.ServerHibernationEvents, ["ISource2Server::ServerHibernationUpdate"]),
        (EngineFeatureNames.ServerSteamApiEvents,
            ["ISource2Server::GameServerSteamAPIActivated", "ISource2Server::GameServerSteamAPIDeactivated"]),
        (EngineFeatureNames.ServerHostnameChanged, ["ISource2Server::OnHostNameChanged"]),
        (EngineFeatureNames.ServerFatalShutdown, ["ISource2Server::PreFatalShutdown"]),
        (EngineFeatureNames.ServerUpdateWhenNotInGame, ["ISource2Server::UpdateWhenNotInGame"]),
        (EngineFeatureNames.ServerPreWorldUpdate, ["ISource2Server::PreWorldUpdate"]),
        (EngineFeatureNames.ServerEconItemSystem, ["ISource2Server::GetEconItemSystem"]),
        (EngineFeatureNames.ServerIsPaused, ["ISource2Server::IsPaused"]),
    ];

    private sealed class CapturingLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    // Every ISource2Server Slot Binding resolved, except failingKey (fingerprint mismatch).
    private static EngineFeatureSet SetWith(string? failingKey = null)
    {
        var bindings = Features.SelectMany(f => f.Keys).Select(key => key == failingKey
            ? new BindingResult(key, BindingKind.Slot, false, BindingFailureReason.FingerprintMismatch, 1, 0x1000, 0, "server",
                Slot: new SlotEvidence("CSource2Server", 20, "None", 105, 105))
            : new BindingResult(key, BindingKind.Slot, true, BindingFailureReason.None, 1, 0x1000, 20, "server",
                Slot: new SlotEvidence("CSource2Server", 20, "Window", 105, 105)));
        var declarations = Features.Select(f => new EngineFeatureDeclaration(f.Name, f.Keys.Select(BindingDeclaration.Any).ToList()));
        return new EngineFeatureSet(new ResolutionReport(BuildId, bindings, declarations), []);
    }

    public static IEnumerable<object[]> AllKeys() => Features.SelectMany(f => f.Keys).Select(key => new object[] { key });

    [Theory]
    [MemberData(nameof(AllKeys))]
    public void AFailedSlotBindingDisablesOnlyItsOwnEngineFeature(string failingKey)
    {
        var set = SetWith(failingKey);

        foreach (var (name, keys) in Features)
        {
            Assert.Equal(!keys.Contains(failingKey), set.IsAvailable(name));
        }

        var failing = Features.Single(f => f.Keys.Contains(failingKey)).Name;
        Assert.Equal(
            $"Engine Features for build {BuildId}: {Features.Length - 1} of {Features.Length} available, 1 disabled:\n" +
            $"  {failing} disabled ({failingKey}: fingerprint mismatch)",
            set.FormatStartupReport());
    }

    [Theory]
    [InlineData("OnServerHibernationUpdate", "cssharp.server/hibernation-events", "ISource2Server::ServerHibernationUpdate")]
    [InlineData("OnGameServerSteamAPIActivated", "cssharp.server/steam-api-events", "ISource2Server::GameServerSteamAPIActivated")]
    [InlineData("OnGameServerSteamAPIDeactivated", "cssharp.server/steam-api-events", "ISource2Server::GameServerSteamAPIActivated")]
    [InlineData("OnHostNameChanged", "cssharp.server/hostname-changed", "ISource2Server::OnHostNameChanged")]
    [InlineData("OnPreFatalShutdown", "cssharp.server/fatal-shutdown", "ISource2Server::PreFatalShutdown")]
    [InlineData("OnUpdateWhenNotInGame", "cssharp.server/update-when-not-in-game", "ISource2Server::UpdateWhenNotInGame")]
    [InlineData("OnServerPreWorldUpdate", "cssharp.server/pre-world-update", "ISource2Server::PreWorldUpdate")]
    public void AListenerOnADisabledISource2ServerHookRegistersAndWarnsOnce(string listener, string feature, string failingKey)
    {
        var set = SetWith(failingKey);
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(set, listener, logger));
        Assert.False(ListenerEngineFeatures.CheckRegistration(set, listener, logger));

        Assert.Equal(
            $"{listener} listener registered on {feature} disabled ({failingKey}: fingerprint mismatch); it will never fire",
            Assert.Single(logger.Messages));
        Assert.True(ListenerEngineFeatures.CheckRegistration(SetWith(), listener, logger));
    }

    [Theory]
    [InlineData("NextFrame", "cssharp.server/game-frame", "ISource2Server::GameFrame")]
    [InlineData("RunOnTick", "cssharp.server/game-frame", "ISource2Server::GameFrame")]
    [InlineData("NextWorldUpdate", "cssharp.server/pre-world-update", "ISource2Server::PreWorldUpdate")]
    public void AFrameTaskOnADisabledHookIsQueuedAndWarnsOnce(string api, string feature, string failingKey)
    {
        var set = SetWith(failingKey);
        var logger = new CapturingLogger();

        Assert.False(FrameTaskEngineFeatures.CheckRegistration(set, api, logger));
        Assert.False(FrameTaskEngineFeatures.CheckRegistration(set, api, logger));

        Assert.Equal(
            $"{api} task registered on {feature} disabled ({failingKey}: fingerprint mismatch); it will never fire",
            Assert.Single(logger.Messages));
        Assert.True(FrameTaskEngineFeatures.CheckRegistration(SetWith(), api, logger));
    }

    [Fact]
    public void FrameTasksDoNotDependOnOtherISource2ServerHooks()
    {
        var logger = new CapturingLogger();

        Assert.True(FrameTaskEngineFeatures.CheckRegistration(SetWith("ISource2Server::PreWorldUpdate"), "NextFrame", logger));
        Assert.True(FrameTaskEngineFeatures.CheckRegistration(SetWith("ISource2Server::GameFrame"), "NextWorldUpdate", logger));
        Assert.Empty(logger.Messages);
    }
}
