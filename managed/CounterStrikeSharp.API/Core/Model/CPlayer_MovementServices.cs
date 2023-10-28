using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public class CInButtonState : NativeObject
{
    public CInButtonState(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe ref PlayerButtons Value =>
        ref Unsafe.AsRef<PlayerButtons>((void*)(Handle + Schema.GetSchemaOffset("CInButtonState", "m_pButtonStates")));
}

public partial class CPlayer_MovementServices
{
    public CInButtonState ButtonState =>
        Schema.GetDeclaredClass<CInButtonState>(this.Handle, "CPlayer_MovementServices", "m_nButtons");
}