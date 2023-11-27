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
    public CEntityInstance(IntPtr pointer) : base(pointer)
    {
    }

    public CEntityInstance(uint rawHandle) : base(rawHandle)
    {
    }
    
    public bool IsValid => EntityHandle.IsValid && Handle != IntPtr.Zero;

    public uint EntityIndex => EntityHandle.Index;
    
    public string DesignerName => IsValid ? Entity?.DesignerName : null;

    public void Remove() => VirtualFunctions.UTIL_Remove(this.Handle);
    
    public bool Equals(CEntityInstance? other)
    {
        return this.EntityHandle == other?.EntityHandle;
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
    public unsafe CHandle<CEntityInstance> EntityHandle => new(Unsafe.Read<uint>((void*)this.Handle) + 0x10);
}