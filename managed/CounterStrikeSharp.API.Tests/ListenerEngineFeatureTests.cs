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
            $"{listenerName} listener registered on disabled cssharp.server/game-system-events disabled " +
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
    public void ListenerNotBackedByAnEngineFeatureIsUnaffected()
    {
        var logger = new CapturingLogger();

        Assert.True(ListenerEngineFeatures.CheckRegistration(SetWithGameSystemEvents(resolved: false), "OnTick", logger));
        Assert.Empty(logger.Entries);
    }
}
