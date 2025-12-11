#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_start")]
public class EventRoundStart : GameEvent
{
    public EventRoundStart(IntPtr pointer) : base(pointer){}
    public EventRoundStart(bool force) : base("round_start", force){}
    // frag limit in seconds
    public long Fraglimit
    {
        get => Get<long>("fraglimit");
        set => Set<long>("fraglimit", value);
    }
// round objective
    public string Objective
    {
        get => Get<string>("objective");
        set => Set<string>("objective", value);
    }
// round time limit in seconds
    public long Timelimit
    {
        get => Get<long>("timelimit");
        set => Set<long>("timelimit", value);
    }
}
#nullable restore
