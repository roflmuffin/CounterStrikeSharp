using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// Listeners backed by a disabled Engine Feature register, log once, and never fire (ADR-0002).
public class ListenerEngineFeatureTests
{
    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Entries.Add((logLevel, formatter(state, exception)));
    }

    // The native-owned Engine Feature as the native report carries it.
    private static EngineFeatureSet SetWithGameSystemEvents(bool resolved)
    {
        var gameSystemEvents = new EngineFeatureDeclaration("cssharp.server/game-system-events",
            [BindingDeclaration.Signature("IGameSystem_InitAllSystems_pFirst")]);
        var report = new ResolutionReport("1.42.0.1/2000950",
            [
                new BindingResult("IGameSystem_InitAllSystems_pFirst", BindingKind.Signature, resolved,
                    resolved ? BindingFailureReason.None : BindingFailureReason.NotFound, resolved ? 1 : 0, resolved ? 0x1000 : 0, 0,
                    "server")
            ],
            [gameSystemEvents]);
        return new EngineFeatureSet(report, []);
    }

    [Theory]
    [InlineData("OnServerPrecacheResources")]
    [InlineData("OnServerPreEntityThink")]
    [InlineData("OnServerPostEntityThink")]
    [InlineData("OnPlayerButtonsChanged")]
    public void GameSystemListenerOnDisabledGameSystemEventsWarnsOnce(string listenerName)
    {
        var set = SetWithGameSystemEvents(resolved: false);
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(set, listenerName, logger));
        Assert.False(ListenerEngineFeatures.CheckRegistration(set, listenerName, logger));

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal(
            $"{listenerName} listener registered on cssharp.server/game-system-events disabled " +
            "(IGameSystem_InitAllSystems_pFirst: not found); it will never fire",
            entry.Message);
    }

    [Fact]
    public void GameSystemListenerOnAvailableGameSystemEventsIsSilent()
    {
        var logger = new CapturingLogger();

        Assert.True(ListenerEngineFeatures.CheckRegistration(SetWithGameSystemEvents(resolved: true), "OnServerPreEntityThink", logger));
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void ChatListenerOnDisabledSayHooksWarnsOnce()
    {
        var sayHooks = new EngineFeatureDeclaration("cssharp.chat/say-hooks", [BindingDeclaration.Signature("Host_Say")]);
        var report = new ResolutionReport("1.42.0.1/2000950",
            [new BindingResult("Host_Say", BindingKind.Signature, false, BindingFailureReason.NotFound, 0, 0, 0, "server")],
            [sayHooks]);
        var set = new EngineFeatureSet(report, []);
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(set, "OnPlayerChat", logger));

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(
            "OnPlayerChat listener registered on cssharp.chat/say-hooks disabled (Host_Say: not found); it will never fire",
            entry.Message);
    }

    [Fact]
    public void ListenersOfDifferentPluginsAreEachWarnedOnce()
    {
        var set = SetWithGameSystemEvents(resolved: false);
        var logger = new CapturingLogger();
        object firstPlugin = new(), secondPlugin = new();

        ListenerEngineFeatures.CheckRegistration(set, "OnServerPreEntityThink", logger, firstPlugin);
        ListenerEngineFeatures.CheckRegistration(set, "OnServerPreEntityThink", logger, secondPlugin);
        ListenerEngineFeatures.CheckRegistration(set, "OnServerPreEntityThink", logger, secondPlugin);

        Assert.Equal(2, logger.Entries.Count);
    }

    [Fact]
    public void ListenerNotBackedByAnEngineFeatureIsUnaffected()
    {
        var logger = new CapturingLogger();

        Assert.True(ListenerEngineFeatures.CheckRegistration(SetWithGameSystemEvents(resolved: false), "OnEntityCreated", logger));
        Assert.Empty(logger.Entries);
    }
}
