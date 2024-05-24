#pragma once

#define protected public
#define private   public
#include <tier1/convar.h>
#undef protected
#undef private

#include <thread>

#include <memory>

#include "ISmmAPI.h"
#include "eiface.h"
#include "iserver.h"
#include <sourcehook/sourcehook.h>

class IGameEventManager2;
class IPlayerInfoManager;
class IBotManager;
class IServerPluginHelpers;
class IUniformRandomStream;
class IEngineTrace;
class IEngineSound;
class IEngineServiceMgr;
class INetworkStringTableContainer;
class CGlobalVars;
class IFileSystem;
class IServerTools;
class IPhysics;
class IPhysicsCollision;
class IPhysicsSurfaceProps;
class IMDLCache;
class IVoiceServer;
class CGlobalEntityList;
class CDotNetManager;
class ICvar;
class IGameEventSystem;
class CounterStrikeSharpMMPlugin;
class CGameEntitySystem;
class IGameEventListener2;
class CSchemaSystem;

namespace counterstrikesharp {
class EntityListener;
class EventManager;
class UserMessageManager;
class ConCommandManager;
class CallbackManager;
class ConVarManager;
class PlayerManager;
class MenuManager;
class TickScheduler;
class TimerSystem;
class ChatCommands;
class HookManager;
class EntityManager;
class ChatManager;
class ServerManager;
class VoiceManager;
class CCoreConfig;
class CGameConfig;

namespace globals {

extern IVEngineServer* engine;
extern IVEngineServer2* engineServer2;
extern IGameEventManager2* gameEventManager;
extern IPlayerInfoManager* playerinfoManager;
extern IBotManager* botManager;
extern IServerPluginHelpers* helpers;
extern IUniformRandomStream* randomStream;
extern IEngineTrace* engineTrace;
extern IEngineSound* engineSound;
extern IEngineServiceMgr* engineServiceManager;
extern INetworkStringTableContainer* netStringTables;
extern CGlobalVars* globalVars;
extern IFileSystem* fileSystem;
extern IServerGameDLL* serverGameDll;
extern IServerGameClients* serverGameClients;
extern INetworkServerService* networkServerService;
extern CSchemaSystem* schemaSystem;
extern IServerTools* serverTools;
extern IPhysics* physics;
extern IPhysicsCollision* physicsCollision;
extern IPhysicsSurfaceProps* physicsSurfaceProps;
extern IMDLCache* modelCache;
extern IVoiceServer* voiceServer;
extern CDotNetManager dotnetManager;
extern ICvar* cvars;
extern ISource2Server* server;
extern CGlobalEntityList* globalEntityList;
extern EntityListener entityListener;
extern CGameEntitySystem* entitySystem;

extern EventManager eventManager;
extern UserMessageManager userMessageManager;
extern ConCommandManager conCommandManager;
extern CallbackManager callbackManager;
extern ConVarManager convarManager;
extern PlayerManager playerManager;
extern MenuManager menuManager;
extern EntityManager entityManager;
extern TimerSystem timerSystem;
extern ChatCommands chatCommands;
extern ChatManager chatManager;
extern ServerManager serverManager;
extern VoiceManager voiceManager;
extern TickScheduler tickScheduler;

extern HookManager hookManager;
extern SourceHook::ISourceHook* source_hook;
extern int source_hook_pluginid;
extern IGameEventSystem* gameEventSystem;
extern CounterStrikeSharpMMPlugin* mmPlugin;
extern ISmmAPI* ismm;
extern CCoreConfig* coreConfig;
extern CGameConfig* gameConfig;

extern const float engine_fixed_tick_interval;

typedef void GameEventManagerInit_t(IGameEventManager2* gameEventManager);
typedef IGameEventListener2* GetLegacyGameEventListener_t(CPlayerSlot slot);

static void DetourGameEventManagerInit(IGameEventManager2* gameEventManager);

extern bool gameLoopInitialized;
extern GetLegacyGameEventListener_t* GetLegacyGameEventListener;
extern std::thread::id gameThreadId;

void Initialize();
// Should only be called within the active game loop (i e map should be loaded
// and active) otherwise that'll be nullptr!
CGlobalVars* getGlobalVars();
} // namespace globals

namespace modules {
class CModule;

void Initialize();
CModule* GetModuleByName(std::string name);

extern std::vector<std::unique_ptr<CModule>> moduleList;

extern CModule* engine;
extern CModule* tier0;
extern CModule* server;
extern CModule* schemasystem;
extern CModule* vscript;
} // namespace modules

} // namespace counterstrikesharp

#undef SH_GLOB_SHPTR
#define SH_GLOB_SHPTR counterstrikesharp::globals::source_hook
#undef SH_GLOB_PLUGPTR
#define SH_GLOB_PLUGPTR counterstrikesharp::globals::source_hook_pluginid
