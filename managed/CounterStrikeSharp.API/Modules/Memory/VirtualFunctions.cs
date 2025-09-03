using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Memory;

public static class VirtualFunctions
{
    public static MemoryFunctionVoid<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintFunc =
        new(GameData.GetSignature("ClientPrint"));

    public static Action<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrint =
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => ClientPrintFunc.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

    public static MemoryFunctionVoid<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintAllFunc =
        new(GameData.GetSignature("UTIL_ClientPrintAll"));

    public static Action<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintAll =
        (arg1, arg2, arg3, arg4, arg5, arg6) => ClientPrintAllFunc.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);

    // void (*FnGiveNamedItem)(void* itemService,const char* pchName, void* iSubType,void* pScriptItem, void* a5,void* a6) = nullptr;
    public static MemoryFunctionWithReturn<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> GiveNamedItemFunc =
        new(GameData.GetSignature("GiveNamedItem"));

    public static Func<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> GiveNamedItem =
        (arg1, arg2, arg3, arg4, arg5, arg6) => GiveNamedItemFunc.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);

    public static MemoryFunctionVoid<IntPtr, byte> SwitchTeamFunc =
        new(GameData.GetSignature("CCSPlayerController_SwitchTeam"));

    public static Action<IntPtr, byte> SwitchTeam =
        (arg1, arg2) => SwitchTeam.Invoke(arg1, arg2);

    // void(*UTIL_Remove)(CEntityInstance*);
    public static MemoryFunctionVoid<IntPtr> UTIL_RemoveFunc =
        new(GameData.GetSignature("UTIL_Remove"));

    public static Action<IntPtr> UTIL_Remove =
        (arg1) => UTIL_RemoveFunc.Invoke(arg1);

    // void(*CBaseModelEntity_SetModel)(CBaseModelEntity*, const char*);
    public static MemoryFunctionVoid<IntPtr, string> SetModelFunc =
        new(GameData.GetSignature("CBaseModelEntity_SetModel"));

    public static Action<IntPtr, string> SetModel =
        (arg1, arg2) => SetModelFunc.Invoke(arg1, arg2);

    public static MemoryFunctionVoid<nint, RoundEndReason, float, nint, byte> TerminateRoundFunc =
        new(GameData.GetSignature("CCSGameRules_TerminateRound"));

    public static Action<IntPtr, RoundEndReason, float, nint, byte> TerminateRound =
        (arg1, arg2, arg3, arg4, arg5) => TerminateRoundFunc.Invoke(arg1, arg2, arg3, arg4, arg5);

    public static MemoryFunctionWithReturn<string, int, IntPtr> UTIL_CreateEntityByNameFunc =
        new(GameData.GetSignature("UTIL_CreateEntityByName"));

    public static Func<string, int, IntPtr> UTIL_CreateEntityByName =
        (arg1, arg2) => UTIL_CreateEntityByNameFunc.Invoke(arg1, arg2);

    public static MemoryFunctionVoid<IntPtr, IntPtr> CBaseEntity_DispatchSpawnFunc =
        new(GameData.GetSignature("CBaseEntity_DispatchSpawn"));

    public static Action<IntPtr, IntPtr> CBaseEntity_DispatchSpawn =
        (arg1, arg2) => CBaseEntity_DispatchSpawnFunc.Invoke(arg1, arg2);

    public static MemoryFunctionVoid<CBasePlayerController, CBasePlayerPawn, bool, bool> CBasePlayerController_SetPawnFunc =
        new(GameData.GetSignature("CBasePlayerController_SetPawn"));

    public static MemoryFunctionVoid<CEntityInstance, CTakeDamageInfo> CBaseEntity_TakeDamageOldFunc =
        new(GameData.GetSignature("CBaseEntity_TakeDamageOld"));

    public static Action<CEntityInstance, CTakeDamageInfo> CBaseEntity_TakeDamageOld =
        (arg1, arg2) => CBaseEntity_TakeDamageOldFunc.Invoke(arg1, arg2);

    public static MemoryFunctionWithReturn<CCSPlayer_WeaponServices, CBasePlayerWeapon, bool> CCSPlayer_WeaponServices_CanUseFunc =
        new(GameData.GetSignature("CCSPlayer_WeaponServices_CanUse"));

    public static Func<CCSPlayer_WeaponServices, CBasePlayerWeapon, bool> CCSPlayer_WeaponServices_CanUse =
        (arg1, arg2) => CCSPlayer_WeaponServices_CanUseFunc.Invoke(arg1, arg2);

    public static MemoryFunctionWithReturn<int, string, CCSWeaponBaseVData> GetCSWeaponDataFromKeyFunc =
        new(GameData.GetSignature("GetCSWeaponDataFromKey"));

    public static Func<int, string, CCSWeaponBaseVData> GetCSWeaponDataFromKey =
        (arg1, arg2) => GetCSWeaponDataFromKeyFunc.Invoke(arg1, arg2);

    public static MemoryFunctionWithReturn<CCSPlayer_ItemServices, CEconItemView, AcquireMethod, IntPtr, AcquireResult>
        CCSPlayer_ItemServices_CanAcquireFunc = new(GameData.GetSignature("CCSPlayer_ItemServices_CanAcquire"));

    public static Func<CCSPlayer_ItemServices, CEconItemView, AcquireMethod, IntPtr, AcquireResult> CCSPlayer_ItemServices_CanAcquire =
        (arg1, arg2, arg3, arg4) => CCSPlayer_ItemServices_CanAcquireFunc.Invoke(arg1, arg2, arg3, arg4);

    public static MemoryFunctionVoid<CCSPlayerPawnBase> CCSPlayerPawnBase_PostThinkFunc =
        new(GameData.GetSignature("CCSPlayerPawnBase_PostThink"));

    public static Action<CCSPlayerPawnBase> CCSPlayerPawnBase_PostThink =
        (arg1) => CCSPlayerPawnBase_PostThinkFunc.Invoke(arg1);

    public static MemoryFunctionVoid<CBaseTrigger, CBaseEntity> CBaseTrigger_StartTouchFunc =
        new(GameData.GetSignature("CBaseTrigger_StartTouch"));

    public static Action<CBaseTrigger, CBaseEntity> CBaseTrigger_StartTouch =
        (arg1, arg2) => CBaseTrigger_StartTouchFunc.Invoke(arg1, arg2);

    public static MemoryFunctionVoid<CBaseTrigger, CBaseEntity> CBaseTrigger_EndTouchFunc =
        new(GameData.GetSignature("CBaseTrigger_EndTouch"));

    public static Action<CBaseTrigger, CBaseEntity> CBaseTrigger_EndTouch =
        (arg1, arg2) => CBaseTrigger_EndTouchFunc.Invoke(arg1, arg2);

    public static MemoryFunctionVoid<IntPtr, IntPtr> RemovePlayerItemFunc =
        new(GameData.GetSignature("CBasePlayerPawn_RemovePlayerItem"));

    public static Action<IntPtr, IntPtr> RemovePlayerItemVirtual =
        (arg1, arg2) => RemovePlayerItemFunc.Invoke(arg1, arg2);
}
