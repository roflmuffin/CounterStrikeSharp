using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;

namespace CounterStrikeSharp.API.Tests;

// Entity IO, sound and spawn helpers check their own Engine Feature before calling into native code, so a disabled one
// throws the typed exception and the others keep working (ADR-0002). Installs a process-wide report, so shares the
// collection of the other tests that do.
[Collection(nameof(EngineFeaturesApiTests))]
public class EntityIoEngineFeatureTests : IDisposable
{
    private static readonly (string Name, string Key)[] Features =
    [
        ("cssharp.entity/output-hooks", "CEntityIOOutput_FireOutputInternal"),
        ("cssharp.entity/accept-input", "CEntityInstance_AcceptInput"),
        ("cssharp.entity/add-io-event", "CEntitySystem_AddEntityIOEvent"),
        ("cssharp.entity/emit-sound", "CBaseEntity_EmitSoundFilter"),
        ("cssharp.entity/dispatch-spawn", "CBaseEntity_DispatchSpawn"),
    ];

    // An entity that is not valid: helpers that pass their Engine Feature check then fail on the entity check, before
    // any native call.
    private static readonly CBaseEntity InvalidEntity = new(IntPtr.Zero);

    public void Dispose() => EngineFeatures.Reset();

    // Native declares these Engine Features; the report carries them with one Signature Binding each.
    private static void InstallReport(params string[] failedKeys)
    {
        var bindings = Features.Select(f => failedKeys.Contains(f.Key)
                ? new BindingResult(f.Key, BindingKind.Signature, false, BindingFailureReason.NotFound, 0, 0, 0, "server")
                : new BindingResult(f.Key, BindingKind.Signature, true, BindingFailureReason.None, 1, 0x1000, 0, "server"))
            .ToList();
        var nativeFeatures = Features.Select(f => new EngineFeatureDeclaration(f.Name, [BindingDeclaration.Signature(f.Key)])).ToList();
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport("1.42.0.1/2000950", bindings, nativeFeatures), []);
    }

    public static TheoryData<string, string, Action<CBaseEntity>> Helpers => new()
    {
        { "cssharp.entity/accept-input", "CEntityInstance_AcceptInput", e => e.AcceptInput("Break") },
        { "cssharp.entity/add-io-event", "CEntitySystem_AddEntityIOEvent", e => e.AddEntityIOEvent("Kill", delay: 0.1f) },
        { "cssharp.entity/emit-sound", "CBaseEntity_EmitSoundFilter", e => e.EmitSound("Weapon_AK47.Single") },
        { "cssharp.entity/dispatch-spawn", "CBaseEntity_DispatchSpawn", e => e.DispatchSpawn() },
        { "cssharp.entity/dispatch-spawn", "CBaseEntity_DispatchSpawn", e => e.DispatchSpawn(null) },
    };

    [Theory]
    [MemberData(nameof(Helpers))]
    public void CallingADisabledHelperThrowsTheTypedException(string feature, string key, Action<CBaseEntity> call)
    {
        InstallReport(key);

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => call(InvalidEntity));
        Assert.Equal(feature, exception.EngineFeature);
        Assert.Equal(key, exception.Binding);
        Assert.Equal(BindingFailureReason.NotFound, exception.Reason);
    }

    [Theory]
    [MemberData(nameof(Helpers))]
    public void ABrokenOutputHookBindingLeavesTheHelpersWorking(string feature, string key, Action<CBaseEntity> call)
    {
        InstallReport("CEntityIOOutput_FireOutputInternal");

        Assert.False(EngineFeatures.IsAvailable("cssharp.entity/output-hooks"));
        Assert.True(EngineFeatures.IsAvailable(feature));
        // Past the Engine Feature check: fails only because the entity is not valid.
        var exception = Assert.Throws<InvalidOperationException>(() => call(InvalidEntity));
        Assert.Equal("Entity is not valid", exception.Message);
    }
}
