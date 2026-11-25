using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;

namespace CounterStrikeSharp.API.Tests;

// Contract: every Engine Feature of the spec's catalog is declared, and managed helpers take every core
// Binding from the Resolution Report, never from Gamedata.
[Collection(nameof(EngineFeaturesApiTests))]
public class EngineFeatureCatalogTests : IDisposable
{
    // The initial Engine Feature catalog: name -> Gamedata keys.
    // "CGameEventManager" is the RTTI vtable Binding.
    private static readonly Dictionary<string, string[]> SpecCatalog = new()
    {
        ["cssharp.server/game-frame"] = ["ISource2Server::GameFrame"],
        ["cssharp.server/hibernation-events"] = ["ISource2Server::ServerHibernationUpdate"],
        ["cssharp.server/steam-api-events"] = ["ISource2Server::GameServerSteamAPIActivated", "ISource2Server::GameServerSteamAPIDeactivated"],
        ["cssharp.server/hostname-changed"] = ["ISource2Server::OnHostNameChanged"],
        ["cssharp.server/fatal-shutdown"] = ["ISource2Server::PreFatalShutdown"],
        ["cssharp.server/update-when-not-in-game"] = ["ISource2Server::UpdateWhenNotInGame"],
        ["cssharp.server/pre-world-update"] = ["ISource2Server::PreWorldUpdate"],
        ["cssharp.server/econ-item-system"] = ["ISource2Server::GetEconItemSystem"],
        ["cssharp.server/is-paused"] = ["ISource2Server::IsPaused"],
        ["cssharp.commands/console-dispatch"] = ["ICvar::DispatchConCommand"],
        ["cssharp.commands/add-command"] =
        [
            "ICvar::FindConCommand", "ICvar::RegisterConCommand", "ICvar::GetConCommandData", "ICvar::UnregisterConCommandCallbacks",
            "ICvar::GetCharacterSet"
        ],
        ["cssharp.player/execute-client-command-from-server"] =
            ["ICvar::GetCharacterSet", "ICvar::FindConCommand", "ICvar::DispatchConCommand"],
        ["cssharp.convars/find"] = ["ICvar::FindConVar", "ICvar::GetConVarData"],
        ["cssharp.convars/read"] = ["ICvar::GetConVarData"],
        ["cssharp.convars/write"] =
        [
            "ICvar::GetConVarData", "ICvar::CallFilterCallback", "ICvar::CallChangeCallback", "ICvar::CallGlobalChangeCallbacks",
            "ICvar::QueueThreadSetValue"
        ],
        ["cssharp.convars/create"] =
            ["ICvar::FindConVar", "ICvar::GetConVarData", "ICvar::RegisterConVar", "ICvar::UnregisterConVarCallbacks"],
        ["cssharp.convars/change-events"] =
            ["ICvar::InstallGlobalChangeCallback", "ICvar::RemoveGlobalChangeCallback", "ICvar::GetConVarData"],
        ["cssharp.player/connection-events"] =
            ["IServerGameClients::ClientConnect", "IServerGameClients::ClientPutInServer", "IServerGameClients::ClientDisconnect"],
        ["cssharp.player/client-commands"] = ["IServerGameClients::ClientCommand"],
        ["cssharp.player/voice-events"] = ["IServerGameClients::ClientVoice"],
        ["cssharp.ui/custom-hud-layout"] = ["IServerGameClients::ClientSvcUserMessage"],
        ["cssharp.events/game-events"] = ["CGameEventManager_Init", "CGameEventManager"],
        ["cssharp.events/fire-event-to-client"] = ["LegacyGameEventListener"],
        ["cssharp.network/state-changed"] = ["NetworkStateChanged", "SetStateChanged"],
        ["cssharp.network/check-transmit"] = ["ISource2GameEntities::CheckTransmit", "CheckTransmitPlayerSlot"],
        ["cssharp.server/game-system-events"] = ["IGameSystem_InitAllSystems_pFirst"],
        ["cssharp.chat/say-hooks"] = ["Host_Say"],
        ["cssharp.chat/client-print"] = ["ClientPrint", "UTIL_ClientPrintAll"],
        ["cssharp.entity/output-hooks"] = ["CEntityIOOutput_FireOutputInternal"],
        ["cssharp.entity/accept-input"] = ["CEntityInstance_AcceptInput"],
        ["cssharp.entity/add-io-event"] = ["CEntitySystem_AddEntityIOEvent"],
        ["cssharp.entity/emit-sound"] = ["CBaseEntity_EmitSoundFilter"],
        ["cssharp.entity/dispatch-spawn"] = ["CBaseEntity_DispatchSpawn"],
        ["cssharp.entity/create"] = ["UTIL_CreateEntityByName"],
        ["cssharp.entity/remove"] = ["UTIL_Remove"],
        ["cssharp.entity/set-model"] = ["CBaseModelEntity_SetModel"],
        ["cssharp.entity/teleport"] = ["CBaseEntity_Teleport"],
        ["cssharp.entity/take-damage-hooks"] = ["CBaseEntity_TakeDamageOld", "CBaseEntity_IsPlayerPawn"],
        ["cssharp.entity/take-damage-hit-group"] = ["CTakeDamageInfo_HitGroup"],
        ["cssharp.entity/trigger-touch-hooks"] = ["CBaseTrigger_StartTouch", "CBaseTrigger_EndTouch"],
        ["cssharp.entity/skeleton-instance"] = ["CGameSceneNode_GetSkeletonInstance"],
        ["cssharp.game-rules/terminate-round"] = ["CCSGameRules_TerminateRound"],
        ["cssharp.game-rules/find-picker-entity"] = ["CCSGameRules_FindPickerEntity"],
        ["cssharp.player/set-pawn"] = ["CBasePlayerController_SetPawn"],
        ["cssharp.player/switch-team"] = ["CCSPlayerController_SwitchTeam"],
        ["cssharp.player/change-team"] = ["CCSPlayerController_ChangeTeam"],
        ["cssharp.player/respawn"] = ["CCSPlayerController_Respawn"],
        ["cssharp.player/commit-suicide"] = ["CBasePlayerPawn_CommitSuicide"],
        ["cssharp.player/post-think-hooks"] = ["CCSPlayerPawnBase_PostThink"],
        ["cssharp.items/give-named-item"] = ["GiveNamedItem", "CCSPlayer_ItemServices_GiveNamedItem"],
        ["cssharp.items/can-acquire-hooks"] = ["CCSPlayer_ItemServices_CanAcquire"],
        ["cssharp.items/weapon-can-use-hooks"] = ["CCSPlayer_WeaponServices_CanUse"],
        ["cssharp.items/remove-player-item"] = ["CBasePlayerPawn_RemovePlayerItem"],
        ["cssharp.items/drop-active-weapon"] = ["CCSPlayer_ItemServices_DropActivePlayerWeapon"],
        ["cssharp.items/remove-weapons"] = ["CCSPlayer_ItemServices_RemoveWeapons"],
        ["cssharp.items/weapon-data"] = ["GetCSWeaponDataFromKey"],
        ["cssharp.nav/nav-mesh"] = ["CCSNavArea_IsValidNavMesh"],
    };

    public void Dispose() => EngineFeatures.Reset();

    [Fact]
    public void EveryEngineFeatureOfTheSpecCatalogHasAName()
    {
        // The enum is generated from the names declared natively and in managed code.
        var declared = Enum.GetValues<EngineFeature>().Select(f => f.GetName()).Order(StringComparer.Ordinal);

        Assert.Equal(SpecCatalog.Keys.Order(StringComparer.Ordinal), declared);
    }

    [Fact]
    public void ManagedEngineFeaturesDeclareOnlyTheirSpecBindings()
    {
        foreach (var feature in ManagedEngineFeatures.All)
        {
            Assert.True(SpecCatalog.TryGetValue(feature.Name, out var keys), $"{feature.Name} is not in the spec catalog");
            Assert.All(feature.Bindings, binding => Assert.Contains(binding.Key, keys));
        }
    }

    [Fact]
    public void TheCheckTransmitPlayerSlotOffsetComesFromTheReport()
    {
        EngineFeatures.Current = new EngineFeatureSet(CheckTransmitReport(slotResolved: true), ManagedEngineFeatures.All);

        Assert.Equal(624, CCheckTransmitInfoList.PlayerSlotOffset(EngineFeatures.Current));
    }

    [Fact]
    public void TheCheckTransmitPlayerSlotOffsetOfADisabledCheckTransmitThrowsTheTypedException()
    {
        EngineFeatures.Current = new EngineFeatureSet(CheckTransmitReport(slotResolved: false), ManagedEngineFeatures.All);

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() =>
            CCheckTransmitInfoList.PlayerSlotOffset(EngineFeatures.Current));

        Assert.Equal("cssharp.network/check-transmit", exception.EngineFeature);
        Assert.Equal("CheckTransmitPlayerSlot", exception.Binding);
        Assert.Equal(BindingFailureReason.MissingForThisOs, exception.Reason);
    }

    private static ResolutionReport CheckTransmitReport(bool slotResolved) =>
        new("1.42.0.1/2000950",
            [
                new BindingResult("ISource2GameEntities::CheckTransmit", BindingKind.Offset, true, BindingFailureReason.None, 0, 0, 13,
                    ""),
                slotResolved
                    ? new BindingResult("CheckTransmitPlayerSlot", BindingKind.Offset, true, BindingFailureReason.None, 0, 0, 624, "")
                    : new BindingResult("CheckTransmitPlayerSlot", BindingKind.Offset, false, BindingFailureReason.MissingForThisOs, 0,
                        0, 0, ""),
            ],
            [
                new EngineFeatureDeclaration("cssharp.network/check-transmit",
                    [
                        BindingDeclaration.Offset("ISource2GameEntities::CheckTransmit"),
                        BindingDeclaration.Offset("CheckTransmitPlayerSlot")
                    ]),
            ]);
}
