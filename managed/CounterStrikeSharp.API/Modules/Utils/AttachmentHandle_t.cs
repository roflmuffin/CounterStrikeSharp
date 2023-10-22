using System;
using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Modules.Utils;

public class AttachmentHandle_t : NativeObject
{
    public AttachmentHandle_t(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe ref sbyte Value => ref Unsafe.Add(ref *(sbyte*)Handle.ToPointer(), 0);
}