using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API;

public abstract class NativeEntity : NativeObject
{
    public new IntPtr Handle => EntitySystem.GetEntityByHandle(EntityHandle) ?? IntPtr.Zero;
    public CEntityHandle EntityHandle { get; }

    public NativeEntity(IntPtr pointer) : base(pointer)
    {
        EntityHandle = new(EntitySystem.GetRawHandleFromEntityPointer(pointer));
    }

    public NativeEntity(uint rawHandle) : base(EntitySystem.GetEntityByHandle(rawHandle) ?? IntPtr.Zero)
    {
        EntityHandle = new(rawHandle);
    }
}