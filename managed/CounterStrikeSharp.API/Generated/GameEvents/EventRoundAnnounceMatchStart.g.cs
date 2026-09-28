#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_announce_match_start")]
public class EventRoundAnnounceMatchStart : GameEvent
{
    public EventRoundAnnounceMatchStart(IntPtr pointer) : base(pointer){}
    public EventRoundAnnounceMatchStart(bool force) : base("round_announce_match_start", force){}
    
}
#nullable restore
