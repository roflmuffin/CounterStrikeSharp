using System;
using CounterStrikeSharp.API.Core;
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
    public static Action<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr> GiveNamedItem =
        VirtualFunction.CreateVoid<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr>(
            GameData.GetSignature("GiveNamedItem"));

    public static Action<IntPtr, byte> SwitchTeam = VirtualFunction.CreateVoid<IntPtr, byte>(GameData.GetSignature("CCSPlayerController_SwitchTeam"));

    // void(*UTIL_Remove)(CEntityInstance*);
    public static Action<IntPtr> UTIL_Remove = VirtualFunction.CreateVoid<IntPtr>(GameData.GetSignature("UTIL_Remove"));
}