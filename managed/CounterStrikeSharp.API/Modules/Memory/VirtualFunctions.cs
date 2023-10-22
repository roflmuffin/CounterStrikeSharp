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
}