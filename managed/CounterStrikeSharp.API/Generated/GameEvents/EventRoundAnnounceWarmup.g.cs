#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_announce_warmup")]
public class EventRoundAnnounceWarmup : GameEvent
{
    public EventRoundAnnounceWarmup(IntPtr pointer) : base(pointer){}
    public EventRoundAnnounceWarmup(bool force) : base("round_announce_warmup", force){}
    
}
#nullable restore
