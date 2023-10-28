using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CBaseEntity
{
    public Vector AbsVelocity => Schema.GetDeclaredClass<Vector>(this.Handle, "CBaseEntity", "m_vecAbsVelocity");
}