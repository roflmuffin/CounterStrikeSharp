#include "mm_plugin.h"
#include "core/globals.h"
#include "core/managers/player_manager.h"
#include "iserver.h"
#include "managers/event_manager.h"
#include "scripting/callback_manager.h"
#include "scripting/dotnet_host.h"
#include "timer_system.h"

#include <ISmmPlugin.h>
#include <sourcehook/sourcehook.h>
#include <sourcehook/sourcehook_impl.h>

#include "log.h"
#include "utils/virtual.h"
#include "core/memory.h"
#include "core/managers/con_command_manager.h"
#include "core/managers/chat_manager.h"
#include "memory_module.h"
#include "interfaces/cs2_interfaces.h"
#include "core/managers/entity_manager.h"
#include "core/managers/server_manager.h"
#include <public/game/server/iplayerinfo.h>
#include <public/entity2/entitysystem.h>


namespace counterstrikesharp {

namespace modules {
CModule *engine = nullptr;
CModule *tier0 = nullptr;
CModule *server = nullptr;
CModule *schemasystem = nullptr;
CModule *vscript = nullptr;
}  // namespace modules

namespace globals {
IVEngineServer *engine = nullptr;
IGameEventManager2 *gameEventManager = nullptr;
IGameEventSystem *gameEventSystem = nullptr;
IPlayerInfoManager *playerinfoManager = nullptr;
IBotManager *botManager = nullptr;
IServerPluginHelpers *helpers = nullptr;
IUniformRandomStream *randomStream = nullptr;
IEngineTrace *engineTrace = nullptr;
IEngineSound *engineSound = nullptr;
INetworkStringTableContainer *netStringTables = nullptr;
CGlobalVars *globalVars = nullptr;
IFileSystem *fileSystem = nullptr;
IServerGameDLL *serverGameDll = nullptr;
IServerGameClients *serverGameClients = nullptr;
INetworkServerService *networkServerService = nullptr;
IServerTools *serverTools = nullptr;
IPhysics *physics = nullptr;
IPhysicsCollision *physicsCollision = nullptr;
IPhysicsSurfaceProps *physicsSurfaceProps = nullptr;
IMDLCache *modelCache = nullptr;
IVoiceServer *voiceServer = nullptr;
CDotNetManager dotnetManager;
ICvar *cvars = nullptr;
ISource2Server *server = nullptr;
CGlobalEntityList *globalEntityList = nullptr;
CounterStrikeSharpMMPlugin *mmPlugin = nullptr;
SourceHook::Impl::CSourceHookImpl source_hook_impl;
SourceHook::ISourceHook *source_hook = &source_hook_impl;
ISmmAPI *ismm = nullptr;
CGameEntitySystem* entitySystem = nullptr;
CCoreConfig* coreConfig = nullptr;
CGameConfig* gameConfig = nullptr;

// Custom Managers
CallbackManager callbackManager;
EventManager eventManager;
PlayerManager playerManager;
TimerSystem timerSystem;
ConCommandManager conCommandManager;
EntityManager entityManager;
ChatManager chatManager;
ServerManager serverManager;

void Initialize() {
    modules::engine = new modules::CModule(ROOTBIN, "engine2");
    modules::tier0 = new modules::CModule(ROOTBIN, "tier0");
    modules::server = new modules::CModule(GAMEBIN, "server");
    modules::schemasystem = new modules::CModule(ROOTBIN, "schemasystem");
    modules::vscript = new modules::CModule(ROOTBIN, "vscript");

    interfaces::Initialize();

    entitySystem = interfaces::pGameResourceServiceServer->GetGameEntitySystem();

    if (int offset = -1; (offset = gameConfig->GetOffset("GameEventManager")) != -1) {
        gameEventManager = (IGameEventManager2*)(CALL_VIRTUAL(uintptr_t, offset, server) - 8);
    }
}

int source_hook_pluginid = 0;
CGlobalVars *getGlobalVars() {
    INetworkGameServer *server = networkServerService->GetIGameServer();

    if (!server) {
        return nullptr;
    }

    return networkServerService->GetIGameServer()->GetGlobals();
}

}  // namespace globals
}  // namespace counterstrikesharp
