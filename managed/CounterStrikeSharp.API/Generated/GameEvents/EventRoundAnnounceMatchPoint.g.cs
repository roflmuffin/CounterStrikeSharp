#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_announce_match_point")]
public class EventRoundAnnounceMatchPoint : GameEvent
{
    public EventRoundAnnounceMatchPoint(IntPtr pointer) : base(pointer){}
    public EventRoundAnnounceMatchPoint(bool force) : base("round_announce_match_point", force){}
    
}
#nullable restore
