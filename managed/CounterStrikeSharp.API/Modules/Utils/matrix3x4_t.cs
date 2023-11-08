using System;
using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Modules.Utils;

public partial class matrix3x4_t : NativeObject
{
    public matrix3x4_t(IntPtr pointer) : base(pointer)
    {
    }
    
    public unsafe ref float this[int row, int column] => ref Unsafe.Add(ref *(float*)Handle, row * 4 + column);
}