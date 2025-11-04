using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Natives.Structs;

/// <summary>
/// CHandle is a class that represents a 32-bit ID (entindex + serial number) unique to every past and present entity in a game.
/// It is used to refer to entities where pointers and entity indexes are unsafe; mainly across the client/server divide.
/// <a href="https://developer.valvesoftware.com/wiki/CHandle">More info</a>
/// </summary>
/// <typeparam name="T">Type of entity this handle refers to</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 4)]
public record struct CHandle<T> where T : NativeEntity
{
    public uint Raw { get; set; }

    public CHandle(uint raw)
    {
        Raw = raw;
    }

    /// <inheritdoc cref="Get"/>
    public T? Value
    {
        get => Get();
        set => Raw = value == null ? uint.MaxValue : EntitySystem.GetRawHandleFromEntityPointer(value.Handle);
    }

    /// <summary>
    /// Retrieves/sets the instance of the entity this handle refers to.
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


    /// <summary>
    /// Checks that the handle is valid and points to an entity.
    /// </summary>
    public bool IsValid => EntityIndex != Utilities.MaxEdicts - 1;

    public uint EntityIndex => (uint)(Raw & (Utilities.MaxEdicts - 1));
    public uint SerialNumber => Raw >> Utilities.MaxEdictBits;

    public static implicit operator uint(CHandle<T> handle) => handle.Raw;
    public static implicit operator T(CHandle<T> handle) => handle.Value;
    public override string ToString() => IsValid ? $"Index = {EntityIndex}, Serial = {SerialNumber}" : "<invalid>";
}
