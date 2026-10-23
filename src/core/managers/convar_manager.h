#pragma once

#include <tier1/convar.h>

#include <vector>

#include "core/global_listener.h"

namespace counterstrikesharp {
class ScriptCallback;

class ConVarManager : public GlobalClass
{
  public:
    void OnAllInitialized() override;
    void OnShutdown() override;

  private:
    static void
    ConVarGlobalChanged(ConVarRefAbstract* ref, CSplitScreenSlot nSlot, const char* pNewValue, const char* pOldValue, void* __unk01);
    void DispatchConVarChanged(ConVarRefAbstract* ref);
    void FlushConvarChangedQueue();

    static thread_local int s_convar_dispatch_depth;
    static thread_local std::vector<uint16> s_pending_convar_changes;
    ScriptCallback* m_on_convar_changed_callback = nullptr;
};
} // namespace counterstrikesharp
