#include "core/engine_features.h"

#include "core/slot_binding_keys.h"

#include <algorithm>
#include <string>
#include <unordered_map>

namespace counterstrikesharp::bindings {
namespace {

BindingDeclaration Signature(std::string key) { return { std::move(key), BindingKind::Signature }; }
BindingDeclaration Offset(std::string key) { return { std::move(key), BindingKind::Offset }; }
// A vtable slot we hook or call: its index, class and fingerprint window come from Gamedata (ADR-0003).
BindingDeclaration Slot(std::string_view key) { return { std::string(key), BindingKind::Slot }; }
// A game-class virtual called through its Gamedata index: a Slot Binding with no compiled header index to fall back on.
BindingDeclaration ClassSlot(std::string_view key) { return { std::string(key), BindingKind::Slot, {}, false }; }
BindingDeclaration RttiVtable(std::string className, std::string library)
{
    return { std::move(className), BindingKind::RttiVtable, std::move(library) };
}
// A schema field native core code reads directly; key "Class::field", looked up in the server schema scope.
BindingDeclaration SchemaField(const std::string& className, const std::string& fieldName)
{
    return { className + "::" + fieldName, BindingKind::SchemaField, "server" };
}

// The declaration table. One row per Engine Feature: its stable name and the Bindings (Gamedata keys) it needs.
// Additive only. A Binding may appear in several rows; it is resolved once.
const std::vector<EngineFeatureDeclaration> kEngineFeatures{
    { std::string(features::ChatSayHooks), { Signature("Host_Say") } },
    // Server lifecycle: ticks, timers and frame-scheduled tasks
    { std::string(features::GameFrame), { Slot(slots::ISource2Server_GameFrame) } },
    // The other ISource2Server hooks (server lifecycle listeners) and its direct calls
    { std::string(features::HibernationEvents), { Slot(slots::ISource2Server_ServerHibernationUpdate) } },
    { std::string(features::SteamApiEvents),
      { Slot(slots::ISource2Server_GameServerSteamAPIActivated), Slot(slots::ISource2Server_GameServerSteamAPIDeactivated) } },
    { std::string(features::HostnameChanged), { Slot(slots::ISource2Server_OnHostNameChanged) } },
    { std::string(features::FatalShutdown), { Slot(slots::ISource2Server_PreFatalShutdown) } },
    { std::string(features::UpdateWhenNotInGame), { Slot(slots::ISource2Server_UpdateWhenNotInGame) } },
    { std::string(features::PreWorldUpdate), { Slot(slots::ISource2Server_PreWorldUpdate) } },
    { std::string(features::EconItemSystem), { Slot(slots::ISource2Server_GetEconItemSystem) } },
    { std::string(features::IsPaused), { Slot(slots::ISource2Server_IsPaused) } },
    // Players and UI: client connections, client-typed commands, voice and custom HUD clicks
    { std::string(features::PlayerConnectionEvents),
      { Slot(slots::IServerGameClients_ClientConnect), Slot(slots::IServerGameClients_ClientPutInServer),
        Slot(slots::IServerGameClients_ClientDisconnect) } },
    { std::string(features::PlayerClientCommands), { Slot(slots::IServerGameClients_ClientCommand) } },
    { std::string(features::PlayerVoiceEvents), { Slot(slots::IServerGameClients_ClientVoice) } },
    // Custom HUD clicks, and dialog variable strings built with the game's vtable.
    { std::string(features::UiCustomHudLayout),
      { Slot(slots::IServerGameClients_ClientSvcUserMessage), RttiVtable("HUDPanelDialogVariableString_t", "server") } },
    // Events and networking
    // Plus every game event manager slot it hooks or calls, and the loop-mode hook that flushes event hooks registered
    // before the game loop exists.
    { std::string(features::GameEvents),
      { Signature("CGameEventManager_Init"), RttiVtable("CGameEventManager", "server"), Slot(slots::IGameEventManager2_LoadEventsFromFile),
        Slot(slots::IGameEventManager2_AddListener), Slot(slots::IGameEventManager2_FindListener),
        Slot(slots::IGameEventManager2_RemoveListener), Slot(slots::IGameEventManager2_CreateEvent),
        Slot(slots::IGameEventManager2_FireEvent), Slot(slots::IGameEventManager2_FreeEvent),
        Slot(slots::IEngineServiceMgr_RegisterLoopMode) } },
    { std::string(features::FireEventToClient), { Signature("LegacyGameEventListener") } },
    { std::string(features::StateChanged), { Signature("NetworkStateChanged"), ClassSlot(slots::SetStateChanged) } },
    { std::string(features::CheckTransmit), { ClassSlot(slots::CheckTransmit), Offset("CheckTransmitPlayerSlot") } },
    // Our game system (precache, pre/post entity think) and player think (button changes) via RunThink.
    { std::string(features::ServerGameSystemEvents),
      { Signature("IGameSystem_InitAllSystems_pFirst"), SchemaField("CCSPlayerController", "m_hPlayerPawn"),
        SchemaField("CBasePlayerController", "m_iConnected"), SchemaField("CBasePlayerPawn", "m_pMovementServices"),
        SchemaField("CPlayer_MovementServices", "m_nButtons"), SchemaField("CInButtonState", "m_pButtonStates") } },
    // Entity IO and entity helpers
    { std::string(features::EntityOutputHooks), { Signature("CEntityIOOutput_FireOutputInternal") } },
    { std::string(features::EntityAcceptInput), { Signature("CEntityInstance_AcceptInput") } },
    { std::string(features::EntityAddIoEvent), { Signature("CEntitySystem_AddEntityIOEvent") } },
    { std::string(features::EntityEmitSound), { Signature("CBaseEntity_EmitSoundFilter") } },
    { std::string(features::EntityDispatchSpawn), { Signature("CBaseEntity_DispatchSpawn") } },
    // Take damage
    { std::string(features::EntityTakeDamageHooks),
      { Signature("CBaseEntity_TakeDamageOld"), ClassSlot(slots::CBaseEntity_IsPlayerPawn) } },
    // Engine server (IVEngineServer2): each plugin-visible API on the slots it hooks or calls. Per-player voice overrides
    // (VoiceFlags, SetListenOverride) apply in the SetClientListening hook; OnClientAuthorized and AuthorizedSteamID come
    // from the auth check on every frame.
    { std::string(features::PlayerSetListening), { Slot(slots::IVEngineServer2_SetClientListening) } },
    { std::string(features::ServerExecuteCommand), { Slot(slots::IVEngineServer2_ServerCommand) } },
    { std::string(features::ServerIsMapValid), { Slot(slots::IVEngineServer2_IsMapValid) } },
    { std::string(features::ServerPrecacheModel), { Slot(slots::IVEngineServer2_PrecacheGeneric) } },
    { std::string(features::PlayerUserId), { Slot(slots::IVEngineServer2_GetPlayerUserId) } },
    { std::string(features::PlayerPrintToConsole), { Slot(slots::IVEngineServer2_ClientPrintf) } },
    { std::string(features::PlayerGetConVarValue), { Slot(slots::IVEngineServer2_GetClientConVarValue) } },
    { std::string(features::PlayerSetFakeClientConVar), { Slot(slots::IVEngineServer2_SetFakeClientConVarValue) } },
    { std::string(features::PlayerExecuteClientCommand), { Slot(slots::IVEngineServer2_ClientCommand) } },
    { std::string(features::PlayerDisconnect), { Slot(slots::IVEngineServer2_DisconnectClient) } },
    { std::string(features::PlayerAuthorization),
      { Slot(slots::IVEngineServer2_IsClientFullyAuthenticated), Slot(slots::IVEngineServer2_GetClientSteamID) } },
    // Console commands and ConVars: every ICvar method we call, directly or through the SDK's convar.cpp (see
    // core/verified_cvar.h). The dispatch hook fires AddCommand callbacks and command listeners for console commands.
    { std::string(features::ConsoleDispatch), { Slot(slots::ICvar_DispatchConCommand) } },
    // Registering a command, and tokenizing chat triggers into one.
    { std::string(features::AddCommand),
      { Slot(slots::ICvar_FindConCommand), Slot(slots::ICvar_RegisterConCommand), Slot(slots::ICvar_GetConCommandData),
        Slot(slots::ICvar_UnregisterConCommandCallbacks), Slot(slots::ICvar_GetCharacterSet) } },
    { std::string(features::ExecuteClientCommandFromServer),
      { Slot(slots::ICvar_GetCharacterSet), Slot(slots::ICvar_FindConCommand), Slot(slots::ICvar_DispatchConCommand) } },
    // ConVars: find by name; read (value, flags, type, name, help text, bounds); write (value, flags, bounds); create and
    // delete; the global change listener.
    { std::string(features::ConVarFind), { Slot(slots::ICvar_FindConVar), Slot(slots::ICvar_GetConVarData) } },
    { std::string(features::ConVarRead), { Slot(slots::ICvar_GetConVarData) } },
    { std::string(features::ConVarWrite),
      { Slot(slots::ICvar_GetConVarData), Slot(slots::ICvar_CallFilterCallback), Slot(slots::ICvar_CallChangeCallback),
        Slot(slots::ICvar_CallGlobalChangeCallbacks), Slot(slots::ICvar_QueueThreadSetValue) } },
    { std::string(features::ConVarCreate),
      { Slot(slots::ICvar_FindConVar), Slot(slots::ICvar_GetConVarData), Slot(slots::ICvar_RegisterConVar),
        Slot(slots::ICvar_UnregisterConVarCallbacks) } },
    { std::string(features::ConVarChangeEvents),
      { Slot(slots::ICvar_InstallGlobalChangeCallback), Slot(slots::ICvar_RemoveGlobalChangeCallback), Slot(slots::ICvar_GetConVarData) } },
    // Interfaces that never shifted a slot we use. Hooks follow the verified index; direct calls stay compiled, so their
    // Bindings fail when verified anywhere but the compiled index (core_bindings.cpp supplies it).
    // Map start: entity system, OnMapStart.
    { std::string(features::Startup), { Slot(slots::INetworkServerService_StartupServer) } },
    // Game loop start (flushes early game event hooks).
    { std::string(features::LoopMode), { Slot(slots::IEngineServiceMgr_RegisterLoopMode), Slot(slots::IEngineServiceMgr_FindService) } },
    // Tick count, current time, frame time, map name, max players.
    { std::string(features::GlobalVars),
      { Slot(slots::INetworkServerService_GetIGameServer), Slot(slots::INetworkGameServer_GetGlobals) } },
    // User message hooks and sends, client prints, replicated convars.
    { std::string(features::UserMessages),
      { Slot(slots::IGameEventSystem_PostEventAbstract), Slot(slots::IGameEventSystem_PostEventAbstract_Filter) } },
};

// Virtuals called through their Gamedata index on game-class objects (and CheckTransmit, hooked by index on the game
// entities' vtable): Slot Bindings, verified and healed like the others. Managed helpers, plugins reading them by key
// (GameData.GetOffset) and native code all use the index in the report. Declared as Slot Bindings even when Gamedata has
// no "vtable" for them (e.g. an auto-updated gamedata.json from before Slot Bindings): then they are unverified at
// their Gamedata index, as before. Struct field offsets are not here: they stay Offset Bindings.
const std::vector<BindingDeclaration> kGameClassSlots{
    ClassSlot(slots::CheckTransmit),
    ClassSlot("CCSPlayerController_ChangeTeam"),
    ClassSlot("CCSPlayerController_Respawn"),
    ClassSlot("CCSPlayer_ItemServices_GiveNamedItem"),
    ClassSlot("CCSPlayer_ItemServices_DropActivePlayerWeapon"),
    ClassSlot("CCSPlayer_ItemServices_RemoveWeapons"),
    ClassSlot("CGameSceneNode_GetSkeletonInstance"),
    ClassSlot("CCSGameRules_FindPickerEntity"),
    ClassSlot(slots::CBaseEntity_IsPlayerPawn),
    ClassSlot("CBaseEntity_Teleport"),
    ClassSlot("CBasePlayerPawn_CommitSuicide"),
    ClassSlot(slots::SetStateChanged),
};

// Interface methods only compiled calls use, in modules the boot does not fingerprint yet (network messages, schema
// system); no Engine Feature lists them, so they are declared only where Gamedata has them. Interface Slot Bindings
// like the others: on Windows unverified at their compiled header index, as before Slot Bindings, never failed for want
// of a Windows index in Gamedata.
const std::vector<BindingDeclaration> kCompiledInterfaceSlots{
    Slot(slots::INetworkMessages_FindNetworkMessagePartial),
    Slot(slots::INetworkMessages_FindNetworkMessageById),
    Slot(slots::CSchemaSystem_GlobalTypeScope),
    Slot(slots::CSchemaSystem_FindTypeScopeForModule),
};

bool Declared(const std::vector<BindingDeclaration>& bindings, std::string_view key)
{
    return std::any_of(bindings.begin(), bindings.end(), [&](const BindingDeclaration& b) {
        return b.key == key;
    });
}

} // namespace

std::span<const EngineFeatureDeclaration> NativeEngineFeatures() { return kEngineFeatures; }

std::vector<BindingDeclaration> CoreBindingDeclarations(const Gamedata& gamedata)
{
    std::vector<BindingDeclaration> bindings;
    for (const auto& feature : kEngineFeatures)
        for (const auto& binding : feature.bindings)
            if (!Declared(bindings, binding.key)) bindings.push_back(binding);
    for (const auto& binding : kGameClassSlots)
        if (!Declared(bindings, binding.key)) bindings.push_back(binding);

    for (const auto& [key, entry] : gamedata)
    {
        if (Declared(bindings, key)) continue;
        if (entry.slot)
        {
            const auto compiled = std::find_if(kCompiledInterfaceSlots.begin(), kCompiledInterfaceSlots.end(),
                                               [&](const BindingDeclaration& b) { return b.key == key; });
            // Otherwise only Gamedata declares it, so no native code calls it through a compiled method.
            bindings.push_back(compiled != kCompiledInterfaceSlots.end() ? *compiled : ClassSlot(key));
        }
        else if (entry.hasSignatures)
            bindings.push_back(Signature(key));
        else if (entry.hasOffsets)
            bindings.push_back(Offset(key));
    }
    return bindings;
}

const EngineFeatureDeclaration* FindNativeEngineFeature(std::string_view name)
{
    // Natives check their Engine Feature on every call: by name through an index, not a scan of the catalog.
    static const auto index = [] {
        std::unordered_map<std::string_view, const EngineFeatureDeclaration*> byName;
        for (const auto& feature : kEngineFeatures)
            byName.emplace(feature.name, &feature);
        return byName;
    }();
    const auto it = index.find(name);
    return it == index.end() ? nullptr : it->second;
}

} // namespace counterstrikesharp::bindings
