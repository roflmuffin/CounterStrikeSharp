using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils;

public class CHandle<T> : NativeObject
{
    public CHandle(IntPtr pointer) : base(pointer)
    {
    }

    public T Value => (T)Activator.CreateInstance(typeof(T), NativeAPI.GetEntityPointerFromHandle(Handle));
}

public class PointerTo<T> : NativeObject where T : NativeObject
{
    public PointerTo(IntPtr pointer) : base(pointer)
    {
    }
    
    public T Value
    {
        get
        {
            unsafe
            {
                
                return (T)Activator.CreateInstance(typeof(T), Unsafe.Read<IntPtr>((void*)Handle));
            }
        }
    }
}