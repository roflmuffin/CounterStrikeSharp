using System;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

public class DynamicHook : NativeObject
{
    public DynamicHook(IntPtr pointer) : base(pointer)
    {
    }

    public T GetParam<T>(int index)
    {
        return NativeAPI.DynamicHookGetParam<T>(Handle, (int)typeof(T).ToValidDataType(), index);
    }

    [Obsolete("Use GetReturn<T>() instead")]
    public T GetReturn<T>(int index)
    {
        return GetReturn<T>();
    }
    
    public T GetReturn<T>()
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
