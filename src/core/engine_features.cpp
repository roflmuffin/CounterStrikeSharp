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
    { std::string(features::GameEvents), { Signature("CGameEventManager_Init"), RttiVtable("CGameEventManager", "server") } },
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
        // Only Gamedata declares it, so no native code calls it through a compiled method.
        if (entry.slot) bindings.push_back(ClassSlot(key));
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
