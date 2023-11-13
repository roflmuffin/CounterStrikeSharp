using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class DynamicHook : NativeObject
{
    public DynamicHook(IntPtr pointer) : base(pointer)
    {
    }

    public T GetParam<T>(int index)
    {
        return NativeAPI.DynamicHookGetParam<T>(Handle, (int)typeof(T).ToValidDataType(), index);
    }

    public T GetReturn<T>(int index)
    {
        return NativeAPI.DynamicHookGetReturn<T>(Handle, (int)typeof(T).ToValidDataType());
    }

    public void SetParam<T>(int index, T value)
    {
        NativeAPI.DynamicHookSetParam(Handle, (int)typeof(T).ToValidDataType(), index, value);
    }

    public void SetReturn<T>(T value)
    {
        NativeAPI.DynamicHookSetReturn(Handle, (int)typeof(T).ToValidDataType(), value);
    }
}

public class DynamicDetour
{
    private readonly DataType _returnType;
    private readonly DataType[] _parameters;
    private static Dictionary<string, IntPtr> _createdFunctions = new();
    private readonly IntPtr _address;

    public static IntPtr CreateValveFunctionBySignature(string signature, DataType returnType,
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

    public DynamicDetour(IntPtr address, DataType returnType, DataType[] parameters)
    {
        _address = address;
        _returnType = returnType;
        _parameters = parameters;
    }

    public void Hook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        NativeAPI.HookFunction(_address, handler, mode == HookMode.Post);
    }

    public static DynamicDetour Test
    {
        get
        {
            var valveFunction = CreateValveFunctionBySignature(GameData.GetSignature("UTIL_Remove"),
                DataType.DATA_TYPE_VOID, new[] { DataType.DATA_TYPE_POINTER }
            );

            return new DynamicDetour(valveFunction, DataType.DATA_TYPE_VOID, new[] { DataType.DATA_TYPE_POINTER });
        }
    }

    public static DynamicDetour TakeDamage
    {
        get
        {
            var valveFunction = CreateValveFunctionBySignature(GameData.GetSignature("CBaseEntity_TakeDamageOld"),
                DataType.DATA_TYPE_VOID, new[] { DataType.DATA_TYPE_POINTER, DataType.DATA_TYPE_POINTER }
            );

            return new DynamicDetour(valveFunction, DataType.DATA_TYPE_VOID,
                new[] { DataType.DATA_TYPE_POINTER, DataType.DATA_TYPE_POINTER });
        }
    }
}