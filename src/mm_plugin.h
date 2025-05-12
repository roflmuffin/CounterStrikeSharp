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
#include <iserver.h>
#include <igameevents.h>
#include <iplayerinfo.h>
#include <sh_vector.h>
#include <vector>
#include "entitysystem.h"
#include "concurrentqueue.h"

namespace counterstrikesharp {
class ScriptCallback;

class CounterStrikeSharpMMPlugin : public ISmmPlugin, public IMetamodListener
{
  public:
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
    void Hook_GameFrame(bool simulating, bool bFirstTick, bool bLastTick);
    void Hook_StartupServer(const GameSessionConfiguration_t& config, ISource2WorldSession*, const char*);
    void AddTaskForNextFrame(std::function<void()>&& task);

    void Hook_RegisterLoopMode(const char* pszLoopModeName, ILoopModeFactory* pLoopModeFactory, void** ppGlobalPointer);
    IEngineService* Hook_FindService(const char* serviceName);

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
    moodycamel::ConcurrentQueue<std::function<void()>> m_nextTasks;
};

static ScriptCallback* on_activate_callback;
static ScriptCallback* on_metamod_all_plugins_loaded_callback;
extern CounterStrikeSharpMMPlugin gPlugin;

#endif //_INCLUDE_METAMOD_SOURCE_STUB_PLUGIN_H_
}

PLUGIN_GLOBALVARS();
