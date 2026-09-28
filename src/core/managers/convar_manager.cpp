#include "core/managers/convar_manager.h"

#include <icvar.h>

#include "convar.h"
#include "core/globals.h"
#include "scripting/callback_manager.h"

namespace counterstrikesharp {
thread_local int ConVarManager::s_convar_dispatch_depth = 0;
thread_local std::vector<uint16> ConVarManager::s_pending_convar_changes;

void ConVarManager::FlushConvarChangedQueue()
{
    while (!s_pending_convar_changes.empty())
    {
        const uint16 accessIndex = s_pending_convar_changes.front();
        s_pending_convar_changes.erase(s_pending_convar_changes.begin());

        ConVarRefAbstract ref(accessIndex);
        if (!ref.IsValidRef() || !ref.IsConVarDataValid() || !ref.IsConVarDataAvailable()) continue;

        m_on_convar_changed_callback->ScriptContext().Reset();
        m_on_convar_changed_callback->ScriptContext().Push(accessIndex);
        m_on_convar_changed_callback->Execute();
    }
}

void ConVarManager::OnAllInitialized()
{
    if (!globals::cvars || m_on_convar_changed_callback) return;

    m_on_convar_changed_callback = globals::callbackManager.CreateCallback("OnConVarChanged");
    globals::cvars->InstallGlobalChangeCallback(&ConVarManager::ConVarGlobalChanged);
}

void ConVarManager::OnShutdown()
{
    if (globals::cvars && m_on_convar_changed_callback)
    {
        globals::cvars->RemoveGlobalChangeCallback(&ConVarManager::ConVarGlobalChanged);
    }

    if (m_on_convar_changed_callback)
    {
        globals::callbackManager.ReleaseCallback(m_on_convar_changed_callback);
        m_on_convar_changed_callback = nullptr;
    }
}

void ConVarManager::ConVarGlobalChanged(ConVarRefAbstract* ref, CSplitScreenSlot, const char*, const char*, void*)
{
    globals::convarManager.DispatchConVarChanged(ref);
}

void ConVarManager::DispatchConVarChanged(ConVarRefAbstract* ref)
{
    if (!m_on_convar_changed_callback || !ref) return;

    s_pending_convar_changes.push_back(ref->GetAccessIndex());

    if (s_convar_dispatch_depth > 0) return;

    ++s_convar_dispatch_depth;
    try
    {
        FlushConvarChangedQueue();
    }
    catch (...)
    {
        --s_convar_dispatch_depth;
        throw;
    }
    --s_convar_dispatch_depth;
}
} // namespace counterstrikesharp
