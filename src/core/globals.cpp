// clang-format off
#include "mm_plugin.h"
#include "core/globals.h"
#include "core/managers/player_manager.h"
#include "core/tick_scheduler.h"
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
#include "core/managers/voice_manager.h"
#include "core/managers/usermessage_manager.h"
#include <public/game/server/iplayerinfo.h>
#include <public/entity2/entitysystem.h>

#include <funchook.h>
// clang-format on

namespace counterstrikesharp {

namespace modules {
std::vector<std::unique_ptr<CModule>> moduleList{};
CModule* engine = nullptr;
CModule* tier0 = nullptr;
CModule* server = nullptr;
CModule* schemasystem = nullptr;
CModule* vscript = nullptr;
} // namespace modules

namespace globals {
IVEngineServer2* engineServer2 = nullptr;
IVEngineServer* engine = nullptr;
IGameEventManager2* gameEventManager = nullptr;
IGameEventSystem* gameEventSystem = nullptr;
IPlayerInfoManager* playerinfoManager = nullptr;
IBotManager* botManager = nullptr;
IServerPluginHelpers* helpers = nullptr;
IUniformRandomStream* randomStream = nullptr;
IEngineTrace* engineTrace = nullptr;
IEngineSound* engineSound = nullptr;
IEngineServiceMgr* engineServiceManager = nullptr;
INetworkMessages* networkMessages = nullptr;
INetworkStringTableContainer* netStringTables = nullptr;
CGlobalVars* globalVars = nullptr;
IFileSystem* fileSystem = nullptr;
IServerGameDLL* serverGameDll = nullptr;
IServerGameClients* serverGameClients = nullptr;
INetworkServerService* networkServerService = nullptr;
CSchemaSystem* schemaSystem = nullptr;
IServerTools* serverTools = nullptr;
IPhysics* physics = nullptr;
IPhysicsCollision* physicsCollision = nullptr;
IPhysicsSurfaceProps* physicsSurfaceProps = nullptr;
IMDLCache* modelCache = nullptr;
IVoiceServer* voiceServer = nullptr;
CDotNetManager dotnetManager;
ICvar* cvars = nullptr;
ISource2Server* server = nullptr;
CGlobalEntityList* globalEntityList = nullptr;
CounterStrikeSharpMMPlugin* mmPlugin = nullptr;
SourceHook::Impl::CSourceHookImpl source_hook_impl;
SourceHook::ISourceHook* source_hook = &source_hook_impl;
ISmmAPI* ismm = nullptr;
CGameEntitySystem* entitySystem = nullptr;
CCoreConfig* coreConfig = nullptr;
CGameConfig* gameConfig = nullptr;
ISource2GameEntities* gameEntities = nullptr;

// Custom Managers
CallbackManager callbackManager;
EventManager eventManager;
PlayerManager playerManager;
TimerSystem timerSystem;
ConCommandManager conCommandManager;
EntityManager entityManager;
ChatManager chatManager;
ServerManager serverManager;
VoiceManager voiceManager;
TickScheduler tickScheduler;
UserMessageManager userMessageManager;

bool gameLoopInitialized = false;
GetLegacyGameEventListener_t* GetLegacyGameEventListener = nullptr;
GameEventManagerInit_t* GameEventManagerInit = nullptr;
std::thread::id gameThreadId;

// Based on 64 fixed tick rate
const float engine_fixed_tick_interval = 0.015625f;

void Initialize()
{
    modules::Initialize();

    modules::engine = modules::GetModuleByName(MODULE_PREFIX "engine2" MODULE_EXT);
    modules::tier0 = modules::GetModuleByName(MODULE_PREFIX "tier0" MODULE_EXT);
    modules::server = modules::GetModuleByName(MODULE_PREFIX "server" MODULE_EXT);
    modules::schemasystem = modules::GetModuleByName(MODULE_PREFIX "schemasystem" MODULE_EXT);
    modules::vscript = modules::GetModuleByName(MODULE_PREFIX "vscript" MODULE_EXT);

    interfaces::Initialize();

    entitySystem = interfaces::pGameResourceServiceServer->GetGameEntitySystem();

    GetLegacyGameEventListener = reinterpret_cast<GetLegacyGameEventListener_t*>(
        modules::server->FindSignature(globals::gameConfig->GetSignature("LegacyGameEventListener")));

    GameEventManagerInit = reinterpret_cast<GameEventManagerInit_t*>(
        modules::server->FindSignature(globals::gameConfig->GetSignature("CGameEventManager_Init")));

    if (GameEventManagerInit == nullptr)
    {
        CSSHARP_CORE_ERROR("Failed to find signature for \'GameEventManagerInit\'");
        return;
    }

    auto m_hook = funchook_create();
    funchook_prepare(m_hook, (void**)&GameEventManagerInit, (void*)&DetourGameEventManagerInit);
    funchook_install(m_hook, 0);
}

void DetourGameEventManagerInit(IGameEventManager2* pGameEventManager)
{
    gameEventManager = pGameEventManager;

    GameEventManagerInit(pGameEventManager);

    eventManager.OnAllInitialized_Post();
}

int source_hook_pluginid = 0;
CGlobalVars* getGlobalVars() {
    INetworkGameServer* server = networkServerService->GetIGameServer();
    if (!server) return nullptr;
    return networkServerService->GetIGameServer()->GetGlobals();
}
} // namespace globals
} // namespace counterstrikesharp
