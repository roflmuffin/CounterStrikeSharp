using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Natives;

namespace CounterStrikeSharp.API.Modules.Utils;

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
