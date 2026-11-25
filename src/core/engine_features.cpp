#include "core/engine_features.h"

#include <algorithm>
#include <string>

namespace counterstrikesharp::bindings {
namespace {

BindingDeclaration Signature(std::string key) { return { std::move(key), BindingKind::Signature }; }
BindingDeclaration Offset(std::string key) { return { std::move(key), BindingKind::Offset }; }
// A vtable slot we hook or call: its index, class and fingerprint window come from Gamedata (ADR-0003).
BindingDeclaration Slot(std::string key) { return { std::move(key), BindingKind::Slot }; }
// A game-class virtual called through its Gamedata index: a Slot Binding with no compiled header index to fall back on.
BindingDeclaration ClassSlot(std::string key) { return { std::move(key), BindingKind::Slot, {}, false }; }
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
    { std::string(features::GameFrame), { Slot("ISource2Server::GameFrame") } },
    // The other ISource2Server hooks (server lifecycle listeners) and its direct calls
    { std::string(features::HibernationEvents), { Slot("ISource2Server::ServerHibernationUpdate") } },
    { std::string(features::SteamApiEvents),
      { Slot("ISource2Server::GameServerSteamAPIActivated"), Slot("ISource2Server::GameServerSteamAPIDeactivated") } },
    { std::string(features::HostnameChanged), { Slot("ISource2Server::OnHostNameChanged") } },
    { std::string(features::FatalShutdown), { Slot("ISource2Server::PreFatalShutdown") } },
    { std::string(features::UpdateWhenNotInGame), { Slot("ISource2Server::UpdateWhenNotInGame") } },
    { std::string(features::PreWorldUpdate), { Slot("ISource2Server::PreWorldUpdate") } },
    { std::string(features::EconItemSystem), { Slot("ISource2Server::GetEconItemSystem") } },
    { std::string(features::IsPaused), { Slot("ISource2Server::IsPaused") } },
    // Players and UI: client connections, client-typed commands, voice and custom HUD clicks
    { std::string(features::PlayerConnectionEvents),
      { Slot("IServerGameClients::ClientConnect"), Slot("IServerGameClients::ClientPutInServer"),
        Slot("IServerGameClients::ClientDisconnect") } },
    { std::string(features::PlayerClientCommands), { Slot("IServerGameClients::ClientCommand") } },
    { std::string(features::PlayerVoiceEvents), { Slot("IServerGameClients::ClientVoice") } },
    { std::string(features::UiCustomHudLayout), { Slot("IServerGameClients::ClientSvcUserMessage") } },
    // Events and networking
    // Plus every game event manager slot it hooks or calls, and the loop-mode hook that flushes event hooks registered
    // before the game loop exists.
    { std::string(features::GameEvents),
      { Signature("CGameEventManager_Init"), RttiVtable("CGameEventManager", "server"), Slot("IGameEventManager2::LoadEventsFromFile"),
        Slot("IGameEventManager2::AddListener"), Slot("IGameEventManager2::FindListener"), Slot("IGameEventManager2::RemoveListener"),
        Slot("IGameEventManager2::CreateEvent"), Slot("IGameEventManager2::FireEvent"), Slot("IGameEventManager2::FreeEvent"),
        Slot("IEngineServiceMgr::RegisterLoopMode") } },
    { std::string(features::FireEventToClient), { Signature("LegacyGameEventListener") } },
    { std::string(features::StateChanged), { Signature("NetworkStateChanged"), Offset("SetStateChanged") } },
    { std::string(features::CheckTransmit), { ClassSlot("ISource2GameEntities::CheckTransmit"), Offset("CheckTransmitPlayerSlot") } },
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
    { std::string(features::EntityTakeDamageHooks), { Signature("CBaseEntity_TakeDamageOld"), ClassSlot("CBaseEntity_IsPlayerPawn") } },
    // Engine server (IVEngineServer2): each plugin-visible API on the slots it hooks or calls. Per-player voice overrides
    // (VoiceFlags, SetListenOverride) apply in the SetClientListening hook; OnClientAuthorized and AuthorizedSteamID come
    // from the auth check on every frame.
    { std::string(features::PlayerSetListening), { Slot("IVEngineServer2::SetClientListening") } },
    { std::string(features::ServerExecuteCommand), { Slot("IVEngineServer2::ServerCommand") } },
    { std::string(features::ServerIsMapValid), { Slot("IVEngineServer2::IsMapValid") } },
    { std::string(features::ServerPrecacheModel), { Slot("IVEngineServer2::PrecacheGeneric") } },
    { std::string(features::PlayerUserId), { Slot("IVEngineServer2::GetPlayerUserId") } },
    { std::string(features::PlayerPrintToConsole), { Slot("IVEngineServer2::ClientPrintf") } },
    { std::string(features::PlayerGetConVarValue), { Slot("IVEngineServer2::GetClientConVarValue") } },
    { std::string(features::PlayerSetFakeClientConVar), { Slot("IVEngineServer2::SetFakeClientConVarValue") } },
    { std::string(features::PlayerExecuteClientCommand), { Slot("IVEngineServer2::ClientCommand") } },
    { std::string(features::PlayerDisconnect), { Slot("IVEngineServer2::DisconnectClient") } },
    { std::string(features::PlayerAuthorization),
      { Slot("IVEngineServer2::IsClientFullyAuthenticated"), Slot("IVEngineServer2::GetClientSteamID") } },
    // Console commands and ConVars: every ICvar method we call, directly or through the SDK's convar.cpp (see
    // core/verified_cvar.h). The dispatch hook fires AddCommand callbacks and command listeners for console commands.
    { std::string(features::ConsoleDispatch), { Slot("ICvar::DispatchConCommand") } },
    // Registering a command, and tokenizing chat triggers into one.
    { std::string(features::AddCommand),
      { Slot("ICvar::FindConCommand"), Slot("ICvar::RegisterConCommand"), Slot("ICvar::GetConCommandData"),
        Slot("ICvar::UnregisterConCommandCallbacks"), Slot("ICvar::GetCharacterSet") } },
    { std::string(features::ExecuteClientCommandFromServer),
      { Slot("ICvar::GetCharacterSet"), Slot("ICvar::FindConCommand"), Slot("ICvar::DispatchConCommand") } },
    // ConVars: find by name; read (value, flags, type, name, help text, bounds); write (value, flags, bounds); create and
    // delete; the global change listener.
    { std::string(features::ConVarFind), { Slot("ICvar::FindConVar"), Slot("ICvar::GetConVarData") } },
    { std::string(features::ConVarRead), { Slot("ICvar::GetConVarData") } },
    { std::string(features::ConVarWrite),
      { Slot("ICvar::GetConVarData"), Slot("ICvar::CallFilterCallback"), Slot("ICvar::CallChangeCallback"),
        Slot("ICvar::CallGlobalChangeCallbacks"), Slot("ICvar::QueueThreadSetValue") } },
    { std::string(features::ConVarCreate),
      { Slot("ICvar::FindConVar"), Slot("ICvar::GetConVarData"), Slot("ICvar::RegisterConVar"), Slot("ICvar::UnregisterConVarCallbacks") } },
    { std::string(features::ConVarChangeEvents),
      { Slot("ICvar::InstallGlobalChangeCallback"), Slot("ICvar::RemoveGlobalChangeCallback"), Slot("ICvar::GetConVarData") } },
    // Interfaces that never shifted a slot we use. Hooks follow the verified index; direct calls stay compiled, so their
    // Bindings fail when verified anywhere but the compiled index (core_bindings.cpp supplies it).
    // Map start: entity system, OnMapStart.
    { std::string(features::Startup), { Slot("INetworkServerService::StartupServer") } },
    // Game loop start (flushes early game event hooks).
    { std::string(features::LoopMode), { Slot("IEngineServiceMgr::RegisterLoopMode"), Slot("IEngineServiceMgr::FindService") } },
    // Tick count, current time, frame time, map name, max players.
    { std::string(features::GlobalVars), { Slot("INetworkServerService::GetIGameServer"), Slot("INetworkGameServer::GetGlobals") } },
    // User message hooks and sends, client prints, replicated convars.
    { std::string(features::UserMessages),
      { Slot("IGameEventSystem::PostEventAbstract"), Slot("IGameEventSystem::PostEventAbstract_Filter") } },
};

// Virtuals called through their Gamedata index on game-class objects (and CheckTransmit, hooked by index on the game
// entities' vtable): Slot Bindings, verified and healed like the others. Managed helpers, plugins reading them by key
// (GameData.GetOffset) and native code all use the index in the report. Declared as Slot Bindings even when Gamedata has
// no "vtable" for them (e.g. an auto-updated gamedata.json from before Slot Bindings): then they are unverified at
// their Gamedata index, as before. Struct field offsets are not here: they stay Offset Bindings.
const std::vector<BindingDeclaration> kGameClassSlots{
    ClassSlot("ISource2GameEntities::CheckTransmit"),
    ClassSlot("CCSPlayerController_ChangeTeam"),
    ClassSlot("CCSPlayerController_Respawn"),
    ClassSlot("CCSPlayer_ItemServices_GiveNamedItem"),
    ClassSlot("CCSPlayer_ItemServices_DropActivePlayerWeapon"),
    ClassSlot("CCSPlayer_ItemServices_RemoveWeapons"),
    ClassSlot("CGameSceneNode_GetSkeletonInstance"),
    ClassSlot("CCSGameRules_FindPickerEntity"),
    ClassSlot("CBaseEntity_IsPlayerPawn"),
    ClassSlot("CBaseEntity_Teleport"),
    ClassSlot("CBasePlayerPawn_CommitSuicide"),
};

// Interface methods only compiled calls use, in modules the boot does not fingerprint yet (network messages, schema
// system); no Engine Feature lists them, so they are declared only where Gamedata has them. Interface Slot Bindings
// like the others: on Windows unverified at their compiled header index, as before Slot Bindings, never failed for want
// of a Windows index in Gamedata.
const std::vector<BindingDeclaration> kCompiledInterfaceSlots{
    Slot("INetworkMessages::FindNetworkMessagePartial"),
    Slot("INetworkMessages::FindNetworkMessageById"),
    Slot("CSchemaSystem::GlobalTypeScope"),
    Slot("CSchemaSystem::FindTypeScopeForModule"),
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
    const auto it = std::find_if(kEngineFeatures.begin(), kEngineFeatures.end(), [&](const EngineFeatureDeclaration& feature) {
        return feature.name == name;
    });
    return it == kEngineFeatures.end() ? nullptr : &*it;
}

} // namespace counterstrikesharp::bindings
