#pragma once

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

namespace counterstrikesharp
{
class EntityListener;
class EventManager;
class UserMessageManager;
class ConCommandManager;
class CallbackManager;
class ConVarManager;
class PlayerManager;
class MenuManager;
class TimerSystem;
class ChatCommands;
class HookManager;

namespace globals
{

extern IVEngineServer *engine;
// extern IGameEventManager2 *gameEventManager; // Disabled until we can figure out how to hook it.
extern IPlayerInfoManager *playerinfoManager;
extern IBotManager *botManager;
extern IServerPluginHelpers *helpers;
extern IUniformRandomStream *randomStream;
extern IEngineTrace *engineTrace;
extern IEngineSound *engineSound;
extern INetworkStringTableContainer *netStringTables;
extern CGlobalVars *globalVars;
extern IFileSystem *fileSystem;
extern IServerGameDLL *serverGameDll;
extern IServerGameClients *serverGameClients;
extern INetworkServerService *networkServerService;
extern IServerTools *serverTools;
extern IPhysics *physics;
extern IPhysicsCollision *physicsCollision;
extern IPhysicsSurfaceProps *physicsSurfaceProps;
extern IMDLCache *modelCache;
extern IVoiceServer *voiceServer;
extern CDotNetManager dotnetManager;
extern ICvar *cvars;
extern ISource2Server *server;
extern CGlobalEntityList *globalEntityList;
extern EntityListener entityListener;
extern EventManager eventManager;
extern UserMessageManager userMessageManager;
extern ConCommandManager conCommandManager;
extern CallbackManager callbackManager;
extern ConVarManager convarManager;
extern PlayerManager playerManager;
extern MenuManager menuManager;
extern TimerSystem timerSystem;
extern ChatCommands chatCommands;
extern HookManager hookManager;
extern SourceHook::ISourceHook *source_hook;
extern int source_hook_pluginid;
extern IGameEventSystem *gameEventSystem;
} // namespace globals

} // namespace counterstrikesharp

#undef SH_GLOB_SHPTR
#define SH_GLOB_SHPTR counterstrikesharp::globals::source_hook
#undef SH_GLOB_PLUGPTR
#define SH_GLOB_PLUGPTR counterstrikesharp::globals::source_hook_pluginid