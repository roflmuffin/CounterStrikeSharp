using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// The ICvar Engine Features (console commands and ConVars, by verified slot index): a failed ICvar Slot Binding disables
// exactly the Engine Features using it; registrations on a disabled one are kept, warn once and never reach the engine.
[Collection(nameof(EngineFeaturesApiTests))]
public class ICvarEngineFeatureTests : IDisposable
{
    private const string BuildId = "1.42.0.1/2000950";

    // Engine Feature -> its Slot Bindings, as the native catalog declares them.
    private static readonly (string Name, string[] Keys)[] Features =
    [
        (EngineFeatureNames.CommandsConsoleDispatch, ["ICvar::DispatchConCommand"]),
        (EngineFeatureNames.CommandsAddCommand,
        [
            "ICvar::FindConCommand", "ICvar::RegisterConCommand", "ICvar::GetConCommandData", "ICvar::UnregisterConCommandCallbacks",
            "ICvar::GetCharacterSet"
        ]),
        (EngineFeatureNames.PlayerExecuteClientCommandFromServer,
            ["ICvar::GetCharacterSet", "ICvar::FindConCommand", "ICvar::DispatchConCommand"]),
        (EngineFeatureNames.ConvarsFind, ["ICvar::FindConVar", "ICvar::GetConVarData"]),
        (EngineFeatureNames.ConvarsRead, ["ICvar::GetConVarData"]),
        (EngineFeatureNames.ConvarsWrite,
        [
            "ICvar::GetConVarData", "ICvar::CallFilterCallback", "ICvar::CallChangeCallback", "ICvar::CallGlobalChangeCallbacks",
            "ICvar::QueueThreadSetValue"
        ]),
        (EngineFeatureNames.ConvarsCreate,
            ["ICvar::FindConVar", "ICvar::GetConVarData", "ICvar::RegisterConVar", "ICvar::UnregisterConVarCallbacks"]),
        (EngineFeatureNames.ConvarsChangeEvents,
            ["ICvar::InstallGlobalChangeCallback", "ICvar::RemoveGlobalChangeCallback", "ICvar::GetConVarData"]),
    ];

    private static IEnumerable<string> Keys => Features.SelectMany(f => f.Keys).Distinct();

    public void Dispose() => EngineFeatures.Reset();

    private sealed class CapturingLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        private readonly CapturingLogger _inner = new();
        public List<string> Messages => _inner.Messages;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => _inner.Log(logLevel, eventId, state, exception, formatter);
    }

    // Every ICvar Slot Binding resolved, except failingKey (fingerprint mismatch).
    private static EngineFeatureSet SetWith(string? failingKey = null)
    {
        var bindings = Keys.Select(key => key == failingKey
            ? new BindingResult(key, BindingKind.Slot, false, BindingFailureReason.FingerprintMismatch, 1, 0x1000, 0, "tier0",
                Slot: new SlotEvidence("CCvar", 20, "None", 46, 46))
            : new BindingResult(key, BindingKind.Slot, true, BindingFailureReason.None, 1, 0x1000, 20, "tier0",
                Slot: new SlotEvidence("CCvar", 20, "Window", 46, 46)));
        var declarations = Features.Select(f => new EngineFeatureDeclaration(f.Name, f.Keys.Select(BindingDeclaration.Any).ToList()));
        return new EngineFeatureSet(new ResolutionReport(BuildId, bindings, declarations), []);
    }

    public static IEnumerable<object[]> AllKeys() => Keys.Select(key => new object[] { key });

    [Theory]
    [MemberData(nameof(AllKeys))]
    public void AFailedICvarSlotBindingDisablesExactlyTheEngineFeaturesUsingIt(string failingKey)
    {
        var set = SetWith(failingKey);

        foreach (var (name, keys) in Features)
        {
            Assert.Equal(!keys.Contains(failingKey), set.IsAvailable(name));
        }
    }

    [Fact]
    public void OnConVarChangedOnADisabledChangeCallbackRegistersAndWarnsOnce()
    {
        var set = SetWith("ICvar::InstallGlobalChangeCallback");
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(set, "OnConVarChanged", logger));
        Assert.False(ListenerEngineFeatures.CheckRegistration(set, "OnConVarChanged", logger));

        Assert.Equal(
            "OnConVarChanged listener registered on cssharp.convars/change-events disabled " +
            "(ICvar::InstallGlobalChangeCallback: fingerprint mismatch); it will never fire",
            Assert.Single(logger.Messages));
        Assert.True(ListenerEngineFeatures.CheckRegistration(SetWith(), "OnConVarChanged", logger));
    }

    [Fact]
    public void ACommandOnADisabledAddCommandIsKeptAndWarnsOnceWithoutReachingTheEngine()
    {
        // NativeAPI is not available here: reaching the engine would throw.
        EngineFeatures.Current = SetWith("ICvar::RegisterConCommand");
        var logger = new CapturingLogger<CommandManager>();
        var manager = new CommandManager(logger);
        var first = new CommandDefinition("css_first", "first", (_, _) => { });
        var second = new CommandDefinition("css_second", "second", (_, _) => { });

        manager.RegisterCommand(first);
        manager.RegisterCommand(second);
        manager.RemoveCommand(first);

        Assert.Single(logger.Messages, message =>
            message == "Command registered on cssharp.commands/add-command disabled " +
            "(ICvar::RegisterConCommand: fingerprint mismatch); it will never fire");
    }
}
