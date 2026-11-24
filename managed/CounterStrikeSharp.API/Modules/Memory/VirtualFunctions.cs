using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CounterStrikeSharp.API.Modules.Memory;

/// <summary>
/// Core memory functions. Each member binds lazily on first use (call, hook or <c>Handle</c>), so a failed Binding or a
/// missing Gamedata key only affects its own members. Calling a member whose Engine Feature is disabled throws
/// <see cref="EngineFeatureUnavailableException"/>; hooking one registers, logs once and never fires.
/// </summary>
public static class VirtualFunctions
{
    public static MemoryFunctionVoid<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintFunc =
        new(CoreFunction.FromEngineFeature(ManagedEngineFeatures.ChatClientPrint, "ClientPrint"));

    public static Action<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrint =
        ClientPrintFunc.Invoke;

    public static MemoryFunctionVoid<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintAllFunc =
        new(CoreFunction.FromEngineFeature(ManagedEngineFeatures.ChatClientPrint, "UTIL_ClientPrintAll"));

    public static Action<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintAll =
        ClientPrintAllFunc.Invoke;

    // void (*FnGiveNamedItem)(void* itemService,const char* pchName, void* iSubType,void* pScriptItem, void* a5,void* a6) = nullptr;
    public static MemoryFunctionWithReturn<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> GiveNamedItemFunc =
        new(CoreFunction.FromGamedata("GiveNamedItem"));

    public static Func<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> GiveNamedItem = GiveNamedItemFunc.Invoke;

    public static MemoryFunctionVoid<IntPtr, byte> SwitchTeamFunc =
        new(CoreFunction.FromGamedata("CCSPlayerController_SwitchTeam"));

    public static Action<IntPtr, byte> SwitchTeam = SwitchTeamFunc.Invoke;

    // void(*UTIL_Remove)(CEntityInstance*);
    public static MemoryFunctionVoid<IntPtr> UTIL_RemoveFunc =
        new(CoreFunction.FromGamedata("UTIL_Remove"));

    public static Action<IntPtr> UTIL_Remove = UTIL_RemoveFunc.Invoke;

    // void(*CBaseModelEntity_SetModel)(CBaseModelEntity*, const char*);
    public static MemoryFunctionVoid<IntPtr, string> SetModelFunc =
        new(CoreFunction.FromGamedata("CBaseModelEntity_SetModel"));

    public static Action<IntPtr, string> SetModel = SetModelFunc.Invoke;

    [Obsolete("Use TerminateRoundFuncLinux or TerminateRoundFuncWindows instead")]
    public static MemoryFunctionVoid<IntPtr, RoundEndReason, float, IntPtr, byte> TerminateRoundFunc =
        new(CoreFunction.FromGamedata("CCSGameRules_TerminateRound"));

    [Obsolete("Use TerminateRoundLinux or TerminateRoundWindows instead")]
    public static Action<IntPtr, RoundEndReason, float, IntPtr, byte> TerminateRound = TerminateRoundFunc.Invoke;

    public static MemoryFunctionVoid<IntPtr, RoundEndReason, float, IntPtr, byte> TerminateRoundFuncLinux =
        new(CoreFunction.FromGamedata("CCSGameRules_TerminateRound"));

    public static Action<IntPtr, RoundEndReason, float, IntPtr, byte> TerminateRoundLinux = TerminateRoundFuncLinux.Invoke;

    public static MemoryFunctionVoid<IntPtr, float, RoundEndReason, IntPtr, byte> TerminateRoundFuncWindows =
        new(CoreFunction.FromGamedata("CCSGameRules_TerminateRound"));

    public static Action<IntPtr, float, RoundEndReason, IntPtr, byte> TerminateRoundWindows = TerminateRoundFuncWindows.Invoke;

    public static MemoryFunctionWithReturn<string, int, IntPtr> UTIL_CreateEntityByNameFunc =
        new(CoreFunction.FromGamedata("UTIL_CreateEntityByName"));

    public static Func<string, int, IntPtr> UTIL_CreateEntityByName = UTIL_CreateEntityByNameFunc.Invoke;

    public static MemoryFunctionVoid<IntPtr, IntPtr> CBaseEntity_DispatchSpawnFunc =
        new(CoreFunction.FromGamedata("CBaseEntity_DispatchSpawn"));

    public static Action<IntPtr, IntPtr> CBaseEntity_DispatchSpawn = CBaseEntity_DispatchSpawnFunc.Invoke;

    [Obsolete("Passes only two of SetPawn's four flags. Use CBasePlayerController_SetPawnFullFunc instead")]
    public static MemoryFunctionVoid<CBasePlayerController, CBasePlayerPawn, bool, bool> CBasePlayerController_SetPawnFunc =
        new(CoreFunction.FromGamedata("CBasePlayerController_SetPawn"));

    public static MemoryFunctionVoid<CBasePlayerController, CBasePlayerPawn, bool, bool, bool, bool> CBasePlayerController_SetPawnFullFunc =
        new(CoreFunction.FromGamedata("CBasePlayerController_SetPawn"));

    [Obsolete("Use Listeners.OnEntityTakeDamagePre instead")]
    public static MemoryFunctionVoid<CEntityInstance, CTakeDamageInfo, CTakeDamageResult> CBaseEntity_TakeDamageOldFunc =
        new(CoreFunction.FromGamedata("CBaseEntity_TakeDamageOld"));

    public static Action<CEntityInstance, CTakeDamageInfo, CTakeDamageResult> CBaseEntity_TakeDamageOld =
        CBaseEntity_TakeDamageOldFunc.Invoke;

    public static MemoryFunctionWithReturn<CCSPlayer_WeaponServices, CBasePlayerWeapon, bool> CCSPlayer_WeaponServices_CanUseFunc =
        new(CoreFunction.FromGamedata("CCSPlayer_WeaponServices_CanUse"));

    public static Func<CCSPlayer_WeaponServices, CBasePlayerWeapon, bool> CCSPlayer_WeaponServices_CanUse =
        CCSPlayer_WeaponServices_CanUseFunc.Invoke;

    public static MemoryFunctionWithReturn<int, string, CCSWeaponBaseVData> GetCSWeaponDataFromKeyFunc =
        new(CoreFunction.FromGamedata("GetCSWeaponDataFromKey"));

    public static Func<int, string, CCSWeaponBaseVData> GetCSWeaponDataFromKey = GetCSWeaponDataFromKeyFunc.Invoke;

    public static MemoryFunctionWithReturn<CCSPlayer_ItemServices, CEconItemView, AcquireMethod, IntPtr, AcquireResult>
        CCSPlayer_ItemServices_CanAcquireFunc = new(CoreFunction.FromGamedata("CCSPlayer_ItemServices_CanAcquire"));

    public static Func<CCSPlayer_ItemServices, CEconItemView, AcquireMethod, IntPtr, AcquireResult> CCSPlayer_ItemServices_CanAcquire =
        CCSPlayer_ItemServices_CanAcquireFunc.Invoke;

    public static MemoryFunctionVoid<CCSPlayerPawnBase> CCSPlayerPawnBase_PostThinkFunc =
        new(CoreFunction.FromGamedata("CCSPlayerPawnBase_PostThink"));

    public static Action<CCSPlayerPawnBase> CCSPlayerPawnBase_PostThink = CCSPlayerPawnBase_PostThinkFunc.Invoke;

    public static MemoryFunctionVoid<CBaseTrigger, CBaseEntity> CBaseTrigger_StartTouchFunc =
        new(CoreFunction.FromGamedata("CBaseTrigger_StartTouch"));

    public static Action<CBaseTrigger, CBaseEntity> CBaseTrigger_StartTouch = CBaseTrigger_StartTouchFunc.Invoke;

    public static MemoryFunctionVoid<CBaseTrigger, CBaseEntity> CBaseTrigger_EndTouchFunc =
        new(CoreFunction.FromGamedata("CBaseTrigger_EndTouch"));

    public static Action<CBaseTrigger, CBaseEntity> CBaseTrigger_EndTouch = CBaseTrigger_EndTouchFunc.Invoke;

    public static MemoryFunctionVoid<IntPtr, IntPtr> RemovePlayerItemFunc =
        new(CoreFunction.FromGamedata("CBasePlayerPawn_RemovePlayerItem"));

    public static Action<IntPtr, IntPtr> RemovePlayerItemVirtual = RemovePlayerItemFunc.Invoke;
}
