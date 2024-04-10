using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Modules.Utils;

public class CStrongHandle<T> : NativeObject
{
    public CStrongHandle(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe ulong Value => Unsafe.Read<ulong>((void*)Handle);
}

public class CWeakHandle<T> : NativeObject
{
    public CWeakHandle(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe ulong Value => Unsafe.Read<ulong>((void*)Handle);
}
