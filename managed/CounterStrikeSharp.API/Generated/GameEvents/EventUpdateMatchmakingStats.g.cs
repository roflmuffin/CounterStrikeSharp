#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("update_matchmaking_stats")]
public class EventUpdateMatchmakingStats : GameEvent
{
    public EventUpdateMatchmakingStats(IntPtr pointer) : base(pointer){}
    public EventUpdateMatchmakingStats(bool force) : base("update_matchmaking_stats", force){}
    
}
#nullable restore
