using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Tests;

// The player, game rules, entity, items and nav helpers take their addresses and offsets from the Resolution Report, per
// Engine Feature (ADR-0002): a failed Binding disables only its own Engine Feature, and calling a helper of a disabled
// one throws the typed exception (never a call through vtable slot 0 or a missing Gamedata key). Installs a
// process-wide report, so shares the collection of the other tests that do.
[Collection(nameof(EngineFeaturesApiTests))]
public class ManagedHelperEngineFeatureTests : IDisposable
{
    private const string BuildId = "1.42.0.1/2000950";

    private readonly Func<nint, DataType, DataType[], IntPtr> _originalCreate = CoreFunction.CreateFunction;
    private readonly List<nint> _createdAt = new();

    public ManagedHelperEngineFeatureTests()
    {
        CoreFunction.CreateFunction = (address, _, _) =>
        {
            _createdAt.Add(address);
            return address + 0x10;
        };
    }

    public void Dispose()
    {
        EngineFeatures.Reset();
        CoreFunction.CreateFunction = _originalCreate;
    }

    // Every managed Binding resolves (signatures at 0x1000, offsets 7), except the given failures.
    private static void InstallReport(params (string Key, BindingFailureReason Reason)[] failures)
    {
        var bindings = ManagedEngineFeatures.All
            .SelectMany(feature => feature.Bindings)
            .DistinctBy(binding => binding.Key)
            .Select(binding =>
            {
                var kind = binding.Kind ?? BindingKind.Signature;
                var failure = failures.FirstOrDefault(f => f.Key == binding.Key);
                if (failure.Key != null) return new BindingResult(binding.Key, kind, false, failure.Reason, 0, 0, 0, "server");
                return kind == BindingKind.Signature
                    ? new BindingResult(binding.Key, kind, true, BindingFailureReason.None, 1, 0x1000, 0, "server")
                    : new BindingResult(binding.Key, kind, true, BindingFailureReason.None, 0, 0, 7, "server");
            })
            .ToList();
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport(BuildId, bindings, []), ManagedEngineFeatures.All);
    }

    // Not valid entities: a helper whose Engine Feature is available gets past its check and fails on the entity check,
    // before any native call.
    private static readonly CCSPlayerController InvalidController = new(IntPtr.Zero);
    private static readonly CCSPlayerPawn InvalidPawn = new(IntPtr.Zero);
    private static readonly CBaseEntity InvalidEntity = new(IntPtr.Zero);
    private static readonly CCSPlayer_ItemServices NullItemServices = new(IntPtr.Zero);
    private static readonly CGameSceneNode NullSceneNode = new(IntPtr.Zero);

    public static TheoryData<string, string, string> Declared => new()
    {
        { "cssharp.player/set-pawn", "CBasePlayerController_SetPawn", "Signature" },
        { "cssharp.player/switch-team", "CCSPlayerController_SwitchTeam", "Signature" },
        { "cssharp.player/change-team", "CCSPlayerController_ChangeTeam", "Slot" },
        { "cssharp.player/respawn", "CCSPlayerController_Respawn", "Slot" },
        { "cssharp.player/commit-suicide", "CBasePlayerPawn_CommitSuicide", "Slot" },
        { "cssharp.player/post-think-hooks", "CCSPlayerPawnBase_PostThink", "Signature" },
        { "cssharp.game-rules/terminate-round", "CCSGameRules_TerminateRound", "Signature" },
        { "cssharp.game-rules/find-picker-entity", "CCSGameRules_FindPickerEntity", "Slot" },
        { "cssharp.entity/create", "UTIL_CreateEntityByName", "Signature" },
        { "cssharp.entity/remove", "UTIL_Remove", "Signature" },
        { "cssharp.entity/set-model", "CBaseModelEntity_SetModel", "Signature" },
        { "cssharp.entity/teleport", "CBaseEntity_Teleport", "Slot" },
        { "cssharp.entity/trigger-touch-hooks", "CBaseTrigger_StartTouch", "Signature" },
        { "cssharp.entity/trigger-touch-hooks", "CBaseTrigger_EndTouch", "Signature" },
        { "cssharp.entity/skeleton-instance", "CGameSceneNode_GetSkeletonInstance", "Slot" },
        { "cssharp.items/give-named-item", "GiveNamedItem", "Signature" },
        { "cssharp.items/give-named-item", "CCSPlayer_ItemServices_GiveNamedItem", "Slot" },
        { "cssharp.items/can-acquire-hooks", "CCSPlayer_ItemServices_CanAcquire", "Signature" },
        { "cssharp.items/weapon-can-use-hooks", "CCSPlayer_WeaponServices_CanUse", "Signature" },
        { "cssharp.items/remove-player-item", "CBasePlayerPawn_RemovePlayerItem", "Signature" },
        { "cssharp.items/drop-active-weapon", "CCSPlayer_ItemServices_DropActivePlayerWeapon", "Slot" },
        { "cssharp.items/remove-weapons", "CCSPlayer_ItemServices_RemoveWeapons", "Slot" },
        { "cssharp.items/weapon-data", "GetCSWeaponDataFromKey", "Signature" },
        { "cssharp.nav/nav-mesh", "CCSNavArea_IsValidNavMesh", "Signature" },
    };

    [Theory]
    [MemberData(nameof(Declared))]
    public void EveryCatalogBindingIsDeclaredAndBreakingItDisablesItsEngineFeature(string feature, string key, string kind)
    {
        InstallReport((key, BindingFailureReason.NotFound));

        var status = EngineFeatures.GetStatus(feature);
        Assert.True(status.IsDeclared);
        Assert.False(status.IsAvailable);
        Assert.Equal(key, status.FailedBinding);
        Assert.Equal(Enum.Parse<BindingKind>(kind),
            ManagedEngineFeatures.All.Single(f => f.Name == feature).Bindings.Single(b => b.Key == key).Kind);
        Assert.Contains(feature, EngineFeatures.Current.FormatStartupReport());
    }

    [Fact]
    public void AHealedGameClassVirtualStaysAvailableAndItsHelperCallsTheHealedIndex()
    {
        // Gamedata states CommitSuicide at 386; the boot Vtable Check found its window once, at 387.
        InstallReport();
        var bindings = EngineFeatures.Current.Report.Bindings
            .Select(b => b.Key != "CBasePlayerPawn_CommitSuicide"
                ? b
                : b with { Offset = 387, Healed = true, Slot = new SlotEvidence("CBasePlayerPawn", 386, "Heal", 401, 401) })
            .ToList();
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport(BuildId, bindings, []), ManagedEngineFeatures.All);

        var status = EngineFeatures.GetStatus("cssharp.player/commit-suicide");
        Assert.True(status.IsAvailable);
        var heal = Assert.Single(status.Heals);
        Assert.Equal((386, 387), (heal.OldIndex, heal.NewIndex));
        Assert.Equal(387, EngineFeatures.Current.GetOffset(ManagedEngineFeatures.PlayerCommitSuicide, "CBasePlayerPawn_CommitSuicide"));
        Assert.Contains("Healed CBasePlayerPawn_CommitSuicide: CBasePlayerPawn (server) slot 386 -> 387",
            EngineFeatures.Current.FormatStartupReport());
    }

    [Fact]
    public void BreakingOneBindingDisablesOnlyItsOwnEngineFeature()
    {
        InstallReport(("CBasePlayerPawn_CommitSuicide", BindingFailureReason.MissingFromGamedata));

        var disabled = EngineFeatures.GetAll().Where(s => !s.IsAvailable).Select(s => s.Name);
        Assert.Equal(["cssharp.player/commit-suicide"], disabled);
    }

    // player

    [Fact]
    public void CommitSuicideWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CBasePlayerPawn_CommitSuicide", BindingFailureReason.MissingFromGamedata));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => InvalidPawn.CommitSuicide(false, true));
        Assert.Equal("cssharp.player/commit-suicide", exception.EngineFeature);
        Assert.Equal("CBasePlayerPawn_CommitSuicide", exception.Binding);
        Assert.Equal(BindingFailureReason.MissingFromGamedata, exception.Reason);
    }

    [Fact]
    public void RespawnAndTeleportStillWorkWhenCommitSuicideIsDisabled()
    {
        InstallReport(("CBasePlayerPawn_CommitSuicide", BindingFailureReason.MissingFromGamedata));

        Assert.Equal("Entity is not valid", Assert.Throws<InvalidOperationException>(() => InvalidController.Respawn()).Message);
        Assert.Equal("Entity is not valid",
            Assert.Throws<InvalidOperationException>(() => InvalidEntity.Teleport(new System.Numerics.Vector3(0, 0, 0))).Message);
    }

    [Fact]
    public void AnOffsetMissingForThisOsDisablesItsHelperInsteadOfCallingSlotZero()
    {
        InstallReport(("CCSPlayerController_ChangeTeam", BindingFailureReason.MissingForThisOs));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => InvalidController.ChangeTeam(CsTeam.Terrorist));
        Assert.Equal("cssharp.player/change-team", exception.EngineFeature);
        Assert.Equal(BindingFailureReason.MissingForThisOs, exception.Reason);
    }

    [Fact]
    public void RespawnWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSPlayerController_Respawn", BindingFailureReason.NotFound));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => InvalidController.Respawn());
        Assert.Equal("cssharp.player/respawn", exception.EngineFeature);
    }

    [Fact]
    public void SwitchTeamWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSPlayerController_SwitchTeam", BindingFailureReason.Ambiguous));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => VirtualFunctions.SwitchTeam(IntPtr.Zero, 2));
        Assert.Equal("cssharp.player/switch-team", exception.EngineFeature);
        Assert.Equal("CCSPlayerController_SwitchTeam", exception.Binding);
        Assert.Empty(_createdAt);
    }

    [Fact]
    public void SetPawnBindsToTheAddressInTheReport()
    {
        InstallReport(("CCSPlayerController_SwitchTeam", BindingFailureReason.NotFound));
        var setPawn = new MemoryFunctionVoid<CBasePlayerController, CBasePlayerPawn, bool, bool, bool, bool>(
            CoreFunction.FromEngineFeature(ManagedEngineFeatures.PlayerSetPawn, "CBasePlayerController_SetPawn"));

        Assert.Equal((IntPtr)0x1010, setPawn.Handle);
        Assert.Equal([(nint)0x1000], _createdAt);
    }

    // game rules

    [Fact]
    public void TerminateRoundWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSGameRules_TerminateRound", BindingFailureReason.NotFound));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            VirtualFunctions.TerminateRoundLinux(IntPtr.Zero, RoundEndReason.RoundDraw, 1f, 0, 0));
        Assert.Equal("cssharp.game-rules/terminate-round", exception.EngineFeature);
    }

    [Fact]
    public void FindPickerEntityWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSGameRules_FindPickerEntity", BindingFailureReason.MissingForThisOs));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            new CCSGameRules(IntPtr.Zero).FindPickerEntityInternal(InvalidController));
        Assert.Equal("cssharp.game-rules/find-picker-entity", exception.EngineFeature);
    }

    // entity

    [Fact]
    public void RemoveWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("UTIL_Remove", BindingFailureReason.NotFound));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => VirtualFunctions.UTIL_Remove(IntPtr.Zero));
        Assert.Equal("cssharp.entity/remove", exception.EngineFeature);
    }

    [Fact]
    public void TeleportWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CBaseEntity_Teleport", BindingFailureReason.MissingFromGamedata));

        var exception =
            Assert.Throws<EngineFeatureUnavailableException>(() => InvalidEntity.Teleport(new System.Numerics.Vector3(0, 0, 0)));
        Assert.Equal("cssharp.entity/teleport", exception.EngineFeature);
    }

    [Fact]
    public void SkeletonInstanceWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CGameSceneNode_GetSkeletonInstance", BindingFailureReason.MissingForThisOs));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => NullSceneNode.GetSkeletonInstance());
        Assert.Equal("cssharp.entity/skeleton-instance", exception.EngineFeature);
    }

    [Fact]
    public void BothTriggerTouchHelpersAreDisabledWhenOneOfTheirBindingsFails()
    {
        InstallReport(("CBaseTrigger_EndTouch", BindingFailureReason.NotFound));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => VirtualFunctions.CBaseTrigger_StartTouchFunc.Handle);
        Assert.Equal("cssharp.entity/trigger-touch-hooks", exception.EngineFeature);
        Assert.Equal("CBaseTrigger_EndTouch", exception.Binding);
    }

    // items

    [Fact]
    public void GiveNamedItemSignatureWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSPlayer_ItemServices_GiveNamedItem", BindingFailureReason.MissingForThisOs));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            VirtualFunctions.GiveNamedItem(IntPtr.Zero, "weapon_ak47", 0, 0, 0, 0));
        Assert.Equal("cssharp.items/give-named-item", exception.EngineFeature);
        Assert.Equal("CCSPlayer_ItemServices_GiveNamedItem", exception.Binding);
    }

    [Fact]
    public void GiveNamedItemOffsetWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("GiveNamedItem", BindingFailureReason.Ambiguous));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            NullItemServices.GiveNamedItem<CBasePlayerWeapon>("weapon_ak47"));
        Assert.Equal("cssharp.items/give-named-item", exception.EngineFeature);
        Assert.Equal("GiveNamedItem", exception.Binding);
    }

    [Fact]
    public void RemoveWeaponsWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSPlayer_ItemServices_RemoveWeapons", BindingFailureReason.MissingFromGamedata));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => NullItemServices.RemoveWeapons());
        Assert.Equal("cssharp.items/remove-weapons", exception.EngineFeature);
    }

    [Fact]
    public void DropActivePlayerWeaponWhileDisabledThrowsTheTypedException()
    {
        InstallReport(("CCSPlayer_ItemServices_DropActivePlayerWeapon", BindingFailureReason.MissingFromGamedata));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            NullItemServices.DropActivePlayerWeapon(new CBasePlayerWeapon(IntPtr.Zero)));
        Assert.Equal("cssharp.items/drop-active-weapon", exception.EngineFeature);
    }

    [Fact]
    public void RemoveWeaponsStillWorksWhenRemovePlayerItemIsDisabled()
    {
        InstallReport(("CBasePlayerPawn_RemovePlayerItem", BindingFailureReason.NotFound));

        Assert.Throws<EngineFeatureUnavailableException>(() => VirtualFunctions.RemovePlayerItemVirtual(IntPtr.Zero, IntPtr.Zero));
        Assert.Equal("ItemServices points to null.",
            Assert.Throws<InvalidOperationException>(() => NullItemServices.RemoveWeapons()).Message);
    }

    // nav

    [Fact]
    public void NavAreasWhileNavMeshIsDisabledThrowTheTypedException()
    {
        InstallReport(("CCSNavArea_IsValidNavMesh", BindingFailureReason.NotFound));

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => CCSNavArea.GetAllNavAreas());
        Assert.Equal("cssharp.nav/nav-mesh", exception.EngineFeature);
        Assert.Equal("CCSNavArea_IsValidNavMesh", exception.Binding);
    }

    // Engine Features owned by native whose Binding a core memory function also uses.

    [Fact]
    public void DispatchSpawnFunctionFollowsTheNativeOwnedEngineFeature()
    {
        var nativeDispatchSpawn = new EngineFeatureDeclaration("cssharp.entity/dispatch-spawn",
            [BindingDeclaration.Any("CBaseEntity_DispatchSpawn")]);
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport(BuildId,
            [
                new BindingResult("CBaseEntity_DispatchSpawn", BindingKind.Signature, false, BindingFailureReason.NotFound, 0, 0, 0,
                    "server")
            ],
            [nativeDispatchSpawn]), ManagedEngineFeatures.All);

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            VirtualFunctions.CBaseEntity_DispatchSpawn(IntPtr.Zero, IntPtr.Zero));
        Assert.Equal("cssharp.entity/dispatch-spawn", exception.EngineFeature);
        Assert.Equal("CBaseEntity_DispatchSpawn", exception.Binding);
    }
}
