#include "core/compiled_slots.h"

#include <vector>

#include <igameevents.h>
#include <networksystem/inetworkmessages.h>
#include <schemasystem/schemasystem.h>

#include "core/globals.h"
#include "core/hooks.h"
#include "core/slot_binding_keys.h"
#include "engine2/igameeventsystem.h"
#include "engine2/inetworkgameserver.h"

namespace counterstrikesharp::bindings {
namespace {

template <class F> CompiledSlot Compiled(std::string_view key, F method) { return { key, KHook::GetVtableIndex(method) }; }

using PostEventToClients = void (IGameEventSystem::*)(CSplitScreenSlot, bool, int, const uint64*, NetMessageInfo_t*, const CNetMessage*,
                                                      unsigned long, NetChannelBufType_t);
using PostEventToFilter = void (IGameEventSystem::*)(CSplitScreenSlot, bool, IRecipientFilter*, NetMessageInfo_t*, const CNetMessage*,
                                                     unsigned long);

} // namespace

std::span<const CompiledSlot> CompiledSlots()
{
    namespace slots = bindings::slots;
    static const std::vector<CompiledSlot> compiled{
        // Game event manager (natives_events.cpp, event_manager.cpp, chat_manager.cpp)
        Compiled(slots::IGameEventManager2_LoadEventsFromFile, &IGameEventManager2::LoadEventsFromFile),
        Compiled(slots::IGameEventManager2_AddListener, &IGameEventManager2::AddListener),
        Compiled(slots::IGameEventManager2_FindListener, &IGameEventManager2::FindListener),
        Compiled(slots::IGameEventManager2_RemoveListener, &IGameEventManager2::RemoveListener),
        Compiled(slots::IGameEventManager2_CreateEvent, &IGameEventManager2::CreateEvent),
        Compiled(slots::IGameEventManager2_FireEvent, &IGameEventManager2::FireEvent),
        Compiled(slots::IGameEventManager2_FreeEvent, &IGameEventManager2::FreeEvent),
        // Game event system (client prints, user message sends, replicated convars)
        Compiled(slots::IGameEventSystem_PostEventAbstract, static_cast<PostEventToClients>(&IGameEventSystem::PostEventAbstract)),
        Compiled(slots::IGameEventSystem_PostEventAbstract_Filter, static_cast<PostEventToFilter>(&IGameEventSystem::PostEventAbstract)),
        // Global vars (globals::getGlobalVars)
        Compiled(slots::INetworkServerService_GetIGameServer, &INetworkServerService::GetIGameServer),
        Compiled(slots::INetworkGameServer_GetGlobals, &INetworkGameServer::GetGlobals),
        // Not fingerprinted yet (modules not archived): unverified, so not compared until they have a window
        Compiled(slots::INetworkMessages_FindNetworkMessagePartial, &INetworkMessages::FindNetworkMessagePartial),
        Compiled(slots::INetworkMessages_FindNetworkMessageById, &INetworkMessages::FindNetworkMessageById),
        Compiled(slots::CSchemaSystem_GlobalTypeScope, &CSchemaSystem::GlobalTypeScope),
        Compiled(slots::CSchemaSystem_FindTypeScopeForModule, &CSchemaSystem::FindTypeScopeForModule),
    };
    return compiled;
}

} // namespace counterstrikesharp::bindings
