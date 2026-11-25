using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// The IServerGameClients hooks as Slot Bindings: connection events, client commands, voice and custom HUD clicks are
// separate Engine Features, and listeners on a disabled one register, log once and never fire (ADR-0002, ADR-0003).
public class ServerGameClientsEngineFeatureTests
{
    private const string BuildId = "1.42.0.1/2000950";

    private sealed class CapturingLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    // The native-owned Engine Features as the native report carries them.
    private static readonly EngineFeatureDeclaration[] Features =
    [
        new(EngineFeatureNames.PlayerConnectionEvents,
        [
            BindingDeclaration.Any("IServerGameClients::ClientConnect"),
            BindingDeclaration.Any("IServerGameClients::ClientPutInServer"),
            BindingDeclaration.Any("IServerGameClients::ClientDisconnect"),
        ]),
        new(EngineFeatureNames.PlayerClientCommands, [BindingDeclaration.Any("IServerGameClients::ClientCommand")]),
        new(EngineFeatureNames.PlayerVoiceEvents, [BindingDeclaration.Any("IServerGameClients::ClientVoice")]),
        new(EngineFeatureNames.UiCustomHudLayout, [BindingDeclaration.Any("IServerGameClients::ClientSvcUserMessage")]),
        // Command listeners also see console-issued commands through the ICvar dispatch hook (ticket 08).
        new(EngineFeatureNames.CommandsConsoleDispatch, [BindingDeclaration.Any("ICvar::DispatchConCommand")]),
    ];

    private static EngineFeatureSet WithFailed(string? failedKey)
    {
        var bindings = Features.SelectMany(f => f.Bindings).Select(b => b.Key).Select(key => key == failedKey
            ? new BindingResult(key, BindingKind.Slot, false, BindingFailureReason.FingerprintMismatch, 1, 0, 0, "server")
            : new BindingResult(key, BindingKind.Slot, true, BindingFailureReason.None, 1, 0, 12, "server"));
        return new EngineFeatureSet(new ResolutionReport(BuildId, bindings.ToList(), Features), []);
    }

    public static TheoryData<string, string> ListenerFeatures => new()
    {
        { "OnClientConnect", EngineFeatureNames.PlayerConnectionEvents },
        { "OnClientConnected", EngineFeatureNames.PlayerConnectionEvents },
        { "OnClientPutInServer", EngineFeatureNames.PlayerConnectionEvents },
        { "OnClientDisconnect", EngineFeatureNames.PlayerConnectionEvents },
        { "OnClientDisconnectPost", EngineFeatureNames.PlayerConnectionEvents },
        // OnClientAuthorized also needs cssharp.player/authorization: EngineServerEngineFeatureTests.
        { "OnClientVoice", EngineFeatureNames.PlayerVoiceEvents },
        { "OnCustomHudClicked", EngineFeatureNames.UiCustomHudLayout },
    };

    [Theory]
    [MemberData(nameof(ListenerFeatures))]
    public void ListenerOnItsDisabledEngineFeatureWarnsOnce(string listenerName, string feature)
    {
        var failedKey = Features.Single(f => f.Name == feature).Bindings[0].Key;
        var set = WithFailed(failedKey);
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(set, listenerName, logger));
        Assert.False(ListenerEngineFeatures.CheckRegistration(set, listenerName, logger));

        var message = Assert.Single(logger.Messages);
        Assert.Equal($"{listenerName} listener registered on {feature} disabled ({failedKey}: fingerprint mismatch); " +
                     "it will never fire", message);
    }

    [Theory]
    [MemberData(nameof(ListenerFeatures))]
    public void ListenerIsSilentWhenOnlyAnotherClientsHookFailed(string listenerName, string feature)
    {
        foreach (var other in Features.Where(f => f.Name != feature))
        {
            var logger = new CapturingLogger();

            Assert.True(ListenerEngineFeatures.CheckRegistration(WithFailed(other.Bindings[0].Key), listenerName, logger));
            Assert.Empty(logger.Messages);
        }
    }

    [Fact]
    public void AFailedClientCommandDisablesOnlyClientCommands()
    {
        var set = WithFailed("IServerGameClients::ClientCommand");

        Assert.False(set.IsAvailable(EngineFeatureNames.PlayerClientCommands));
        Assert.True(set.IsAvailable(EngineFeatureNames.PlayerConnectionEvents));
        Assert.True(set.IsAvailable(EngineFeatureNames.PlayerVoiceEvents));
        Assert.True(set.IsAvailable(EngineFeatureNames.UiCustomHudLayout));
        Assert.Equal(
            "cssharp.player/client-commands disabled (IServerGameClients::ClientCommand: fingerprint mismatch)",
            set.GetStatus(EngineFeatureNames.PlayerClientCommands).ToString());
    }

    [Theory]
    [InlineData("IServerGameClients::ClientConnect")]
    [InlineData("IServerGameClients::ClientPutInServer")]
    [InlineData("IServerGameClients::ClientDisconnect")]
    public void AnyFailedConnectionHookDisablesConnectionEvents(string failedKey)
    {
        var set = WithFailed(failedKey);

        Assert.False(set.IsAvailable(EngineFeatureNames.PlayerConnectionEvents));
        Assert.True(set.IsAvailable(EngineFeatureNames.PlayerClientCommands));
    }

    [Fact]
    public void CommandListenersAreSilentWhileClientCommandsIsAvailable()
    {
        var logger = new CapturingLogger();

        Assert.True(ListenerEngineFeatures.CheckCommandListenerRegistration(WithFailed("IServerGameClients::ClientVoice"), logger));
        Assert.Empty(logger.Messages);
    }

    [Fact]
    public void CommandListenersWarnOnceOnDisabledClientCommands()
    {
        var set = WithFailed("IServerGameClients::ClientCommand");
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckCommandListenerRegistration(set, logger));
        Assert.False(ListenerEngineFeatures.CheckCommandListenerRegistration(set, logger));

        var message = Assert.Single(logger.Messages);
        Assert.Equal("Command listener on client-typed commands registered on cssharp.player/client-commands disabled " +
                     "(IServerGameClients::ClientCommand: fingerprint mismatch); it will never fire", message);
    }

    [Fact]
    public void CommandListenersWarnOnceOnDisabledConsoleDispatch()
    {
        var set = WithFailed("ICvar::DispatchConCommand");
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckCommandListenerRegistration(set, logger));
        Assert.False(ListenerEngineFeatures.CheckCommandListenerRegistration(set, logger));

        var message = Assert.Single(logger.Messages);
        Assert.Equal("Command listener on console-issued commands registered on cssharp.commands/console-dispatch disabled " +
                     "(ICvar::DispatchConCommand: fingerprint mismatch); it will never fire", message);
    }
}
