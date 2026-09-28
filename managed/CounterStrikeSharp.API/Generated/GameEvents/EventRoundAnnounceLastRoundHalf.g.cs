#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_announce_last_round_half")]
public class EventRoundAnnounceLastRoundHalf : GameEvent
{
    public EventRoundAnnounceLastRoundHalf(IntPtr pointer) : base(pointer){}
    public EventRoundAnnounceLastRoundHalf(bool force) : base("round_announce_last_round_half", force){}
    
}
#nullable restore
