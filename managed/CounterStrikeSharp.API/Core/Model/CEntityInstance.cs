using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CEntityInstance
{
    public bool IsValid => Handle != IntPtr.Zero;

    public CEntityIndex? EntityIndex => IsValid ? m_pEntity.Value.EntityHandle.Index : null;
}

public partial class CEntityIdentity
{
    public unsafe CEntityInstance EntityInstance => new(Unsafe.Read<IntPtr>((void*)Handle));
    public unsafe CHandle<CEntityInstance> EntityHandle => new(Handle + 0x10);
}