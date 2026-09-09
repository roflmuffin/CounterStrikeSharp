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

#define VPROF_LEVEL 1

#ifndef _INCLUDE_METAMOD_SOURCE_STUB_PLUGIN_H_
#define _INCLUDE_METAMOD_SOURCE_STUB_PLUGIN_H_

#include <ISmmPlugin.h>
#include <functional>
#include <eiface.h>
#include <engine/IEngineService.h>
#include <iserver.h>
#include <igameevents.h>
#include <iplayerinfo.h>
#include <vector>
#include "entitysystem.h"

namespace counterstrikesharp {
class ScriptCallback;

class CounterStrikeSharpMMPlugin : public ISmmPlugin, public IMetamodListener
{
  public:
    CounterStrikeSharpMMPlugin();

    bool Load(PluginId id, ISmmAPI* ismm, char* error, size_t maxlen, bool late) override;
    bool Unload(char* error, size_t maxlen) override;
    bool Pause(char* error, size_t maxlen) override;
    bool Unpause(char* error, size_t maxlen) override;
    void AllPluginsLoaded() override;

  public: // hooks
    void OnLevelInit(char const* pMapName,
                     char const* pMapEntities,
                     char const* pOldLevel,
                     char const* pLandmarkName,
                     bool loadGame,
                     bool background) override;
    void OnLevelShutdown() override;
    KHook::Return<void> Hook_GameFrame(IServerGameDLL*, bool simulating, bool bFirstTick, bool bLastTick);
    KHook::Return<void>
    Hook_StartupServer(INetworkServerService*, const GameSessionConfiguration_t& config, ISource2WorldSession*, const char*);

    KHook::Return<void>
    Hook_RegisterLoopMode(IEngineServiceMgr*, const char* pszLoopModeName, ILoopModeFactory* pLoopModeFactory, void** ppGlobalPointer);
    KHook::Return<int> Hook_LoadEventsFromFile(IGameEventManager2*, const char* filename, bool bSearchAll);
    KHook::Return<IEngineService*> Hook_FindService(IEngineServiceMgr*, const char* serviceName);

  public:
    const char* GetAuthor() override;
    const char* GetName() override;
    const char* GetDescription() override;
    const char* GetURL() override;
    const char* GetLicense() override;
    const char* GetVersion() override;
    const char* GetDate() override;
    const char* GetLogTag() override;

  private:
    bool m_has_level_initialized = false;

    KHook::Virtual<IServerGameDLL, void, bool, bool, bool> m_GameFrame;
    KHook::Virtual<INetworkServerService, void, const GameSessionConfiguration_t&, ISource2WorldSession*, const char*> m_StartupServer;
    KHook::Virtual<IEngineServiceMgr, void, const char*, ILoopModeFactory*, void**> m_RegisterLoopMode;
    KHook::Virtual<IEngineServiceMgr, IEngineService*, const char*> m_FindService;
    KHook::Virtual<IGameEventManager2, int, const char*, bool> m_LoadEventsFromFile;
};

static ScriptCallback* on_activate_callback;
static ScriptCallback* on_map_end_callback;
static ScriptCallback* on_metamod_all_plugins_loaded_callback;
extern CounterStrikeSharpMMPlugin gPlugin;

#endif //_INCLUDE_METAMOD_SOURCE_STUB_PLUGIN_H_
}

PLUGIN_GLOBALVARS();
