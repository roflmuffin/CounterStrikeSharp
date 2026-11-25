using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Events;

namespace CounterStrikeSharp.API.Tests;

// Calling into a disabled events or network Engine Feature throws the typed exception before any native call (ADR-0002).
// The native-owned features arrive in the report as the native catalog declares them.
[Collection(nameof(EngineFeaturesApiTests))]
public class EventsAndNetworkEngineFeatureTests : IDisposable
{
    public EventsAndNetworkEngineFeatureTests()
    {
        var report = new ResolutionReport("1.42.0.1/2000950",
            [
                new BindingResult("CGameEventManager_Init", BindingKind.Signature, false, BindingFailureReason.NotFound, 0, 0, 0,
                    "server"),
                new BindingResult("CGameEventManager", BindingKind.RttiVtable, true, BindingFailureReason.None, 1, 0x5000, 0,
                    "server"),
                new BindingResult("LegacyGameEventListener", BindingKind.Signature, false, BindingFailureReason.Ambiguous, 2, 0, 0,
                    "server"),
                new BindingResult("NetworkStateChanged", BindingKind.Signature, true, BindingFailureReason.None, 1, 0x6000, 0,
                    "server"),
                new BindingResult("SetStateChanged", BindingKind.Slot, false, BindingFailureReason.MissingForThisOs, 0, 0, 0, ""),
            ],
            [
                new EngineFeatureDeclaration("cssharp.events/game-events",
                    [new BindingDeclaration("CGameEventManager_Init", BindingKind.Signature),
                     new BindingDeclaration("CGameEventManager", BindingKind.RttiVtable)]),
                new EngineFeatureDeclaration("cssharp.events/fire-event-to-client",
                    [BindingDeclaration.Signature("LegacyGameEventListener")]),
                new EngineFeatureDeclaration("cssharp.network/state-changed",
                    [BindingDeclaration.Signature("NetworkStateChanged"), BindingDeclaration.Slot("SetStateChanged")]),
            ]);
        EngineFeatures.Current = new EngineFeatureSet(report, []);
    }

    public void Dispose() => EngineFeatures.Reset();

    [Fact]
    public void CreatingAGameEventWhileGameEventsAreDisabledThrowsTheTypedException()
    {
        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => new EventPlayerDeath(false));

        Assert.Equal("cssharp.events/game-events", exception.EngineFeature);
        Assert.Equal("CGameEventManager_Init", exception.Binding);
        Assert.Equal(BindingFailureReason.NotFound, exception.Reason);
    }

    [Fact]
    public void FiringAnEventToAClientWhileThatIsDisabledThrowsTheTypedException()
    {
        var @event = new GameEvent(IntPtr.Zero);

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            @event.FireEventToClient(new CCSPlayerController(IntPtr.Zero)));

        Assert.Equal("cssharp.events/fire-event-to-client", exception.EngineFeature);
        Assert.Equal("Engine Feature cssharp.events/fire-event-to-client is disabled (LegacyGameEventListener: ambiguous (2 matches))",
            exception.Message);
    }

    [Fact]
    public void SettingStateChangedWhileThatIsDisabledThrowsTheTypedException()
    {
        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            Utilities.SetStateChanged(new CBaseEntity(IntPtr.Zero), "CBaseEntity", "m_iHealth"));

        Assert.Equal("cssharp.network/state-changed", exception.EngineFeature);
        Assert.Equal("SetStateChanged", exception.Binding);
        Assert.Equal(BindingFailureReason.MissingForThisOs, exception.Reason);
    }
}
