namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// The managed Engine Feature catalog: Engine Features whose Bindings managed helpers consume (ADR-0002). Native-owned
/// Engine Features are declared in <c>src/core/engine_features.cpp</c> and arrive with the Resolution Report.
/// <para>
/// To add one: add a field (name as a string literal, never renamed or reused; Gamedata keys verbatim) and list it in
/// <see cref="All"/>, then regenerate the <see cref="EngineFeature"/> enum with
/// <c>dotnet run --project tooling/CodeGen.Natives -- engine-features</c> (it reads the names from this file).
/// Core memory functions bind to a Signature Binding lazily with <c>CoreFunction.FromEngineFeature(feature, key)</c>
/// (see <c>VirtualFunctions</c>); other helpers take their addresses/offsets via
/// <c>EngineFeatures.Current.GetAddress(feature, key)</c> / <c>GetOffset(feature, key)</c>. All of them throw
/// <see cref="EngineFeatureUnavailableException"/> when the Engine Feature is disabled.
/// </para>
/// </summary>
internal static class ManagedEngineFeatures
{
    // ticket 06
    public static readonly EngineFeatureDeclaration ChatClientPrint = new("cssharp.chat/client-print",
        [BindingDeclaration.Signature("ClientPrint"), BindingDeclaration.Signature("UTIL_ClientPrintAll")]);

    // ticket 12
    public static readonly EngineFeatureDeclaration PlayerSetPawn = new("cssharp.player/set-pawn",
        [BindingDeclaration.Signature("CBasePlayerController_SetPawn")]);

    public static readonly EngineFeatureDeclaration PlayerSwitchTeam = new("cssharp.player/switch-team",
        [BindingDeclaration.Signature("CCSPlayerController_SwitchTeam")]);

    public static readonly EngineFeatureDeclaration PlayerChangeTeam = new("cssharp.player/change-team",
        [BindingDeclaration.Offset("CCSPlayerController_ChangeTeam")]);

    public static readonly EngineFeatureDeclaration PlayerRespawn = new("cssharp.player/respawn",
        [BindingDeclaration.Offset("CCSPlayerController_Respawn")]);

    public static readonly EngineFeatureDeclaration PlayerCommitSuicide = new("cssharp.player/commit-suicide",
        [BindingDeclaration.Offset("CBasePlayerPawn_CommitSuicide")]);

    public static readonly EngineFeatureDeclaration PlayerPostThinkHooks = new("cssharp.player/post-think-hooks",
        [BindingDeclaration.Signature("CCSPlayerPawnBase_PostThink")]);

    public static readonly EngineFeatureDeclaration GameRulesTerminateRound = new("cssharp.game-rules/terminate-round",
        [BindingDeclaration.Signature("CCSGameRules_TerminateRound")]);

    public static readonly EngineFeatureDeclaration GameRulesFindPickerEntity = new("cssharp.game-rules/find-picker-entity",
        [BindingDeclaration.Offset("CCSGameRules_FindPickerEntity")]);

    public static readonly EngineFeatureDeclaration EntityCreate = new("cssharp.entity/create",
        [BindingDeclaration.Signature("UTIL_CreateEntityByName")]);

    public static readonly EngineFeatureDeclaration EntityRemove = new("cssharp.entity/remove",
        [BindingDeclaration.Signature("UTIL_Remove")]);

    public static readonly EngineFeatureDeclaration EntitySetModel = new("cssharp.entity/set-model",
        [BindingDeclaration.Signature("CBaseModelEntity_SetModel")]);

    public static readonly EngineFeatureDeclaration EntityTeleport = new("cssharp.entity/teleport",
        [BindingDeclaration.Offset("CBaseEntity_Teleport")]);

    public static readonly EngineFeatureDeclaration EntityTriggerTouchHooks = new("cssharp.entity/trigger-touch-hooks",
        [BindingDeclaration.Signature("CBaseTrigger_StartTouch"), BindingDeclaration.Signature("CBaseTrigger_EndTouch")]);

    public static readonly EngineFeatureDeclaration EntitySkeletonInstance = new("cssharp.entity/skeleton-instance",
        [BindingDeclaration.Offset("CGameSceneNode_GetSkeletonInstance")]);

    public static readonly EngineFeatureDeclaration ItemsGiveNamedItem = new("cssharp.items/give-named-item",
        [BindingDeclaration.Signature("GiveNamedItem"), BindingDeclaration.Offset("CCSPlayer_ItemServices_GiveNamedItem")]);

    public static readonly EngineFeatureDeclaration ItemsCanAcquireHooks = new("cssharp.items/can-acquire-hooks",
        [BindingDeclaration.Signature("CCSPlayer_ItemServices_CanAcquire")]);

    public static readonly EngineFeatureDeclaration ItemsWeaponCanUseHooks = new("cssharp.items/weapon-can-use-hooks",
        [BindingDeclaration.Signature("CCSPlayer_WeaponServices_CanUse")]);

    public static readonly EngineFeatureDeclaration ItemsRemovePlayerItem = new("cssharp.items/remove-player-item",
        [BindingDeclaration.Signature("CBasePlayerPawn_RemovePlayerItem")]);

    public static readonly EngineFeatureDeclaration ItemsDropActiveWeapon = new("cssharp.items/drop-active-weapon",
        [BindingDeclaration.Offset("CCSPlayer_ItemServices_DropActivePlayerWeapon")]);

    public static readonly EngineFeatureDeclaration ItemsRemoveWeapons = new("cssharp.items/remove-weapons",
        [BindingDeclaration.Offset("CCSPlayer_ItemServices_RemoveWeapons")]);

    public static readonly EngineFeatureDeclaration ItemsWeaponData = new("cssharp.items/weapon-data",
        [BindingDeclaration.Signature("GetCSWeaponDataFromKey")]);

    public static readonly EngineFeatureDeclaration NavMesh = new("cssharp.nav/nav-mesh",
        [BindingDeclaration.Signature("CCSNavArea_IsValidNavMesh")]);

    // ticket 12: native-owned Engine Features (src/core/engine_features.cpp) whose Binding a core memory function in
    // VirtualFunctions also binds to. Same name as the native row, so the Bindings union by name.
    public static readonly EngineFeatureDeclaration EntityDispatchSpawn = new("cssharp.entity/dispatch-spawn",
        [BindingDeclaration.Signature("CBaseEntity_DispatchSpawn")]);

    public static readonly EngineFeatureDeclaration EntityTakeDamageHooks = new("cssharp.entity/take-damage-hooks",
        [BindingDeclaration.Signature("CBaseEntity_TakeDamageOld")]);

    /// <summary>Every managed Engine Feature. Keep after the fields above (static initialisation order).</summary>
    public static readonly IReadOnlyList<EngineFeatureDeclaration> All =
    [
        // ticket 06
        ChatClientPrint,
        // ticket 12
        PlayerSetPawn,
        PlayerSwitchTeam,
        PlayerChangeTeam,
        PlayerRespawn,
        PlayerCommitSuicide,
        PlayerPostThinkHooks,
        GameRulesTerminateRound,
        GameRulesFindPickerEntity,
        EntityCreate,
        EntityRemove,
        EntitySetModel,
        EntityTeleport,
        EntityTriggerTouchHooks,
        EntitySkeletonInstance,
        ItemsGiveNamedItem,
        ItemsCanAcquireHooks,
        ItemsWeaponCanUseHooks,
        ItemsRemovePlayerItem,
        ItemsDropActiveWeapon,
        ItemsRemoveWeapons,
        ItemsWeaponData,
        NavMesh,
        EntityDispatchSpawn,
        EntityTakeDamageHooks,
    ];
}
