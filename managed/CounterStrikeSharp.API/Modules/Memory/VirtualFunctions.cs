using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Memory;

public static class VirtualFunctions
{
    public static Action<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrint =
        VirtualFunction.CreateVoid<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr>(
            GameData.GetSignature("ClientPrint"));

    public static Action<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrintAll =
        VirtualFunction.CreateVoid<HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr>(
            GameData.GetSignature("UTIL_ClientPrintAll"));

    // void (*FnGiveNamedItem)(void* itemService,const char* pchName, void* iSubType,void* pScriptItem, void* a5,void* a6) = nullptr;
    public static Func<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> GiveNamedItem =
        VirtualFunction.Create<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>(
            GameData.GetSignature("GiveNamedItem"));

    public static Action<IntPtr, byte> SwitchTeam =
        VirtualFunction.CreateVoid<IntPtr, byte>(GameData.GetSignature("CCSPlayerController_SwitchTeam"));

    // void(*UTIL_Remove)(CEntityInstance*);
    public static Action<IntPtr> UTIL_Remove = VirtualFunction.CreateVoid<IntPtr>(GameData.GetSignature("UTIL_Remove"));

    // void(*CBaseModelEntity_SetModel)(CBaseModelEntity*, const char*);
    public static Action<IntPtr, string> SetModel = VirtualFunction.CreateVoid<IntPtr, string>(GameData.GetSignature("CBaseModelEntity_SetModel"));

    public static Action<IntPtr, RoundEndReason, float> TerminateRound = VirtualFunction.CreateVoid<nint, RoundEndReason, float>(GameData.GetSignature("CCSGameRules_TerminateRound"));

    public static Func<string, int, IntPtr> UTIL_CreateEntityByName = VirtualFunction.Create<string, int, IntPtr>(GameData.GetSignature("UTIL_CreateEntityByName"));

    public static Action<IntPtr, IntPtr> CBaseEntity_DispatchSpawn = VirtualFunction.CreateVoid<IntPtr, IntPtr>(GameData.GetSignature("CBaseEntity_DispatchSpawn"));
}