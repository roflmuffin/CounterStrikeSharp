using System;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

public enum DHookRegister {
    EAX_RAX = 0,
    EBX_RBX = 1,
    ECX_RCX = 2,
    EDX_RDX = 3,
    ESI_RSI = 4,
    EDI_RDI = 5,
    EBP_RBP = 6,
    ESP_RSP = 7,

    R8 = 8,
    R9 = 9,
    R10 = 10,
    R11 = 11,
    R12 = 12,
    R13 = 13,
    R14 = 14,
    R15 = 15,

    XMM0 = 16,
    XMM1 = 17,
    XMM2 = 18,
    XMM3 = 19,
    XMM4 = 20,
    XMM5 = 21,
    XMM6 = 22,
    XMM7 = 23,
    XMM8 = 24,
    XMM9 = 25,
    XMM10 = 26,
    XMM11 = 27,
    XMM12 = 28,
    XMM13 = 29,
    XMM14 = 30,
    XMM15 = 31
}

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

    public T GetRegister<T>(DHookRegister registerId) {
        return NativeAPI.DynamicHookGetRegister<T>(Handle, (int)registerId, (int)typeof(T).ToValidDataType());
    }
}
