using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Natives.Structs;

namespace CounterStrikeSharp.API.Modules.Entities;

public static class EntitySystem
{
    private static Lazy<IntPtr> ConcreteEntityListPointer = new(NativeAPI.GetConcreteEntityListPointer);

    private const int MaxEntities = 32768;
    private const int MaxEntitiesPerChunk = 512;
    private const int MaxChunks = MaxEntities / MaxEntitiesPerChunk;
    private const int SizeOfEntityIdentity = 0x70;
    private const int HandleOffset = 0x10;
    private const uint InvalidEHandleIndex = 0xFFFFFFFF;

    static unsafe Span<IntPtr> IdentityChunks => new((void*)ConcreteEntityListPointer.Value, MaxChunks);
    public static IntPtr FirstActiveEntity => Marshal.ReadIntPtr(ConcreteEntityListPointer.Value, MaxEntitiesPerChunk);

    public static IntPtr? GetEntityByHandle(uint raw)
    {
        return GetEntityByHandle(new CHandle<CEntityInstance>(raw));
    }

    public static IntPtr? GetEntityByHandle<T>(CHandle<T> handle) where T : NativeEntity
    {
        unsafe
        {
            if (!handle.IsValid)
                return null;

            IntPtr pChunkToUse = IdentityChunks[(int)(handle.EntityIndex / MaxEntitiesPerChunk)];
            if (pChunkToUse == IntPtr.Zero)
                return null;

            IntPtr pIdentityPtr = IntPtr.Add(pChunkToUse, SizeOfEntityIdentity * (int)(handle.EntityIndex % MaxEntitiesPerChunk));

            if (pIdentityPtr == IntPtr.Zero)
                return null;

            var foundHandle = Unsafe.Read<CHandle<CEntityInstance>>((void*)(pIdentityPtr + HandleOffset));

            if (foundHandle.Raw != handle.Raw)
                return null;

            return Marshal.ReadIntPtr(pIdentityPtr);
        }
    }

    public static IntPtr? GetEntityByIndex(uint index)
    {
        unsafe
        {
            if ((int)index <= -1 || index >= MaxEntities - 1) return null;

            IntPtr pChunkToUse = IdentityChunks[(int)(index / MaxEntitiesPerChunk)];
            if (pChunkToUse == IntPtr.Zero)
                return null;

            IntPtr pIdentityPtr = IntPtr.Add(pChunkToUse, SizeOfEntityIdentity * (int)(index % MaxEntitiesPerChunk));

            if (pIdentityPtr == IntPtr.Zero)
                return null;

            var foundHandle = Unsafe.Read<CHandle<CEntityInstance>>((void*)(pIdentityPtr + HandleOffset));

            if (foundHandle.EntityIndex != index)
                return null;

            return Marshal.ReadIntPtr(pIdentityPtr);
        }
    }

    public static uint GetRawHandleFromEntityPointer(IntPtr pointer)
    {
        if (pointer == IntPtr.Zero)
            return InvalidEHandleIndex;

        return Schema.GetPointer<CEntityIdentity?>(pointer, "CEntityInstance", "m_pEntity")?.EntityHandle.Raw ??
               InvalidEHandleIndex;
    }

}
