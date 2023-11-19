using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Memory;

class MemoryFunction : NativeObject
{
    private static Dictionary<string, IntPtr> _createdFunctions = new();

    private static IntPtr CreateValveFunctionBySignature(string signature, DataType returnType,
        DataType[] argumentTypes)
    {
        if (!_createdFunctions.TryGetValue(signature, out var function))
        {
            function = NativeAPI.CreateVirtualFunctionBySignature(IntPtr.Zero, Addresses.ServerPath, signature,
                argumentTypes.Length, (int)returnType, argumentTypes.Cast<object>().ToArray());
            _createdFunctions[signature] = function;
        }

        return function;
    }

    public MemoryFunction(string signature, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionBySignature(signature, returnType, parameters))
    {
    }

    public void Hook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        NativeAPI.HookFunction(Handle, handler, mode == HookMode.Post);
    }

    public void Unhook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        NativeAPI.UnhookFunction(Handle, handler, mode == HookMode.Post);
    }
}

public static class VirtualFunctions
{
    public static Action<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr> ClientPrint =
        VirtualFunction.CreateVoid<IntPtr, HudDestination, string, IntPtr, IntPtr, IntPtr, IntPtr>(
            GameData.GetSignature("ClientPrint"));

    // public static MemoryFunction ClientPrintMemory =
    //     new MemoryFunction(GameData.GetSignature("ClientPrint"), DataType.DATA_TYPE_VOID, new[]
    //     {
    //         DataType.DATA_TYPE_POINTER, DataType.DATA_TYPE_INT, DataType.Int32, DataType.Int32, DataType.Int32, DataType.Int32
    //     });

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
    public static Action<IntPtr, string> SetModel =
        VirtualFunction.CreateVoid<IntPtr, string>(GameData.GetSignature("CBaseModelEntity_SetModel"));
}