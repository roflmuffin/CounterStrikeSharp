using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CEntityIOOutput 
{ 
    public EntityIOConnection_t? Connections => Utilities.GetPointer<EntityIOConnection_t>(Handle + 8); 
 
    public EntityIOOutputDesc_t Description => new(Marshal.ReadIntPtr(Handle + 16)); 
} 
 
public class EntityIOOutputDesc_t : NativeObject 
{ 
    public EntityIOOutputDesc_t(IntPtr pointer) : base(pointer) 
    { 
    } 
 
    public string Name => Utilities.ReadStringUtf8(Handle + 0); 
    public unsafe ref uint Flags => ref Unsafe.AsRef<uint>((void*)(Handle + 8)); 
    public unsafe ref uint OutputOffset => ref Unsafe.AsRef<uint>((void*)(Handle + 16)); 
} 
 
public class EntityIOConnection_t : EntityIOConnectionDesc_t 
{ 
    public EntityIOConnection_t(IntPtr pointer) : base(pointer) 
    { 
    } 
 
    public unsafe ref bool MarkedForRemoval => ref Unsafe.AsRef<bool>((void*)(Handle + 40)); 
 
    public EntityIOConnection_t? Next => Utilities.GetPointer<EntityIOConnection_t>(Handle + 48); 
} 
 
public class EntityIOConnectionDesc_t : NativeObject 
{ 
    public EntityIOConnectionDesc_t(IntPtr pointer) : base(pointer) 
    { 
    } 
 
    public string TargetDesc => Utilities.ReadStringUtf8(Handle + 0); 
    public string TargetInput => Utilities.ReadStringUtf8(Handle + 8); 
    public string ValueOverride => Utilities.ReadStringUtf8(Handle + 16); 
    public CEntityHandle Target => new(Handle + 24); 
    public unsafe ref EntityIOTargetType_t TargetType => ref Unsafe.AsRef<EntityIOTargetType_t>((void*)(Handle + 28)); 
    public unsafe ref int TimesToFire => ref Unsafe.AsRef<int>((void*)(Handle + 32)); 
    public unsafe ref float Delay => ref Unsafe.AsRef<float>((void*)(Handle + 36)); 
}