using System;
using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CBasePlayerPawn
{
    public void CommitSuicide(bool explode, bool force)
    {
        VirtualFunction.CreateVoid<IntPtr, bool, bool>(Handle, 354)(Handle, explode, force);
    }
}