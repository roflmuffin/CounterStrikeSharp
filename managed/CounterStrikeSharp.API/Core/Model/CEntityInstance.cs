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

    public CEntityIndex? EntityIndex => IsValid ? Entity?.EntityHandle.Index : null;
    
    public string DesignerName => IsValid ? Entity?.DesignerName : null;
}

public partial class CEntityIdentity
{
    public unsafe CEntityInstance EntityInstance => new(Unsafe.Read<IntPtr>((void*)Handle));
    public unsafe CHandle<CEntityInstance> EntityHandle => new(Handle + 0x10);
    public unsafe string DesignerName => Utilities.ReadStringUtf8(Handle + 0x20);
    public unsafe PointerTo<CEntityIdentity> Prev => new(Handle + 0x58);
    public unsafe PointerTo<CEntityIdentity> Next => new(Handle + 0x60);
    public unsafe PointerTo<CEntityIdentity> PrevByClass => new(Handle + 0x68);
    public unsafe PointerTo<CEntityIdentity> NextByClass => new(Handle + 0x70);
}