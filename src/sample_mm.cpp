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

#include "core/log.h"
#include "core/utils.h"
#include "iserver.h"

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
    CSSHARP_CORE_INFO("Sample command called by {0}. Command: {1}", context.GetPlayerSlot().Get(),
                      utils::PluginDirectory().c_str());
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
    META_CONPRINTF("Hook_ClientActive(%d, %d, \"%s\", %d)\n", slot, bLoadGame, pszName, xuid);
}

void SamplePlugin::Hook_ClientCommand(CPlayerSlot slot, const CCommand &args)
{
    META_CONPRINTF("Hook_ClientCommand(%d, \"%s\")\n", slot, args.GetCommandString());
}

void SamplePlugin::Hook_ClientSettingsChanged(CPlayerSlot slot)
{
    META_CONPRINTF("Hook_ClientSettingsChanged(%d)\n", slot);
}

void SamplePlugin::Hook_OnClientConnected(CPlayerSlot slot, const char *pszName, uint64 xuid, const char *pszNetworkID,
                                          const char *pszAddress, bool bFakePlayer)
{
    META_CONPRINTF("Hook_OnClientConnected(%d, \"%s\", %d, \"%s\", \"%s\", %d)\n", slot, pszName, xuid, pszNetworkID,
                   pszAddress, bFakePlayer);
}

bool SamplePlugin::Hook_ClientConnect(CPlayerSlot slot, const char *pszName, uint64 xuid, const char *pszNetworkID,
                                      bool unk1, CBufferString *pRejectReason)
{
    META_CONPRINTF("Hook_ClientConnect(%d, \"%s\", %d, \"%s\", %d, \"%s\")\n", slot, pszName, xuid, pszNetworkID, unk1,
                   pRejectReason->ToGrowable()->Get());

    RETURN_META_VALUE(MRES_IGNORED, true);
}

void SamplePlugin::Hook_ClientPutInServer(CPlayerSlot slot, char const *pszName, int type, uint64 xuid)
{
    META_CONPRINTF("Hook_ClientPutInServer(%d, \"%s\", %d, %d, %d)\n", slot, pszName, type, xuid);
}

void SamplePlugin::Hook_ClientDisconnect(CPlayerSlot slot, int reason, const char *pszName, uint64 xuid,
                                         const char *pszNetworkID)
{
    META_CONPRINTF("Hook_ClientDisconnect(%d, %d, \"%s\", %d, \"%s\")\n", slot, reason, pszName, xuid, pszNetworkID);
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
void SamplePlugin::OnLevelInit(char const *pMapName, char const *pMapEntities, char const *pOldLevel,
                               char const *pLandmarkName, bool loadGame, bool background)
{
    META_CONPRINTF("OnLevelInit(%s)\n", pMapName);
}

// Potentially might not work
void SamplePlugin::OnLevelShutdown()
{
    META_CONPRINTF("OnLevelShutdown()\n");
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