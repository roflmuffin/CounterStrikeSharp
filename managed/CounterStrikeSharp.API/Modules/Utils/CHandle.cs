using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils;

public readonly record struct CEntityIndex(uint Value)
{
    public override string ToString() => $"Entity Index {Value}";
}

public class CHandle<T> : IEquatable<CHandle<T>> where T : NativeEntity
{
    public uint Raw;
    
    public CHandle(uint raw)
    {
        Raw = raw;
    }
    
    public CHandle(IntPtr raw)
    {
        Raw = (uint)Marshal.ReadInt32(raw);
    }
    
    public T? Value => (T)Activator.CreateInstance(typeof(T), NativeAPI.GetEntityPointerFromRef(Raw));

    public override string ToString() => IsValid ? $"Index = {Index}, Serial = {SerialNum}" : "<invalid>";

    public bool IsValid => Index != (Utilities.MaxEdicts - 1) && NativeAPI.IsRefValidEntity(Raw);

    public uint Index => Raw & (Utilities.MaxEdicts - 1);
    public uint SerialNum => Raw >> Utilities.MaxEdictBits;

    public bool Equals(CHandle<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Raw == other.Raw;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CHandle<T>)obj);
    }

    public override int GetHashCode()
    {
        return (int)Raw;
    }
}

public class CEntityHandle : CHandle<CEntityInstance>
{
    public CEntityHandle(uint raw) : base(raw)
    {
    }
}

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