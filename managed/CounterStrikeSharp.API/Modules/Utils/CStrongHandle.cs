using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils;

public class CStrongHandle<T> : NativeObject
{
    public CStrongHandle(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe ulong Value => Unsafe.Read<ulong>((void*)Handle);
}