using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CPlayer_MovementServices
{
    public CInButtonState ButtonState =>
        Schema.GetDeclaredClass<CInButtonState>(this.Handle, "CPlayer_MovementServices", "m_nButtons");
}