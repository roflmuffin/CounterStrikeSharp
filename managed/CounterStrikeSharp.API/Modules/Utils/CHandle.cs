using System;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils;

public class CHandle<T> : NativeObject
{
    public CHandle(IntPtr pointer) : base(pointer)
    {
    }

    public T Value => (T)Activator.CreateInstance(typeof(T), NativeAPI.GetEntityPointerFromHandle(Handle));
}