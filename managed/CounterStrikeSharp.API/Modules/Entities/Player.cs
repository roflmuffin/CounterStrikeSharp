using System;

namespace CounterStrikeSharp.API.Modules.Entities;

public class Player : BaseEntity
{
    public Player(int index, IntPtr handle, IntPtr pawnHandle) : base(index, handle)
    {
        PawnHandle = pawnHandle;
    }

    public IntPtr PawnHandle { get; init; }

    public override string ToString()
    {
        return $"[Player index={Index}, handle={Handle:x8}, pawnHandle={PawnHandle:x8}]";
    }
}