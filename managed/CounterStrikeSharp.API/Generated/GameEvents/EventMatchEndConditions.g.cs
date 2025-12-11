#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("match_end_conditions")]
public class EventMatchEndConditions : GameEvent
{
    public EventMatchEndConditions(IntPtr pointer) : base(pointer){}
    public EventMatchEndConditions(bool force) : base("match_end_conditions", force){}
    
    public long Frags
    {
        get => Get<long>("frags");
        set => Set<long>("frags", value);
    }

    public long MaxRounds
    {
        get => Get<long>("max_rounds");
        set => Set<long>("max_rounds", value);
    }

    public long Time
    {
        get => Get<long>("time");
        set => Set<long>("time", value);
    }

    public long WinRounds
    {
        get => Get<long>("win_rounds");
        set => Set<long>("win_rounds", value);
    }
}
#nullable restore
