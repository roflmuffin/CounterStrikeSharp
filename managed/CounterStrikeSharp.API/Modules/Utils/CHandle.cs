using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Utils;

public readonly record struct CEntityIndex(uint Value)
{
    public override string ToString() => $"Entity Index {Value}";
}

public class CHandle<T> : IEquatable<CHandle<T>> where T : NativeEntity
{
    private uint _raw;
    private IntPtr? _pointer;

    public uint Raw
    {
        get
        {
            if (_pointer != null)
            {
                return (uint)Marshal.ReadInt32(_pointer.Value);
            }

            return _raw;
        }
        set
        {
            if (_pointer != null)
            {
                Marshal.WriteInt32(_pointer.Value, (int)value);
                return;
            }

            _raw = value;
        }
    }

    public CHandle(uint raw)
    {
        Raw = raw;
    }

    public CHandle(IntPtr raw)
    {
        _pointer = raw;
    }

    public T? Value => (T)Activator.CreateInstance(typeof(T), EntitySystem.GetEntityByHandle(this));

    public override string ToString() => IsValid ? $"Index = {Index}, Serial = {SerialNum}" : "<invalid>";

    public bool IsValid => Index != (Utilities.MaxEdicts - 1);

    public uint Index => (uint)(Raw & (Utilities.MaxEdicts - 1));
    public uint SerialNum => Raw >> Utilities.MaxEdictBits;
    
    public static implicit operator uint(CHandle<T> handle) => handle.Raw;

    public bool Equals(CHandle<T>? other)
    {
        return other != null && Raw == other.Raw;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CHandle<T>);
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
    
    public CEntityHandle(IntPtr raw) : base (raw)
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