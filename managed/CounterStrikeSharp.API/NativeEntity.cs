using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API;

public abstract class NativeEntity : NativeObject
{
    public new IntPtr Handle => EntitySystem.GetEntityByHandle(EntityHandle) ?? 0;
    public CEntityHandle EntityHandle { get; }

    public NativeEntity(IntPtr pointer) : base(pointer)
    {
        EntityHandle = new(NativeAPI.GetRefFromEntityPointer(pointer));
    }

    public NativeEntity(uint rawHandle) : base(EntitySystem.GetEntityByHandle(rawHandle) ?? 0)
    {
        EntityHandle = new(rawHandle);
    }
    
    public new T As<T>() where T : NativeEntity
    {
        return (T)Activator.CreateInstance(typeof(T), this.EntityHandle);
    }
}