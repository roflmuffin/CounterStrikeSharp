#include "core/verified_cvar.h"

#include <tier0/icvar.h>
#include <tier0/convar.h>

#include <cstring>
#include <optional>
#include <utility>

#include "core/core_bindings.h"
#include "core/slot_calls.h"

namespace counterstrikesharp::bindings {

#ifdef _WIN32

ICvar* VerifiedCvar(ICvar* engine) { return engine; }

#else

namespace {

class SlotForwardingCvar final : public ICvar
{
  public:
    explicit SlotForwardingCvar(ICvar* engine) : m_engine(engine) { CopySplitScreenSlots(); }

    // --- IAppSystem: never called by CounterStrikeSharp; compiled calls ---
    bool Connect(CreateInterfaceFn factory) override { return m_engine->Connect(factory); }
    void Disconnect() override { m_engine->Disconnect(); }
    void* QueryInterface(const char* name) override { return m_engine->QueryInterface(name); }
    InitReturnVal_t Init() override { return m_engine->Init(); }
    void Shutdown() override { m_engine->Shutdown(); }
    void PreShutdown() override { m_engine->PreShutdown(); }
    const AppSystemInfo_t* GetDependencies() override { return m_engine->GetDependencies(); }
    AppSystemTier_t GetTier() override { return m_engine->GetTier(); }
    void Reconnect(CreateInterfaceFn factory, const char* name) override { m_engine->Reconnect(factory, name); }
    bool IsSingleton() override { return m_engine->IsSingleton(); }
    BuildType_t GetBuildType() override { return m_engine->GetBuildType(); }

    // --- ICvar, in vtable order. Slot Bindings where CounterStrikeSharp calls them; compiled calls otherwise ---
    ConVarRef FindConVar(const char* name, bool allowDefensive) override
    {
        return Forward(m_findConVar, &ICvar::FindConVar, ConVarRef(), name, allowDefensive);
    }
    ConVarRef FindFirstConVar() override { return m_engine->FindFirstConVar(); }
    ConVarRef FindNextConVar(ConVarRef previous) override { return m_engine->FindNextConVar(previous); }
    void CallChangeCallback(ConVarRef cvar, const CSplitScreenSlot slot, const CVValue_t* newValue, const CVValue_t* oldValue,
                            ConVarUserInfoSet_t* userInfo) override
    {
        ForwardVoid(m_callChangeCallback, &ICvar::CallChangeCallback, cvar, slot, newValue, oldValue, userInfo);
    }
    void IterateConVarCallbacks(ConVarRef cvar, FnCvarCallbacksReader_t callback) override
    {
        m_engine->IterateConVarCallbacks(cvar, std::move(callback));
    }
    // An unusable filter refuses the change: the value is not modified.
    bool CallFilterCallback(ConVarRef cvar, const CSplitScreenSlot slot, const CVValue_t* newValue, const CVValue_t* oldValue,
                            ConVarUserInfoSet_t* userInfo) override
    {
        return Forward(m_callFilterCallback, &ICvar::CallFilterCallback, false, cvar, slot, newValue, oldValue, userInfo);
    }

    ConCommandRef FindConCommand(const char* name, bool allowDefensive) override
    {
        return Forward(m_findConCommand, &ICvar::FindConCommand, ConCommandRef(), name, allowDefensive);
    }
    ConCommandRef FindFirstConCommand() override { return m_engine->FindFirstConCommand(); }
    ConCommandRef FindNextConCommand(ConCommandRef previous) override { return m_engine->FindNextConCommand(previous); }
    void DispatchConCommand(ConCommandRef command, const CCommandContext& context, const CCommand& args) override
    {
        ForwardVoid(m_dispatchConCommand, &ICvar::DispatchConCommand, command, context, args);
    }

    void InstallGlobalChangeCallback(FnChangeCallbackGlobal_t callback) override
    {
        ForwardVoid(m_installGlobalChangeCallback, &ICvar::InstallGlobalChangeCallback, callback);
    }
    void RemoveGlobalChangeCallback(FnChangeCallbackGlobal_t callback) override
    {
        ForwardVoid(m_removeGlobalChangeCallback, &ICvar::RemoveGlobalChangeCallback, callback);
    }
    void CallGlobalChangeCallbacks(ConVarRefAbstract* ref, CSplitScreenSlot slot, const char* newValue, const char* oldValue,
                                   ConVarUserInfoSet_t* userInfo) override
    {
        ForwardVoid(m_callGlobalChangeCallbacks, &ICvar::CallGlobalChangeCallbacks, ref, slot, newValue, oldValue, userInfo);
    }

    void ResetConVarsToDefaultValuesByFlag(uint64 flag) override { m_engine->ResetConVarsToDefaultValuesByFlag(flag); }
    void SetMaxSplitScreenSlots(int slots) override
    {
        m_engine->SetMaxSplitScreenSlots(slots);
        CopySplitScreenSlots();
    }
    void RegisterCreationListeners(IConVarListener* callbacks) override { m_engine->RegisterCreationListeners(callbacks); }
    void RemoveCreationListeners(IConVarListener* callbacks) override { m_engine->RemoveCreationListeners(callbacks); }
    void ResetConVarsToDefaultValuesByName(const char* prefix) override { m_engine->ResetConVarsToDefaultValuesByName(prefix); }
    ConVarSnapshot_t* TakeConVarSnapshot() override { return m_engine->TakeConVarSnapshot(); }
    void ResetConVarsToSnapshot(ConVarSnapshot_t* snapshot) override { m_engine->ResetConVarsToSnapshot(snapshot); }
    void DestroyConVarSnapshot(ConVarSnapshot_t* snapshot) override { m_engine->DestroyConVarSnapshot(snapshot); }

    // An unusable character set tokenizes on whitespace only (no break characters) rather than crash.
    characterset_t* GetCharacterSet() override
    {
        static characterset_t noBreakCharacters{};
        return Forward(m_getCharacterSet, &ICvar::GetCharacterSet, &noBreakCharacters);
    }
    void SetConVarsFromGameInfo(KeyValues* keyValues) override { m_engine->SetConVarsFromGameInfo(keyValues); }
    void StripDevelopmentFlags() override { m_engine->StripDevelopmentFlags(); }
    int GetTotalUserInfoCvarsByteSize() override { return m_engine->GetTotalUserInfoCvarsByteSize(); }
    void CopyUserInfoCvarDefaults(ConVarUserInfoSet_t buffer, int from, int to, bool copyOrCleanup) override
    {
        m_engine->CopyUserInfoCvarDefaults(buffer, from, to, copyOrCleanup);
    }
    void GetCompletionResults(const CCommand& command, CUtlVector<CUtlString>& completions, bool* successful) override
    {
        m_engine->GetCompletionResults(command, completions, successful);
    }

    void RegisterConVar(const ConVarCreation_t& setup, uint64 additionalFlags, ConVarRef* ref, ConVarData** data) override
    {
        ForwardVoid(m_registerConVar, &ICvar::RegisterConVar, setup, additionalFlags, ref, data);
    }
    void UnregisterConVarCallbacks(ConVarRef cvar) override
    {
        ForwardVoid(m_unregisterConVarCallbacks, &ICvar::UnregisterConVarCallbacks, cvar);
    }
    void LockConVarValueInitialisation(bool state) override { m_engine->LockConVarValueInitialisation(state); }
    ConVarData* GetConVarData(ConVarRef cvar) override
    {
        return Forward(m_getConVarData, &ICvar::GetConVarData, static_cast<ConVarData*>(nullptr), cvar);
    }

    ConCommandRef RegisterConCommand(const ConCommandCreation_t& setup, uint64 additionalFlags) override
    {
        return Forward(m_registerConCommand, &ICvar::RegisterConCommand, ConCommandRef(), setup, additionalFlags);
    }
    void UnregisterConCommandCallbacks(ConCommandRef command) override
    {
        ForwardVoid(m_unregisterConCommandCallbacks, &ICvar::UnregisterConCommandCallbacks, command);
    }
    ConCommandData* GetConCommandData(ConCommandRef command) override
    {
        return Forward(m_getConCommandData, &ICvar::GetConCommandData, static_cast<ConCommandData*>(nullptr), command);
    }

    void QueueThreadSetValue(ConVarRefAbstract* ref, CSplitScreenSlot slot, ConVarUserInfoSet_t* userInfo, CVValue_t* value) override
    {
        ForwardVoid(m_queueThreadSetValue, &ICvar::QueueThreadSetValue, ref, slot, userInfo, value);
    }

  private:
    template <class F, class R, class... Args> R Forward(const std::optional<int>& slot, F method, R unusable, Args&&... args)
    {
        if (!slot) return unusable;
        return CallSlot(slot, method, m_engine, std::forward<Args>(args)...);
    }
    template <class F, class... Args> void ForwardVoid(const std::optional<int>& slot, F method, Args&&... args)
    {
        if (slot) CallSlot(slot, method, m_engine, std::forward<Args>(args)...);
    }

    // ICvar's only data member (int m_MaxSplitScreenSlots), read by the inline GetMaxSplitScreenSlots; private to ICvar,
    // so copied as the int after the vtable pointer.
    void CopySplitScreenSlots()
    {
        static_assert(sizeof(ICvar) >= sizeof(void*) + sizeof(int));
        std::memcpy(reinterpret_cast<char*>(static_cast<ICvar*>(this)) + sizeof(void*),
                    reinterpret_cast<const char*>(m_engine) + sizeof(void*), sizeof(int));
    }

    ICvar* m_engine;
    // The verified (or healed) slot of each Slot Binding; nullopt if it failed.
    std::optional<int> m_findConVar = GetBindingSlot("ICvar::FindConVar");
    std::optional<int> m_callChangeCallback = GetBindingSlot("ICvar::CallChangeCallback");
    std::optional<int> m_callFilterCallback = GetBindingSlot("ICvar::CallFilterCallback");
    std::optional<int> m_findConCommand = GetBindingSlot("ICvar::FindConCommand");
    std::optional<int> m_dispatchConCommand = GetBindingSlot("ICvar::DispatchConCommand");
    std::optional<int> m_installGlobalChangeCallback = GetBindingSlot("ICvar::InstallGlobalChangeCallback");
    std::optional<int> m_removeGlobalChangeCallback = GetBindingSlot("ICvar::RemoveGlobalChangeCallback");
    std::optional<int> m_callGlobalChangeCallbacks = GetBindingSlot("ICvar::CallGlobalChangeCallbacks");
    std::optional<int> m_getCharacterSet = GetBindingSlot("ICvar::GetCharacterSet");
    std::optional<int> m_registerConVar = GetBindingSlot("ICvar::RegisterConVar");
    std::optional<int> m_unregisterConVarCallbacks = GetBindingSlot("ICvar::UnregisterConVarCallbacks");
    std::optional<int> m_getConVarData = GetBindingSlot("ICvar::GetConVarData");
    std::optional<int> m_registerConCommand = GetBindingSlot("ICvar::RegisterConCommand");
    std::optional<int> m_unregisterConCommandCallbacks = GetBindingSlot("ICvar::UnregisterConCommandCallbacks");
    std::optional<int> m_getConCommandData = GetBindingSlot("ICvar::GetConCommandData");
    std::optional<int> m_queueThreadSetValue = GetBindingSlot("ICvar::QueueThreadSetValue");
};

std::optional<SlotForwardingCvar> verifiedCvar;

} // namespace

ICvar* VerifiedCvar(ICvar* engine)
{
    if (engine == nullptr) return nullptr;
    verifiedCvar.emplace(engine);
    return &*verifiedCvar;
}

#endif

} // namespace counterstrikesharp::bindings
