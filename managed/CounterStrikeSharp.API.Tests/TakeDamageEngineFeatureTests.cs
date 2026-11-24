using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests;

// Take-damage Engine Features (ticket 11): take-damage-hooks is native-owned (CBaseEntity_TakeDamageOld,
// CBaseEntity_IsPlayerPawn), take-damage-hit-group is managed (CTakeDamageInfo_HitGroup).
[Collection(nameof(EngineFeaturesApiTests))]
public class TakeDamageEngineFeatureTests : IDisposable
{
    private const string TakeDamageHooks = "cssharp.entity/take-damage-hooks";
    private const string TakeDamageHitGroup = "cssharp.entity/take-damage-hit-group";
    private const int HitGroupOffset = 72;

    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Entries.Add((logLevel, formatter(state, exception)));
    }

    private static BindingResult Resolved(string key, BindingKind kind, long offset = 0) =>
        new(key, kind, true, BindingFailureReason.None, kind == BindingKind.Signature ? 1 : 0, kind == BindingKind.Signature ? 0x1000 : 0,
            offset, "server");

    private static BindingResult Failed(string key, BindingKind kind, BindingFailureReason reason) =>
        new(key, kind, false, reason, 0, 0, 0, "server");

    // The native catalog row as it arrives with the report.
    private static readonly EngineFeatureDeclaration NativeTakeDamageHooks = new(TakeDamageHooks,
        [BindingDeclaration.Signature("CBaseEntity_TakeDamageOld"), BindingDeclaration.Offset("CBaseEntity_IsPlayerPawn")]);

    private static EngineFeatureSet Features(BindingResult takeDamageOld, BindingResult isPlayerPawn, BindingResult hitGroup) =>
        new(new ResolutionReport("1.42.0.1/2000950", [takeDamageOld, isPlayerPawn, hitGroup], [NativeTakeDamageHooks]),
            ManagedEngineFeatures.All);

    private static EngineFeatureSet AllResolved() => Features(
        Resolved("CBaseEntity_TakeDamageOld", BindingKind.Signature),
        Resolved("CBaseEntity_IsPlayerPawn", BindingKind.Offset, 160),
        Resolved("CTakeDamageInfo_HitGroup", BindingKind.Offset, HitGroupOffset));

    private static EngineFeatureSet TakeDamageOldNotFound() => Features(
        Failed("CBaseEntity_TakeDamageOld", BindingKind.Signature, BindingFailureReason.NotFound),
        Resolved("CBaseEntity_IsPlayerPawn", BindingKind.Offset, 160),
        Resolved("CTakeDamageInfo_HitGroup", BindingKind.Offset, HitGroupOffset));

    public void Dispose() => EngineFeatures.Reset();

    [Theory]
    [InlineData("OnEntityTakeDamagePre")]
    [InlineData("OnEntityTakeDamagePost")]
    [InlineData("OnPlayerTakeDamagePre")]
    [InlineData("OnPlayerTakeDamagePost")]
    public void TakeDamageListenersOnDisabledTakeDamageHooksRegisterButWarnOnce(string listener)
    {
        var features = TakeDamageOldNotFound();
        var logger = new CapturingLogger();

        Assert.False(ListenerEngineFeatures.CheckRegistration(features, listener, logger));
        Assert.False(ListenerEngineFeatures.CheckRegistration(features, listener, logger));

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal($"{listener} listener registered on {TakeDamageHooks} disabled (CBaseEntity_TakeDamageOld: not found); " +
                     "it will never fire", entry.Message);
    }

    [Fact]
    public void TakeDamageListenersRegisterSilentlyWhenTakeDamageHooksAreAvailable()
    {
        var logger = new CapturingLogger();

        Assert.True(ListenerEngineFeatures.CheckRegistration(AllResolved(), "OnPlayerTakeDamagePre", logger));
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void ListenersNotBackedByAnEngineFeatureAreNotGated()
    {
        var logger = new CapturingLogger();

        Assert.True(ListenerEngineFeatures.CheckRegistration(TakeDamageOldNotFound(), "OnTick", logger));
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void BreakingTheHitGroupOffsetDisablesOnlyTakeDamageHitGroup()
    {
        var features = Features(
            Resolved("CBaseEntity_TakeDamageOld", BindingKind.Signature),
            Resolved("CBaseEntity_IsPlayerPawn", BindingKind.Offset, 160),
            Failed("CTakeDamageInfo_HitGroup", BindingKind.Offset, BindingFailureReason.MissingForThisOs));

        Assert.True(features.IsAvailable(TakeDamageHooks));
        Assert.Equal("cssharp.entity/take-damage-hit-group disabled (CTakeDamageInfo_HitGroup: missing for this OS)",
            features.GetStatus(TakeDamageHitGroup).ToString());
    }

    [Fact]
    public void ReadingTheHitGroupWhileDisabledThrowsTheTypedException()
    {
        EngineFeatures.Current = Features(
            Resolved("CBaseEntity_TakeDamageOld", BindingKind.Signature),
            Resolved("CBaseEntity_IsPlayerPawn", BindingKind.Offset, 160),
            Failed("CTakeDamageInfo_HitGroup", BindingKind.Offset, BindingFailureReason.MissingFromGamedata));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => new CTakeDamageInfo(0x1234).GetHitGroup());
        Assert.Equal(TakeDamageHitGroup, exception.EngineFeature);
        Assert.Equal("CTakeDamageInfo_HitGroup", exception.Binding);
        Assert.Equal(BindingFailureReason.MissingFromGamedata, exception.Reason);
    }

    [Fact]
    public void TheHitGroupIsReadAtTheResolvedOffset()
    {
        EngineFeatures.Current = AllResolved();

        // CTakeDamageInfo -> (+offset) hit info -> (+16) hitbox data -> (+56) hit group.
        var info = Marshal.AllocHGlobal(128);
        var hitInfo = Marshal.AllocHGlobal(32);
        var hitbox = Marshal.AllocHGlobal(64);
        try
        {
            Marshal.WriteIntPtr(info, HitGroupOffset, hitInfo);
            Marshal.WriteIntPtr(hitInfo, 16, hitbox);
            Marshal.WriteInt32(hitbox, 56, (int)HitGroup_t.HITGROUP_HEAD);

            Assert.Equal(HitGroup_t.HITGROUP_HEAD, new CTakeDamageInfo(info).GetHitGroup());
        }
        finally
        {
            Marshal.FreeHGlobal(info);
            Marshal.FreeHGlobal(hitInfo);
            Marshal.FreeHGlobal(hitbox);
        }
    }

    [Fact]
    public void IsPlayerPawnKeepsWorkingWhenOnlyTheTakeDamageSignatureBroke()
    {
        Assert.Equal(160, CBaseEntity.IsPlayerPawnOffset(TakeDamageOldNotFound()));
    }

    [Fact]
    public void IsPlayerPawnThrowsTheTypedExceptionWhenItsOffsetIsMissing()
    {
        var features = Features(
            Resolved("CBaseEntity_TakeDamageOld", BindingKind.Signature),
            Failed("CBaseEntity_IsPlayerPawn", BindingKind.Offset, BindingFailureReason.MissingForThisOs),
            Resolved("CTakeDamageInfo_HitGroup", BindingKind.Offset, HitGroupOffset));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => CBaseEntity.IsPlayerPawnOffset(features));
        Assert.Equal(TakeDamageHooks, exception.EngineFeature);
        Assert.Equal("CBaseEntity_IsPlayerPawn", exception.Binding);
        Assert.Equal(BindingFailureReason.MissingForThisOs, exception.Reason);
    }
}
