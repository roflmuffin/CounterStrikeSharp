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

#include "mm_plugin.h"

#include <cstdio>

#include "core/global_listener.h"
#include "core/log.h"
#include "core/timer_system.h"
#include "core/utils.h"
#include "igameeventsystem.h"
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

CounterStrikeSharpMMPlugin g_SamplePlugin;

#if 0
// Currently unavailable, requires hl2sdk work!
ConVar sample_cvar("sample_cvar", "42", 0);
#endif

CON_COMMAND_F(sample_command, "Sample command", FCVAR_NONE)
{
    globals::callbackManager.PrintCallbackDebug();
}

PLUGIN_EXPOSE(CounterStrikeSharpMMPlugin, g_SamplePlugin);
bool CounterStrikeSharpMMPlugin::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
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
    GET_V_IFACE_ANY(GetEngineFactory, globals::gameEventSystem, IGameEventSystem, GAMEEVENTSYSTEM_INTERFACE_VERSION);

    CSSHARP_CORE_INFO("Globals loaded.");

    CALL_GLOBAL_LISTENER(OnAllInitialized());

    if (!globals::dotnetManager.Initialize())
    {
        CSSHARP_CORE_ERROR("Failed to initialize .NET runtime");
        return false;
    }

    SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, globals::server, this, &CounterStrikeSharpMMPlugin::Hook_GameFrame, true);

    CSSHARP_CORE_INFO("Hooks added.");

    // Used by Metamod Console Commands
    g_pCVar = globals::cvars;
    ConVar_Register(FCVAR_RELEASE | FCVAR_CLIENT_CAN_EXECUTE | FCVAR_GAMEDLL);

    return true;
}

bool CounterStrikeSharpMMPlugin::Unload(char *error, size_t maxlen)
{
    SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, globals::server, this, &CounterStrikeSharpMMPlugin::Hook_GameFrame, true);

    return true;
}

void CounterStrikeSharpMMPlugin::AllPluginsLoaded()
{
    /* This is where we'd do stuff that relies on the mod or other plugins
     * being initialized (for example, cvars added and events registered).
     */
}

void CounterStrikeSharpMMPlugin::Hook_ClientActive(CPlayerSlot slot, bool bLoadGame, const char *pszName, uint64 xuid)
{
    CSSHARP_CORE_INFO("Hook_ClientActive({0}, {1}, \"{2}\", {3})", slot.Get(), bLoadGame, pszName, xuid);
}

void CounterStrikeSharpMMPlugin::Hook_ClientCommand(CPlayerSlot slot, const CCommand &args)
{
    CSSHARP_CORE_INFO("Hook_ClientCommand({0}, \"{1}\")", slot.Get(), args.GetCommandString());
}

void CounterStrikeSharpMMPlugin::Hook_ClientSettingsChanged(CPlayerSlot slot)
{
    CSSHARP_CORE_INFO("Hook_ClientSettingsChanged({0})\n", slot.Get());
}

void CounterStrikeSharpMMPlugin::Hook_OnClientConnected(CPlayerSlot slot, const char *pszName, uint64 xuid,
                                                        const char *pszNetworkID, const char *pszAddress,
                                                        bool bFakePlayer)
{
    CSSHARP_CORE_INFO("Hook_OnClientConnected({}, \"{}\", {}, \"{}\", \"{}\", {})\n", slot.Get(), pszName, xuid,
                      pszNetworkID, pszAddress, bFakePlayer);
}

bool CounterStrikeSharpMMPlugin::Hook_ClientConnect(CPlayerSlot slot, const char *pszName, uint64 xuid,
                                                    const char *pszNetworkID, bool unk1, CBufferString *pRejectReason)
{
    CSSHARP_CORE_INFO("Hook_ClientConnect({}, \"{}\", {}, \"{}\", {}, \"{}\")\n", slot.Get(), pszName, xuid,
                      pszNetworkID, unk1, pRejectReason->ToGrowable()->Get());

    RETURN_META_VALUE(MRES_IGNORED, true);
}

void CounterStrikeSharpMMPlugin::Hook_ClientPutInServer(CPlayerSlot slot, char const *pszName, int type, uint64 xuid)
{
    CSSHARP_CORE_INFO("Hook_ClientPutInServer({}, \"{}\", {}, {}, {})\n", slot.Get(), pszName, type, xuid);
}

void CounterStrikeSharpMMPlugin::Hook_ClientDisconnect(CPlayerSlot slot, int reason, const char *pszName, uint64 xuid,
                                                       const char *pszNetworkID)
{
    CSSHARP_CORE_INFO("Hook_ClientDisconnect({}, {}, \"{}\", {}, \"{}\")\n", slot.Get(), reason, pszName, xuid,
                      pszNetworkID);
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
}

// Potentially might not work
static ScriptCallback *on_map_end_callback;
static bool NewLevelStarted = false;
void CounterStrikeSharpMMPlugin::OnLevelInit(char const *pMapName, char const *pMapEntities, char const *pOldLevel,
                                             char const *pLandmarkName, bool loadGame, bool background)
{
    CSSHARP_CORE_TRACE("name={0},mapname={1}", "LevelInit", pMapName);
    NewLevelStarted = true;

    if (!on_map_end_callback)
    {
        on_map_end_callback = globals::callbackManager.CreateCallback("OnMapEnd");
    }
}

void CounterStrikeSharpMMPlugin::OnLevelShutdown()
{
    if (NewLevelStarted)
    {
        CALL_GLOBAL_LISTENER(OnLevelEnd());

        if (on_map_end_callback && on_map_end_callback->GetFunctionCount())
        {
            on_map_end_callback->ScriptContext().Reset();
            on_map_end_callback->Execute();
        }

        globals::timerSystem.RemoveMapChangeTimers();

        CSSHARP_CORE_TRACE("name={0}", "LevelShutdown");
        NewLevelStarted = false;
    }
}

bool CounterStrikeSharpMMPlugin::Pause(char *error, size_t maxlen)
{
    return true;
}

bool CounterStrikeSharpMMPlugin::Unpause(char *error, size_t maxlen)
{
    return true;
}

const char *CounterStrikeSharpMMPlugin::GetLicense()
{
    return "GNU GPLv3";
}

const char *CounterStrikeSharpMMPlugin::GetVersion()
{
    return "0.1.0";
}

const char *CounterStrikeSharpMMPlugin::GetDate()
{
    return __DATE__;
}

const char *CounterStrikeSharpMMPlugin::GetLogTag()
{
    return "CSSHARP";
}

const char *CounterStrikeSharpMMPlugin::GetAuthor()
{
    return "Roflmuffin";
}

const char *CounterStrikeSharpMMPlugin::GetDescription()
{
    return "Counter Strike .NET Scripting Runtime";
}

const char *CounterStrikeSharpMMPlugin::GetName()
{
    return "CounterStrikeSharp";
}

const char *CounterStrikeSharpMMPlugin::GetURL()
{
    return "https://github.com/roflmuffin/CounterStrikeSharp";
}
} // namespace counterstrikesharp