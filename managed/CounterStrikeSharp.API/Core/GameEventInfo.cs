using System;
using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Core;

public class GameEventInfo : NativeObject
{
    public GameEventInfo(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe ref bool DontBroadcast => ref Unsafe.AsRef<bool>((void*)Handle);
}