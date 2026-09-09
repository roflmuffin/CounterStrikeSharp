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

#include "core/detours.h"
#include "core/coreconfig.h"
#include "core/game_system.h"
#include "core/gameconfig.h"
#include "core/gameconfig_updater.h"
#include "core/global_listener.h"
#include "core/log.h"
#include "core/managers/entity_manager.h"
#include "core/tick_scheduler.h"
#include "core/timer_system.h"
#include "core/utils.h"
#include "entity2/entitysystem.h"
#include "igameeventsystem.h"
#include "interfaces/cs2_interfaces.h"
#include "iserver.h"
#include "scripting/callback_manager.h"
#include "scripting/dotnet_host.h"
#include "scripting/script_engine.h"
#include "tier0/vprof.h"
#include "tier0/icommandline.h"
#include "tier1/utlstringtoken.h"

DLL_IMPORT ICommandLine* CommandLine();

#define VERSION_STRING  "v" SEMVER " @ " GITHUB_SHA
#define BUILD_TIMESTAMP __DATE__ " " __TIME__

// Vtable of CGameEventManager, resolved at Load() and kept so the global KHook on
// LoadEventsFromFile can be removed again on Unload().
void* g_pCGameEventManagerVTable = nullptr;

counterstrikesharp::GlobalClass* counterstrikesharp::GlobalClass::head = nullptr;

CGameEntitySystem* GameEntitySystem() { return counterstrikesharp::globals::entitySystem; }

// TODO: Workaround for windows, we __MUST__ have COUNTERSTRIKESHARP_API to handle it.
// like on windows it should be `extern "C" __declspec(dllexport)`, on linux it should be anything else.
DLL_EXPORT void InvokeNative(counterstrikesharp::fxNativeContext& context)
{
    if (context.nativeIdentifier == 0) return;

    if (context.nativeIdentifier != counterstrikesharp::hash_string_const("QUEUE_TASK_FOR_FRAME") &&
        context.nativeIdentifier != counterstrikesharp::hash_string_const("GET_SCHEMA_OFFSET") &&
        counterstrikesharp::globals::gameThreadId != std::this_thread::get_id())
    {
        counterstrikesharp::ScriptContextRaw scriptContext(context);
        scriptContext.ThrowNativeError("Invoked on a non-main thread");

        CSSHARP_CORE_CRITICAL("Native {:x} was invoked on a non-main thread", context.nativeIdentifier);
        return;
    }

    counterstrikesharp::ScriptEngine::InvokeNative(context);
}

class GameSessionConfiguration_t
{
};

PLUGIN_EXPOSE(CounterStrikeSharpMMPlugin, counterstrikesharp::gPlugin);

namespace counterstrikesharp {

CounterStrikeSharpMMPlugin gPlugin;

CounterStrikeSharpMMPlugin::CounterStrikeSharpMMPlugin()
    : m_GameFrame(&IServerGameDLL::GameFrame, this, nullptr, &CounterStrikeSharpMMPlugin::Hook_GameFrame),
      m_StartupServer(&INetworkServerService::StartupServer, this, nullptr, &CounterStrikeSharpMMPlugin::Hook_StartupServer),
      m_RegisterLoopMode(&IEngineServiceMgr::RegisterLoopMode, this, &CounterStrikeSharpMMPlugin::Hook_RegisterLoopMode, nullptr),
      m_FindService(&IEngineServiceMgr::FindService, this, nullptr, &CounterStrikeSharpMMPlugin::Hook_FindService),
      m_LoadEventsFromFile(&IGameEventManager2::LoadEventsFromFile, this, &CounterStrikeSharpMMPlugin::Hook_LoadEventsFromFile, nullptr)
{
}

#if 0
// Currently unavailable, requires hl2sdk work!
ConVar sample_cvar("sample_cvar", "42", 0);
#endif

bool CounterStrikeSharpMMPlugin::Load(PluginId id, ISmmAPI* ismm, char* error, size_t maxlen, bool late)
{
    PLUGIN_SAVEVARS();
    ismm->AddListener(this, this);

    globals::ismm = ismm;
    globals::gameThreadId = std::this_thread::get_id();

    Log::Init();

    CSSHARP_CORE_INFO("Initializing with command line: {}", CommandLine()->GetCmdLine());
    const char* basePath = CommandLine()->ParmValue(MakeStringToken("+css_basepath"), "/addons/counterstrikesharp");

    GET_V_IFACE_CURRENT(GetEngineFactory, globals::engineServer2, IVEngineServer2, SOURCE2ENGINETOSERVER_INTERFACE_VERSION);
    GET_V_IFACE_CURRENT(GetEngineFactory, globals::engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
    GET_V_IFACE_CURRENT(GetEngineFactory, globals::cvars, ICvar, CVAR_INTERFACE_VERSION);
    GET_V_IFACE_CURRENT(GetEngineFactory, g_pGameResourceServiceServer, IGameResourceService, GAMERESOURCESERVICESERVER_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetServerFactory, globals::server, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
    GET_V_IFACE_ANY(GetServerFactory, globals::serverGameClients, IServerGameClients, INTERFACEVERSION_SERVERGAMECLIENTS);
    GET_V_IFACE_ANY(GetEngineFactory, globals::networkServerService, INetworkServerService, NETWORKSERVERSERVICE_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetEngineFactory, globals::schemaSystem, CSchemaSystem, SCHEMASYSTEM_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetEngineFactory, globals::gameEventSystem, IGameEventSystem, GAMEEVENTSYSTEM_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetEngineFactory, globals::engineServiceManager, IEngineServiceMgr, ENGINESERVICEMGR_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetEngineFactory, globals::networkMessages, INetworkMessages, NETWORKMESSAGES_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetServerFactory, globals::gameEntities, ISource2GameEntities, SOURCE2GAMEENTITIES_INTERFACE_VERSION);
    g_pCVar = globals::cvars;
    g_pSource2GameEntities = globals::gameEntities;
    interfaces::pGameResourceServiceServer = (CGameResourceService*)g_pGameResourceServiceServer;

    if (utils::RelativeDirectory(std::string(basePath)) == "NotFound")
    {
        CSSHARP_CORE_ERROR("Invalid base path: {}", basePath);
        return false;
    }
    CSSHARP_CORE_INFO("Current root directory: {}", utils::GetRootDirectory());

    auto coreconfig_path = std::string(utils::ConfigsDirectory() + "/core");
    globals::coreConfig = new CCoreConfig(coreconfig_path);
    char coreconfig_error[255] = "";

    if (!globals::coreConfig->Init(coreconfig_error, sizeof(coreconfig_error)))
    {
        CSSHARP_CORE_ERROR("Could not read \'{}\'. Error: {}", coreconfig_path, coreconfig_error);
        return false;
    }

    CSSHARP_CORE_INFO("CoreConfig loaded.");

    if (globals::coreConfig->AutoUpdateEnabled)
    {
#ifdef _WIN32
        if (!update::TryUpdateGameConfig())
        {
            CSSHARP_CORE_ERROR("Failed to update game config.");
        }
#else
        CSSHARP_CORE_WARN("Auto-update is not currently supported on this platform.");
#endif
    }

    auto gamedata_path = std::string(utils::GamedataDirectory() + "/gamedata.json");
    globals::gameConfig = new CGameConfig(gamedata_path);
    char conf_error[255] = "";

    if (!globals::gameConfig->Init(conf_error, sizeof(conf_error)))
    {
        CSSHARP_CORE_ERROR("Could not read \'{}\'. Error: {}", gamedata_path, conf_error);
        return false;
    }

    globals::Initialize();

    CSSHARP_CORE_INFO("Globals loaded.");
    globals::mmPlugin = &gPlugin;

    CALL_GLOBAL_LISTENER(OnAllInitialized());

    on_activate_callback = globals::callbackManager.CreateCallback("OnMapStart");
    on_map_end_callback = globals::callbackManager.CreateCallback("OnMapEnd");
    on_metamod_all_plugins_loaded_callback = globals::callbackManager.CreateCallback("OnMetamodAllPluginsLoaded");

    m_GameFrame.Add(globals::server);
    m_StartupServer.Add(globals::networkServerService);
    m_RegisterLoopMode.Add(globals::engineServiceManager);
    m_FindService.Add(globals::engineServiceManager);

    // CGameEventManager is instantiated by the engine after we load, so hook every
    // instance sharing the class vtable rather than a specific object (the KHook
    // equivalent of SourceHook's DVP hook). AddGlobal() dereferences its argument to
    // get the vtable, hence the address-of on the vtable pointer.
    g_pCGameEventManagerVTable = modules::server->FindVirtualTable("CGameEventManager");
    if (g_pCGameEventManagerVTable != nullptr)
    {
        m_LoadEventsFromFile.AddGlobal((IGameEventManager2*)&g_pCGameEventManagerVTable);
    }
    else
    {
        ismm->Format(error, maxlen, "Failed to find the CGameEventManager vtable required for game events.");
        return false;
    }

    if (!InitGameSystems())
    {
        CSSHARP_CORE_ERROR("Failed to initialize GameSystem!");
        return false;
    }

    CSSHARP_CORE_INFO("Initialized GameSystem.");

    if (!globals::dotnetManager.Initialize())
    {
        CSSHARP_CORE_ERROR("Failed to initialize .NET runtime");
    }

    CSSHARP_CORE_INFO("Hooks added.");

    // Used by Metamod Console Commands
    g_pCVar = globals::cvars;
    ConVar_Register(FCVAR_RELEASE | FCVAR_CLIENT_CAN_EXECUTE | FCVAR_GAMEDLL);

    return true;
}

KHook::Return<void> CounterStrikeSharpMMPlugin::Hook_StartupServer(INetworkServerService*, const GameSessionConfiguration_t& config, ISource2WorldSession*, const char*)
{
    globals::entitySystem = interfaces::pGameResourceServiceServer->GetGameEntitySystem();
    globals::entitySystem->AddListenerEntity(&globals::entityManager.entityListener);

    globals::timerSystem.OnStartupServer();

    on_activate_callback->ScriptContext().Reset();
    on_activate_callback->ScriptContext().Push(globals::getGlobalVars()->mapname.ToCStr());
    on_activate_callback->Execute();
    return { KHook::Action::Ignore };
}
bool CounterStrikeSharpMMPlugin::Unload(char* error, size_t maxlen)
{
    m_GameFrame.Remove(globals::server);
    m_StartupServer.Remove(globals::networkServerService);
    m_RegisterLoopMode.Remove(globals::engineServiceManager);
    m_FindService.Remove(globals::engineServiceManager);
    if (g_pCGameEventManagerVTable != nullptr)
    {
        m_LoadEventsFromFile.RemoveGlobal(reinterpret_cast<IGameEventManager2*>(&g_pCGameEventManagerVTable));
        g_pCGameEventManagerVTable = nullptr;
    }

    globals::callbackManager.ReleaseCallback(on_activate_callback);
    globals::callbackManager.ReleaseCallback(on_map_end_callback);
    globals::callbackManager.ReleaseCallback(on_metamod_all_plugins_loaded_callback);

    return true;
}

void CounterStrikeSharpMMPlugin::AllPluginsLoaded()
{
    /* This is where we'd do stuff that relies on the mod or other plugins
     * being initialized (for example, cvars added and events registered).
     */
    on_metamod_all_plugins_loaded_callback->ScriptContext().Reset();
    on_metamod_all_plugins_loaded_callback->Execute();

    if (globals::entityManager.Func_OnTakeDamage)
    {
        globals::entityManager.Func_OnTakeDamage->AddHook(&OnTakeDamageProxy);
    }
}

KHook::Return<void> CounterStrikeSharpMMPlugin::Hook_GameFrame(IServerGameDLL*, bool simulating, bool bFirstTick, bool bLastTick)
{
    /**
     * simulating:
     * ***********
     * true  | game is ticking
     * false | game is not ticking
     */
    // VPROF_BUDGET("CS#::Hook_GameFrame", "CS# On Frame");
    globals::timerSystem.OnGameFrame(simulating);

    auto callbacks = globals::tickScheduler.getCallbacks(globals::getGlobalVars()->tickcount);
    if (callbacks.size() > 0)
    {
        CSSHARP_CORE_TRACE("Executing frame specific tasks of size: {0} on tick number {1}", callbacks.size(),
                           globals::getGlobalVars()->tickcount);

        for (auto& callback : callbacks)
        {
            callback();
        }
    }
    return { KHook::Action::Ignore };
}

// Potentially might not work
void CounterStrikeSharpMMPlugin::OnLevelInit(
    char const* pMapName, char const* pMapEntities, char const* pOldLevel, char const* pLandmarkName, bool loadGame, bool background)
{
    CSSHARP_CORE_TRACE("name={0},mapname={1}", "LevelInit", pMapName);

    m_has_level_initialized = true;
}

KHook::Return<void> CounterStrikeSharpMMPlugin::Hook_RegisterLoopMode(IEngineServiceMgr*,
                                                                      const char* pszLoopModeName,
                                                                      ILoopModeFactory* pLoopModeFactory,
                                                                      void** ppGlobalPointer)
{
    if (strcmp(pszLoopModeName, "game") == 0)
    {
        if (!globals::gameLoopInitialized) globals::gameLoopInitialized = true;

        CALL_GLOBAL_LISTENER(OnGameLoopInitialized());
    }
    return { KHook::Action::Ignore };
}

KHook::Return<IEngineService*> CounterStrikeSharpMMPlugin::Hook_FindService(IEngineServiceMgr*, const char* serviceName)
{
    // A preceding plugin may have superseded the call, leaving no original return.
    auto* original = static_cast<IEngineService**>(KHook::GetOriginalValuePtr());
    IEngineService* pService = original ? *original : nullptr;

    return { KHook::Action::Ignore, pService };
}

KHook::Return<int> CounterStrikeSharpMMPlugin::Hook_LoadEventsFromFile(IGameEventManager2* pGameEventManager, const char* filename, bool bSearchAll)
{
    ExecuteOnce(globals::gameEventManager = pGameEventManager);

    return { KHook::Action::Ignore, 0 };
}

void CounterStrikeSharpMMPlugin::OnLevelShutdown()
{
    CSSHARP_CORE_TRACE("name={0}", "LevelShutdown");

    if (!m_has_level_initialized)
    {
        return;
    }

    m_has_level_initialized = false;

    if (on_map_end_callback && on_map_end_callback->GetFunctionCount())
    {
        on_map_end_callback->ScriptContext().Reset();
        on_map_end_callback->Execute();
    }
}

bool CounterStrikeSharpMMPlugin::Pause(char* error, size_t maxlen) { return true; }

bool CounterStrikeSharpMMPlugin::Unpause(char* error, size_t maxlen) { return true; }

const char* CounterStrikeSharpMMPlugin::GetLicense() { return "GNU GPLv3"; }

const char* CounterStrikeSharpMMPlugin::GetVersion() { return VERSION_STRING; }

const char* CounterStrikeSharpMMPlugin::GetDate() { return BUILD_TIMESTAMP; }

const char* CounterStrikeSharpMMPlugin::GetLogTag() { return "CSSHARP"; }

const char* CounterStrikeSharpMMPlugin::GetAuthor() { return "Roflmuffin"; }

const char* CounterStrikeSharpMMPlugin::GetDescription() { return "Counter Strike .NET Scripting Runtime"; }

const char* CounterStrikeSharpMMPlugin::GetName() { return "CounterStrikeSharp"; }

const char* CounterStrikeSharpMMPlugin::GetURL() { return "https://github.com/roflmuffin/CounterStrikeSharp"; }
} // namespace counterstrikesharp
