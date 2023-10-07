#pragma ide diagnostic ignored "readability-identifier-naming"
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

#include "sample_mm.h"

#include <cstdio>

#include "core/global_listener.h"
#include "core/log.h"
#include "core/utils.h"
#include "iserver.h"
#include "scripting/callback_manager.h"
#include "scripting/dotnet_host.h"
#include "scripting/script_engine.h"

counterstrikesharp::GlobalClass *counterstrikesharp::GlobalClass::head = nullptr;

extern "C" void InvokeNative(counterstrikesharp::fxNativeContext &context)
{
    if (context.nativeIdentifier == 0)
        return;

    counterstrikesharp::ScriptEngine::InvokeNative(context);
}

namespace counterstrikesharp
{
SH_DECL_HOOK3_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool, bool, bool);
SH_DECL_HOOK4_void(IServerGameClients, ClientActive, SH_NOATTRIB, 0, CPlayerSlot, bool, const char *, uint64);
SH_DECL_HOOK5_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, CPlayerSlot, int, const char *, uint64,
                   const char *);
SH_DECL_HOOK4_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, CPlayerSlot, char const *, int, uint64);
SH_DECL_HOOK1_void(IServerGameClients, ClientSettingsChanged, SH_NOATTRIB, 0, CPlayerSlot);
SH_DECL_HOOK6_void(IServerGameClients, OnClientConnected, SH_NOATTRIB, 0, CPlayerSlot, const char *, uint64,
                   const char *, const char *, bool);
SH_DECL_HOOK6(IServerGameClients, ClientConnect, SH_NOATTRIB, 0, bool, CPlayerSlot, const char *, uint64, const char *,
              bool, CBufferString *);
SH_DECL_HOOK2(IGameEventManager2, FireEvent, SH_NOATTRIB, 0, bool, IGameEvent *, bool);

SH_DECL_HOOK2_void(IServerGameClients, ClientCommand, SH_NOATTRIB, 0, CPlayerSlot, const CCommand &);

SamplePlugin g_SamplePlugin;
IGameEventManager2 *gameevents = nullptr;

// Should only be called within the active game loop (i e map should be loaded
// and active) otherwise that'll be nullptr!
CGlobalVars *getGameGlobals()
{
    INetworkGameServer *server = g_pNetworkServerService->GetIGameServer();

    if (!server)
        return nullptr;

    return g_pNetworkServerService->GetIGameServer()->GetGlobals();
}

#if 0
// Currently unavailable, requires hl2sdk work!
ConVar sample_cvar("sample_cvar", "42", 0);
#endif

CON_COMMAND_F(sample_command, "Sample command", FCVAR_NONE)
{
    globals::callbackManager.PrintCallbackDebug();
}

PLUGIN_EXPOSE(SamplePlugin, g_SamplePlugin);
bool SamplePlugin::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
    PLUGIN_SAVEVARS();

    Log::Init();

    CSSHARP_CORE_INFO("Initializing");

    GET_V_IFACE_CURRENT(GetEngineFactory, globals::engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
    GET_V_IFACE_CURRENT(GetEngineFactory, globals::cvars, ICvar, CVAR_INTERFACE_VERSION);
    GET_V_IFACE_ANY(GetServerFactory, globals::server, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
    GET_V_IFACE_ANY(GetServerFactory, globals::serverGameClients, IServerGameClients,
                    INTERFACEVERSION_SERVERGAMECLIENTS);
    GET_V_IFACE_ANY(GetEngineFactory, globals::networkServerService, INetworkServerService,
                    NETWORKSERVERSERVICE_INTERFACE_VERSION);

    CSSHARP_CORE_INFO("Globals loaded.");

    CALL_GLOBAL_LISTENER(OnAllInitialized());

    if (!globals::dotnetManager.Initialize())
    {
        CSSHARP_CORE_ERROR("Failed to initialize .NET runtime");
        return false;
    }

    SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, globals::server, this, &SamplePlugin::Hook_GameFrame, true);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientActive, globals::serverGameClients, this,
                        &SamplePlugin::Hook_ClientActive, true);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, globals::serverGameClients, this,
                        &SamplePlugin::Hook_ClientDisconnect, true);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, globals::serverGameClients, this,
                        &SamplePlugin::Hook_ClientPutInServer, true);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, globals::serverGameClients, this,
                        &SamplePlugin::Hook_ClientSettingsChanged, false);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, OnClientConnected, globals::serverGameClients, this,
                        &SamplePlugin::Hook_OnClientConnected, false);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientConnect, globals::serverGameClients, this,
                        &SamplePlugin::Hook_ClientConnect, false);
    SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientCommand, globals::serverGameClients, this,
                        &SamplePlugin::Hook_ClientCommand, false);

    CSSHARP_CORE_INFO("Hooks added.");

    // Used by Metamod Console Commands
    g_pCVar = globals::cvars;
    ConVar_Register(FCVAR_RELEASE | FCVAR_CLIENT_CAN_EXECUTE | FCVAR_GAMEDLL);

    return true;
}

bool SamplePlugin::Unload(char *error, size_t maxlen)
{
    SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, globals::server, this, &SamplePlugin::Hook_GameFrame, true);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientActive, globals::serverGameClients, this,
                           &SamplePlugin::Hook_ClientActive, true);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, globals::serverGameClients, this,
                           &SamplePlugin::Hook_ClientDisconnect, true);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, globals::serverGameClients, this,
                           &SamplePlugin::Hook_ClientPutInServer, true);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, globals::serverGameClients, this,
                           &SamplePlugin::Hook_ClientSettingsChanged, false);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, OnClientConnected, globals::serverGameClients, this,
                           &SamplePlugin::Hook_OnClientConnected, false);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientConnect, globals::serverGameClients, this,
                           &SamplePlugin::Hook_ClientConnect, false);
    SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientCommand, globals::serverGameClients, this,
                           &SamplePlugin::Hook_ClientCommand, false);
    return true;
}

void SamplePlugin::AllPluginsLoaded()
{
    /* This is where we'd do stuff that relies on the mod or other plugins
     * being initialized (for example, cvars added and events registered).
     */
}

void SamplePlugin::Hook_ClientActive(CPlayerSlot slot, bool bLoadGame, const char *pszName, uint64 xuid)
{
    CSSHARP_CORE_INFO("Hook_ClientActive({0}, {1}, \"{2}\", {3})", slot.Get(), bLoadGame, pszName, xuid);
}

void SamplePlugin::Hook_ClientCommand(CPlayerSlot slot, const CCommand &args)
{
    CSSHARP_CORE_INFO("Hook_ClientCommand({0}, \"{1}\")", slot.Get(), args.GetCommandString());
}

void SamplePlugin::Hook_ClientSettingsChanged(CPlayerSlot slot)
{
    CSSHARP_CORE_INFO("Hook_ClientSettingsChanged({0})\n", slot.Get());
}

void SamplePlugin::Hook_OnClientConnected(CPlayerSlot slot, const char *pszName, uint64 xuid, const char *pszNetworkID,
                                          const char *pszAddress, bool bFakePlayer)
{
    CSSHARP_CORE_INFO("Hook_OnClientConnected({}, \"{}\", {}, \"{}\", \"{}\", {})\n", slot.Get(), pszName, xuid, pszNetworkID,
                   pszAddress, bFakePlayer);
}

bool SamplePlugin::Hook_ClientConnect(CPlayerSlot slot, const char *pszName, uint64 xuid, const char *pszNetworkID,
                                      bool unk1, CBufferString *pRejectReason)
{
    CSSHARP_CORE_INFO("Hook_ClientConnect({}, \"{}\", {}, \"{}\", {}, \"{}\")\n", slot.Get(), pszName, xuid, pszNetworkID, unk1,
                   pRejectReason->ToGrowable()->Get());

    RETURN_META_VALUE(MRES_IGNORED, true);
}

void SamplePlugin::Hook_ClientPutInServer(CPlayerSlot slot, char const *pszName, int type, uint64 xuid)
{
    CSSHARP_CORE_INFO("Hook_ClientPutInServer({}, \"{}\", {}, {}, {})\n", slot.Get(), pszName, type, xuid);
}

void SamplePlugin::Hook_ClientDisconnect(CPlayerSlot slot, int reason, const char *pszName, uint64 xuid,
                                         const char *pszNetworkID)
{
    CSSHARP_CORE_INFO("Hook_ClientDisconnect({}, {}, \"{}\", {}, \"{}\")\n", slot.Get(), reason, pszName, xuid, pszNetworkID);
}

void SamplePlugin::Hook_GameFrame(bool simulating, bool bFirstTick, bool bLastTick)
{
    /**
     * simulating:
     * ***********
     * true  | game is ticking
     * false | game is not ticking
     */
}

// Potentially might not work
static ScriptCallback *on_map_end_callback;
static bool NewLevelStarted = false;
void SamplePlugin::OnLevelInit(char const *pMapName, char const *pMapEntities, char const *pOldLevel,
                               char const *pLandmarkName, bool loadGame, bool background)
{
    CSSHARP_CORE_TRACE("name={0},mapname={1}", "LevelInit", pMapName);
    NewLevelStarted = true;

    if (!on_map_end_callback)
    {
        on_map_end_callback = globals::callbackManager.CreateCallback("OnMapEnd");
    }
}

void SamplePlugin::OnLevelShutdown()
{
    if (NewLevelStarted)
    {
        CALL_GLOBAL_LISTENER(OnLevelEnd());

        if (on_map_end_callback && on_map_end_callback->GetFunctionCount())
        {
            on_map_end_callback->ScriptContext().Reset();
            on_map_end_callback->Execute();
        }

        // globals::timer_system.RemoveMapChangeTimers();

        CSSHARP_CORE_TRACE("name={0}", "LevelShutdown");
        NewLevelStarted = false;
    }
}

bool SamplePlugin::Pause(char *error, size_t maxlen)
{
    return true;
}

bool SamplePlugin::Unpause(char *error, size_t maxlen)
{
    return true;
}

const char *SamplePlugin::GetLicense()
{
    return "Public Domain";
}

const char *SamplePlugin::GetVersion()
{
    return "1.0.0.0";
}

const char *SamplePlugin::GetDate()
{
    return __DATE__;
}

const char *SamplePlugin::GetLogTag()
{
    return "SAMPLE";
}

const char *SamplePlugin::GetAuthor()
{
    return "AlliedModders LLC";
}

const char *SamplePlugin::GetDescription()
{
    return "Sample basic plugin";
}

const char *SamplePlugin::GetName()
{
    return "Sample Plugin";
}

const char *SamplePlugin::GetURL()
{
    return "http://www.sourcemm.net/";
}
} // namespace counterstrikesharp