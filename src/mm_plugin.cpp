/**
 * vim: set ts=4 sw=4 tw=99 noet :
 * ======================================================
 * Metamod:Source Sample Plugin
 * Written by AlliedModders LLC.
 * ======================================================
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * This sample plugin is public domain.
 */

#include "mm_plugin.h"

#include <cstdio>

#include "core/global_listener.h"
#include "core/log.h"
#include "core/coreconfig.h"
#include "core/gameconfig.h"
#include "core/timer_system.h"
#include "core/utils.h"
#include "core/managers/entity_manager.h"
#include "igameeventsystem.h"
#include "iserver.h"
#include "scripting/callback_manager.h"
#include "scripting/dotnet_host.h"
#include "scripting/script_engine.h"
#include "entity2/entitysystem.h"
#include "interfaces/cs2_interfaces.h"


counterstrikesharp::GlobalClass* counterstrikesharp::GlobalClass::head = nullptr;

// TODO: Workaround for windows, we __MUST__ have COUNTERSTRIKESHARP_API to handle it.
// like on windows it should be `extern "C" __declspec(dllexport)`, on linux it should be anything else.
DLL_EXPORT void InvokeNative(counterstrikesharp::fxNativeContext& context)
{
    if (context.nativeIdentifier == 0)
        return;

    counterstrikesharp::ScriptEngine::InvokeNative(context);
}

class GameSessionConfiguration_t
{
};

namespace counterstrikesharp {

SH_DECL_HOOK3_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool, bool, bool);
SH_DECL_HOOK3_void(INetworkServerService, StartupServer, SH_NOATTRIB, 0,
                   const GameSessionConfiguration_t&, ISource2WorldSession*, const char*);

CounterStrikeSharpMMPlugin gPlugin;

#if 0
// Currently unavailable, requires hl2sdk work!
ConVar sample_cvar("sample_cvar", "42", 0);
#endif

PLUGIN_EXPOSE(CounterStrikeSharpMMPlugin, gPlugin);
bool CounterStrikeSharpMMPlugin::Load(PluginId id, ISmmAPI* ismm, char* error, size_t maxlen,
                                      bool late)
{
    PLUGIN_SAVEVARS();
    globals::ismm = ismm;

    Log::Init();

    CSSHARP_CORE_INFO("Initializing");

    GET_V_IFACE_CURRENT(GetEngineFactory, globals::engine, IVEngineServer,
                        INTERFACEVERSION_VENGINESERVER);
    GET_V_IFACE_CURRENT(GetEngineFactory, globals::cvars, ICvar, CVAR_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetServerFactory, globals::server, IServerGameDLL,
                    INTERFACEVERSION_SERVERGAMEDLL);
    GET_V_IFACE_ANY(GetServerFactory, globals::serverGameClients, IServerGameClients,
                    INTERFACEVERSION_SERVERGAMECLIENTS);
    GET_V_IFACE_ANY(GetEngineFactory, globals::networkServerService, INetworkServerService,
                    NETWORKSERVERSERVICE_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetEngineFactory, globals::gameEventSystem, IGameEventSystem,
                    GAMEEVENTSYSTEM_INTERFACE_VERSION);

    auto coreconfig_path = std::string(utils::ConfigsDirectory() + "/core.json");
    globals::coreConfig = new CCoreConfig(coreconfig_path);
    char coreconfig_error[255] = "";

    if (!globals::coreConfig->Init(coreconfig_error, sizeof(coreconfig_error))) {
        CSSHARP_CORE_ERROR("Could not read \'{}\'. Error: {}", coreconfig_path, coreconfig_error);
        return false;
    }

    CSSHARP_CORE_INFO("CoreConfig loaded.");

    auto gamedata_path = std::string(utils::GamedataDirectory() + "/gamedata.json");
    globals::gameConfig = new CGameConfig(gamedata_path);
    char conf_error[255] = "";

    if (!globals::gameConfig->Init(conf_error, sizeof(conf_error))) {
        CSSHARP_CORE_ERROR("Could not read \'{}\'. Error: {}", gamedata_path, conf_error);
        return false;
    }

    globals::Initialize();

    CSSHARP_CORE_INFO("Globals loaded.");
    globals::mmPlugin = &gPlugin;

    CALL_GLOBAL_LISTENER(OnAllInitialized());

    on_activate_callback = globals::callbackManager.CreateCallback("OnMapStart");

    SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, globals::server, this,
                        &CounterStrikeSharpMMPlugin::Hook_GameFrame, true);
    SH_ADD_HOOK_MEMFUNC(INetworkServerService, StartupServer, globals::networkServerService, this,
                        &CounterStrikeSharpMMPlugin::Hook_StartupServer, true);

    if (!globals::dotnetManager.Initialize()) {
        CSSHARP_CORE_ERROR("Failed to initialize .NET runtime");
    }

    CSSHARP_CORE_INFO("Hooks added.");

    // Used by Metamod Console Commands
    g_pCVar = globals::cvars;
    ConVar_Register(FCVAR_RELEASE | FCVAR_CLIENT_CAN_EXECUTE | FCVAR_GAMEDLL);

    return true;
}

void CounterStrikeSharpMMPlugin::Hook_StartupServer(const GameSessionConfiguration_t& config,
                                                    ISource2WorldSession*, const char*)
{
    globals::entitySystem = interfaces::pGameResourceServiceServer->GetGameEntitySystem();
    globals::entitySystem->AddListenerEntity(&globals::entityManager.entityListener);
    globals::timerSystem.OnStartupServer();

    on_activate_callback->ScriptContext().Reset();
    on_activate_callback->ScriptContext().Push(globals::getGlobalVars()->mapname);
    on_activate_callback->Execute();
}

bool CounterStrikeSharpMMPlugin::Unload(char* error, size_t maxlen)
{
    SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, globals::server, this,
                           &CounterStrikeSharpMMPlugin::Hook_GameFrame, true);
    SH_REMOVE_HOOK_MEMFUNC(INetworkServerService, StartupServer, globals::networkServerService,
                           this, &CounterStrikeSharpMMPlugin::Hook_StartupServer, true);

    globals::callbackManager.ReleaseCallback(on_activate_callback);

    return true;
}

void CounterStrikeSharpMMPlugin::AllPluginsLoaded()
{
    /* This is where we'd do stuff that relies on the mod or other plugins
     * being initialized (for example, cvars added and events registered).
     */
}

void CounterStrikeSharpMMPlugin::AddTaskForNextFrame(std::function<void()>&& task)
{
    m_nextTasks.push_back(std::forward<decltype(task)>(task));
}

void CounterStrikeSharpMMPlugin::Hook_GameFrame(bool simulating, bool bFirstTick, bool bLastTick)
{
    /**
     * simulating:
     * ***********
     * true  | game is ticking
     * false | game is not ticking
     */
    globals::timerSystem.OnGameFrame(simulating);

    if (m_nextTasks.empty())
        return;

    CSSHARP_CORE_TRACE("Executing queued tasks of size: {0} on tick number {1}", m_nextTasks.size(),
                       globals::getGlobalVars()->tickcount);

    for (size_t i = 0; i < m_nextTasks.size(); i++) {
        m_nextTasks[i]();
    }

    m_nextTasks.clear();
}

// Potentially might not work
void CounterStrikeSharpMMPlugin::OnLevelInit(char const* pMapName, char const* pMapEntities,
                                             char const* pOldLevel, char const* pLandmarkName,
                                             bool loadGame, bool background)
{
    CSSHARP_CORE_TRACE("name={0},mapname={1}", "LevelInit", pMapName);
}

void CounterStrikeSharpMMPlugin::OnLevelShutdown() {}

bool CounterStrikeSharpMMPlugin::Pause(char* error, size_t maxlen) { return true; }

bool CounterStrikeSharpMMPlugin::Unpause(char* error, size_t maxlen) { return true; }

const char* CounterStrikeSharpMMPlugin::GetLicense() { return "GNU GPLv3"; }

const char* CounterStrikeSharpMMPlugin::GetVersion() { return "0.1.0"; }

const char* CounterStrikeSharpMMPlugin::GetDate() { return __DATE__; }

const char* CounterStrikeSharpMMPlugin::GetLogTag() { return "CSSHARP"; }

const char* CounterStrikeSharpMMPlugin::GetAuthor() { return "Roflmuffin"; }

const char* CounterStrikeSharpMMPlugin::GetDescription()
{
    return "Counter Strike .NET Scripting Runtime";
}

const char* CounterStrikeSharpMMPlugin::GetName() { return "CounterStrikeSharp"; }

const char* CounterStrikeSharpMMPlugin::GetURL()
{
    return "https://github.com/roflmuffin/CounterStrikeSharp";
}
} // namespace counterstrikesharp