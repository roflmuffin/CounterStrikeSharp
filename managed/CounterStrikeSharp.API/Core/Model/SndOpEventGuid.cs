using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Core;

public class SndOpEventGuid : NativeObject
{
    public uint Guid { get; set; }
    public ulong StackHash { get; set; }
    
    public SndOpEventGuid(IntPtr pointer) : base(pointer)
    {
        Guid = (uint)Marshal.ReadInt32(Handle);
        StackHash = (ulong)Marshal.ReadInt64(Handle + 4);
    }
}

