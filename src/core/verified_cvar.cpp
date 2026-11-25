#include "core/verified_cvar.h"

#include <tier0/icvar.h>
#include <tier0/convar.h>

#include <cstring>
#include <mutex>
#include <optional>
#include <set>
#include <string_view>
#include <type_traits>
#include <utility>

#include "core/core_bindings.h"
#include "core/log.h"
#include "core/slot_calls.h"

namespace counterstrikesharp::bindings {

#ifdef _WIN32

ICvar* VerifiedCvar(ICvar* engine) { return engine; }

#else

namespace {

void ReportNotASlotBinding(const char* method)
{
    static std::mutex mutex;
    static std::set<std::string_view> reported;
    std::lock_guard guard(mutex);
    if (!reported.insert(method).second) return;
    CSSHARP_CORE_ERROR("ICvar::{} was called, but it is not a Slot Binding (no verified slot index); not called", method);
}

class SlotForwardingCvar final : public ICvar
{
  public:
    explicit SlotForwardingCvar(ICvar* engine) : m_engine(engine) { CopySplitScreenSlots(); }

    // --- IAppSystem: never called by CounterStrikeSharp; not Slot Bindings ---
    bool Connect(CreateInterfaceFn) override { return NotASlotBinding<bool>("Connect"); }
    void Disconnect() override { NotASlotBinding("Disconnect"); }
    void* QueryInterface(const char*) override { return NotASlotBinding<void*>("QueryInterface"); }
    InitReturnVal_t Init() override { return NotASlotBinding<InitReturnVal_t>("Init"); }
    void Shutdown() override { NotASlotBinding("Shutdown"); }
    void PreShutdown() override { NotASlotBinding("PreShutdown"); }
    const AppSystemInfo_t* GetDependencies() override { return NotASlotBinding<const AppSystemInfo_t*>("GetDependencies"); }
    AppSystemTier_t GetTier() override { return NotASlotBinding<AppSystemTier_t>("GetTier"); }
    void Reconnect(CreateInterfaceFn, const char*) override { NotASlotBinding("Reconnect"); }
    bool IsSingleton() override { return NotASlotBinding<bool>("IsSingleton"); }
    BuildType_t GetBuildType() override { return NotASlotBinding<BuildType_t>("GetBuildType"); }

    // --- ICvar, in vtable order. Slot Bindings where CounterStrikeSharp calls them; refused otherwise ---
    ConVarRef FindConVar(const char* name, bool allowDefensive) override
    {
        return Forward(m_findConVar, &ICvar::FindConVar, ConVarRef(), name, allowDefensive);
    }
    ConVarRef FindFirstConVar() override { return NotASlotBinding<ConVarRef>("FindFirstConVar"); }
    ConVarRef FindNextConVar(ConVarRef) override { return NotASlotBinding<ConVarRef>("FindNextConVar"); }
    void CallChangeCallback(ConVarRef cvar, const CSplitScreenSlot slot, const CVValue_t* newValue, const CVValue_t* oldValue,
                            ConVarUserInfoSet_t* userInfo) override
    {
        ForwardVoid(m_callChangeCallback, &ICvar::CallChangeCallback, cvar, slot, newValue, oldValue, userInfo);
    }
    void IterateConVarCallbacks(ConVarRef, FnCvarCallbacksReader_t) override { NotASlotBinding("IterateConVarCallbacks"); }
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
    ConCommandRef FindFirstConCommand() override { return NotASlotBinding<ConCommandRef>("FindFirstConCommand"); }
    ConCommandRef FindNextConCommand(ConCommandRef) override { return NotASlotBinding<ConCommandRef>("FindNextConCommand"); }
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

    void ResetConVarsToDefaultValuesByFlag(uint64) override { NotASlotBinding("ResetConVarsToDefaultValuesByFlag"); }
    void SetMaxSplitScreenSlots(int) override { NotASlotBinding("SetMaxSplitScreenSlots"); }
    void RegisterCreationListeners(IConVarListener*) override { NotASlotBinding("RegisterCreationListeners"); }
    void RemoveCreationListeners(IConVarListener*) override { NotASlotBinding("RemoveCreationListeners"); }
    void ResetConVarsToDefaultValuesByName(const char*) override { NotASlotBinding("ResetConVarsToDefaultValuesByName"); }
    ConVarSnapshot_t* TakeConVarSnapshot() override { return NotASlotBinding<ConVarSnapshot_t*>("TakeConVarSnapshot"); }
    void ResetConVarsToSnapshot(ConVarSnapshot_t*) override { NotASlotBinding("ResetConVarsToSnapshot"); }
    void DestroyConVarSnapshot(ConVarSnapshot_t*) override { NotASlotBinding("DestroyConVarSnapshot"); }

    // An unusable character set tokenizes on whitespace only (no break characters) rather than crash.
    characterset_t* GetCharacterSet() override
    {
        static characterset_t noBreakCharacters{};
        return Forward(m_getCharacterSet, &ICvar::GetCharacterSet, &noBreakCharacters);
    }
    void SetConVarsFromGameInfo(KeyValues*) override { NotASlotBinding("SetConVarsFromGameInfo"); }
    void StripDevelopmentFlags() override { NotASlotBinding("StripDevelopmentFlags"); }
    int GetTotalUserInfoCvarsByteSize() override { return NotASlotBinding<int>("GetTotalUserInfoCvarsByteSize"); }
    void CopyUserInfoCvarDefaults(ConVarUserInfoSet_t, int, int, bool) override { NotASlotBinding("CopyUserInfoCvarDefaults"); }
    void GetCompletionResults(const CCommand&, CUtlVector<CUtlString>&, bool*) override { NotASlotBinding("GetCompletionResults"); }

    void RegisterConVar(const ConVarCreation_t& setup, uint64 additionalFlags, ConVarRef* ref, ConVarData** data) override
    {
        ForwardVoid(m_registerConVar, &ICvar::RegisterConVar, setup, additionalFlags, ref, data);
    }
    void UnregisterConVarCallbacks(ConVarRef cvar) override
    {
        ForwardVoid(m_unregisterConVarCallbacks, &ICvar::UnregisterConVarCallbacks, cvar);
    }
    void LockConVarValueInitialisation(bool) override { NotASlotBinding("LockConVarValueInitialisation"); }
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
    // A method CounterStrikeSharp never calls has no Slot Binding, so nothing verified a slot index for it: it is
    // refused (logged once, empty value) instead of called at its compiled header position. A new caller adds the
    // method as a Slot Binding first.
    template <class R = void> R NotASlotBinding(const char* method)
    {
        ReportNotASlotBinding(method);
        if constexpr (!std::is_void_v<R>) return R{};
    }

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
