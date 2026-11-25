#include "core/engine_features.h"

#include <algorithm>
#include <string>

namespace counterstrikesharp::bindings {
namespace {

BindingDeclaration Signature(std::string key) { return { std::move(key), BindingKind::Signature }; }
BindingDeclaration Offset(std::string key) { return { std::move(key), BindingKind::Offset }; }
// A vtable slot we hook or call: its index, class and fingerprint window come from Gamedata (ADR-0003).
BindingDeclaration Slot(std::string key) { return { std::move(key), BindingKind::Slot }; }
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
    { std::string(features::CheckTransmit), { Offset("ISource2GameEntities::CheckTransmit"), Offset("CheckTransmitPlayerSlot") } },
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
    { std::string(features::EntityTakeDamageHooks), { Signature("CBaseEntity_TakeDamageOld"), Offset("CBaseEntity_IsPlayerPawn") } },
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

    for (const auto& [key, entry] : gamedata)
    {
        if (Declared(bindings, key)) continue;
        if (entry.slot) bindings.push_back(Slot(key));
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
