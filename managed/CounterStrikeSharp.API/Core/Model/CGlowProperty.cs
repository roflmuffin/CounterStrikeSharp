using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CGlowProperty
{
    // m_bGlowing
    public ref bool IsGlowing => ref Schema.GetRef<bool>(this.Handle, "CGlowProperty", "m_bGlowing");

}
