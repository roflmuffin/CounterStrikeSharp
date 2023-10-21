using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils;

public readonly record struct CEntityIndex(uint Value)
{
    public override string ToString() => $"Entity Index {Value}";
}

public class CHandle<T> : NativeObject
{
    public CHandle(IntPtr pointer) : base(pointer)
    {
    }

    public T Value => (T)Activator.CreateInstance(typeof(T), NativeAPI.GetEntityPointerFromHandle(Handle));

    public unsafe ulong Raw => Unsafe.Read<ulong>((void*)Handle);

    public override string ToString() => IsValid ? $"Index = {Index.Value}, Serial = {SerialNum}" : "<invalid>";

    public bool IsValid => Index.Value != (Utilities.MaxEdicts - 1);

    public CEntityIndex Index => new((uint)(Raw & (Utilities.MaxEdicts - 1)));
    public uint SerialNum => (uint)(Raw >> Utilities.MaxEdictBits);
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