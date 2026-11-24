using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// Hooks and listeners on a disabled Engine Feature register, log once, and never fire (ADR-0002).
public class EngineFeatureRegistrationTests
{
    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Entries.Add((logLevel, formatter(state, exception)));
    }

    private static EngineFeatureSet SetWithSayHooks(bool resolved)
    {
        var sayHooks = new EngineFeatureDeclaration("cssharp.chat/say-hooks", [BindingDeclaration.Signature("Host_Say")]);
        var report = new ResolutionReport("1.42.0.1/2000950",
            [
                new BindingResult("Host_Say", BindingKind.Signature, resolved,
                    resolved ? BindingFailureReason.None : BindingFailureReason.NotFound, resolved ? 1 : 0, resolved ? 0x1000 : 0, 0,
                    "server")
            ],
            [sayHooks]);
        return new EngineFeatureSet(report, []);
    }

    [Fact]
    public void AvailableFeatureAllowsRegistrationWithoutLogging()
    {
        var logger = new CapturingLogger();

        Assert.True(SetWithSayHooks(resolved: true).CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger));
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void DisabledFeatureWarnsOnceNamingFeatureBindingAndReason()
    {
        var set = SetWithSayHooks(resolved: false);
        var logger = new CapturingLogger();

        Assert.False(set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger));
        Assert.False(set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger));

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal(
            "chat listener registered on cssharp.chat/say-hooks disabled (Host_Say: not found); it will never fire",
            entry.Message);
    }

    [Fact]
    public void EachKindOfRegistrationWarnsOnce()
    {
        var set = SetWithSayHooks(resolved: false);
        var logger = new CapturingLogger();

        set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger);
        set.CheckRegistration("cssharp.chat/say-hooks", "chat command", logger);
        set.CheckRegistration("cssharp.chat/say-hooks", "chat command", logger);

        Assert.Equal(2, logger.Entries.Count);
    }

    [Fact]
    public void EachRegistrantIsWarnedOnce()
    {
        var set = SetWithSayHooks(resolved: false);
        var logger = new CapturingLogger();
        object firstPlugin = new(), secondPlugin = new();

        Assert.False(set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger, firstPlugin));
        Assert.False(set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger, secondPlugin));
        Assert.False(set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", logger, secondPlugin));

        Assert.Equal(2, logger.Entries.Count);
    }

    [Fact]
    public void EachPluginLoggerIsWarnedOnceWithoutAnExplicitRegistrant()
    {
        var set = SetWithSayHooks(resolved: false);
        var firstPluginLogger = new CapturingLogger();
        var secondPluginLogger = new CapturingLogger();

        set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", firstPluginLogger);
        set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", secondPluginLogger);
        set.CheckRegistration("cssharp.chat/say-hooks", "chat listener", secondPluginLogger);

        Assert.Single(firstPluginLogger.Entries);
        Assert.Single(secondPluginLogger.Entries);
    }

    [Fact]
    public void UnknownFeatureIsTreatedAsDisabled()
    {
        var logger = new CapturingLogger();

        Assert.False(SetWithSayHooks(resolved: true).CheckRegistration("cssharp.nope/never", "listener", logger));
        Assert.Single(logger.Entries);
    }
}
