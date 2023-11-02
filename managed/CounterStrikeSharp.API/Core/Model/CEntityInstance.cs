using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CEntityInstance : IEquatable<CEntityInstance>
{
    public bool IsValid => Handle != IntPtr.Zero;

    public CEntityIndex? EntityIndex => IsValid ? Entity?.EntityHandle.Index : null;
    
    public string DesignerName => IsValid ? Entity?.DesignerName : null;

    public void Remove() => VirtualFunctions.UTIL_Remove(this.Handle);


    public bool Equals(CEntityInstance? other)
    {
        return this.Handle == other?.Handle;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is CEntityInstance other && Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Handle.GetHashCode();
    }

    public static bool operator ==(CEntityInstance? left, CEntityInstance? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CEntityInstance? left, CEntityInstance? right)
    {
        return !Equals(left, right);
    }
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