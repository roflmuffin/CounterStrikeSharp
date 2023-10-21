using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CEntityInstance
{
    public bool IsValid => Handle != IntPtr.Zero;

    public CEntityIndex? EntityIndex => IsValid ? m_pEntity.Value.EntityHandle.Index : null;
    
    public string DesignerName => IsValid ? m_pEntity.Value.DesignerName : null;
}

public partial class CEntityIdentity
{
    public unsafe CEntityInstance EntityInstance => new(Unsafe.Read<IntPtr>((void*)Handle));
    public unsafe CHandle<CEntityInstance> EntityHandle => new(Handle + 0x10);
    public unsafe string DesignerName => ReadStringUtf8(Handle + 0x20);
    public unsafe PointerTo<CEntityIdentity> Prev => new(Handle + 0x58);
    public unsafe PointerTo<CEntityIdentity> Next => new(Handle + 0x60);
    public unsafe PointerTo<CEntityIdentity> PrevByClass => new(Handle + 0x68);
    public unsafe PointerTo<CEntityIdentity> NextByClass => new(Handle + 0x70);

    // TODO: Move this method to a shared marshalling lib
    public unsafe string ReadStringUtf8(IntPtr ptr)
    {
        unsafe
        {
            var nativeUtf8 = Unsafe.Read<IntPtr>((void*)ptr);

            if (nativeUtf8 == IntPtr.Zero)
            {
                return null;
            }

            var len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0)
            {
                ++len;
            }

            var buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}