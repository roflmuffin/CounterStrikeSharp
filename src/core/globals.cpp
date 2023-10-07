#include "core/globals.h"
#include "iserver.h"
#include "managers/event_manager.h"
#include "scripting/callback_manager.h"
#include "scripting/dotnet_host.h"

#include <ISmmPlugin.h>
#include <sourcehook/sourcehook.h>
#include <sourcehook/sourcehook_impl.h>

#include <public/game/server/iplayerinfo.h>

namespace counterstrikesharp
{

namespace globals
{
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
SourceHook::Impl::CSourceHookImpl source_hook_impl;
SourceHook::ISourceHook *source_hook = &source_hook_impl;

// Custom Managers
CallbackManager callbackManager;
EventManager eventManager;
int source_hook_pluginid = 0;

} // namespace globals
} // namespace counterstrikesharp
