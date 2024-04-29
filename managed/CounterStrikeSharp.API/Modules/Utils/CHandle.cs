using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace CounterStrikeSharp.API.Modules.Utils;

public readonly record struct CEntityIndex(uint Value)
{
    public override string ToString() => $"Entity Index {Value}";
}

/// <summary>
/// CHandle is a class that represents a 32-bit ID (entindex + serial number) unique to every past and present entity in a game.
/// It is used to refer to entities where pointers and entity indexes are unsafe; mainly across the client/server divide.
/// <a href="https://developer.valvesoftware.com/wiki/CHandle">More info</a>
/// </summary>
/// <typeparam name="T">Type of entity this handle refers to</typeparam>
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


    /// <inheritdoc cref="Get"/>
    public T? Value => Get();

    /// <summary>
    /// Retrieves the instance of the entity this handle refers to.
    /// </summary>
    public T? Get()
    {
        if (!IsValid)
            return null;

        var entity = EntitySystem.GetEntityByHandle(this);
        if (entity == null)
            return null;

        return (T)Activator.CreateInstance(typeof(T), entity)!;
    }

    public override string ToString() => IsValid ? $"Index = {Index}, Serial = {SerialNum}" : "<invalid>";

    /// <summary>
    /// Checks that the handle is valid and points to an entity.
    /// </summary>
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

    public CEntityHandle(IntPtr raw) : base(raw)
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
