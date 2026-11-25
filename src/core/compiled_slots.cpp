#include "core/compiled_slots.h"

#include <vector>

#include <igameevents.h>
#include <networksystem/inetworkmessages.h>
#include <schemasystem/schemasystem.h>

#include "core/globals.h"
#include "core/hooks.h"
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
    static const std::vector<CompiledSlot> slots{
        // Game event manager (natives_events.cpp, event_manager.cpp, chat_manager.cpp)
        Compiled("IGameEventManager2::LoadEventsFromFile", &IGameEventManager2::LoadEventsFromFile),
        Compiled("IGameEventManager2::AddListener", &IGameEventManager2::AddListener),
        Compiled("IGameEventManager2::FindListener", &IGameEventManager2::FindListener),
        Compiled("IGameEventManager2::RemoveListener", &IGameEventManager2::RemoveListener),
        Compiled("IGameEventManager2::CreateEvent", &IGameEventManager2::CreateEvent),
        Compiled("IGameEventManager2::FireEvent", &IGameEventManager2::FireEvent),
        Compiled("IGameEventManager2::FreeEvent", &IGameEventManager2::FreeEvent),
        // Game event system (client prints, user message sends, replicated convars)
        Compiled("IGameEventSystem::PostEventAbstract", static_cast<PostEventToClients>(&IGameEventSystem::PostEventAbstract)),
        Compiled("IGameEventSystem::PostEventAbstract_Filter", static_cast<PostEventToFilter>(&IGameEventSystem::PostEventAbstract)),
        // Global vars (globals::getGlobalVars)
        Compiled("INetworkServerService::GetIGameServer", &INetworkServerService::GetIGameServer),
        Compiled("INetworkGameServer::GetGlobals", &INetworkGameServer::GetGlobals),
        // Not fingerprinted yet (modules not archived): unverified, so not compared until they have a window
        Compiled("INetworkMessages::FindNetworkMessagePartial", &INetworkMessages::FindNetworkMessagePartial),
        Compiled("INetworkMessages::FindNetworkMessageById", &INetworkMessages::FindNetworkMessageById),
        Compiled("CSchemaSystem::GlobalTypeScope", &CSchemaSystem::GlobalTypeScope),
        Compiled("CSchemaSystem::FindTypeScopeForModule", &CSchemaSystem::FindTypeScopeForModule),
    };
    return slots;
}

} // namespace counterstrikesharp::bindings
