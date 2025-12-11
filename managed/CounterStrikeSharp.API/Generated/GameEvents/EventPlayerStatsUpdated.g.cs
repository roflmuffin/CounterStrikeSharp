#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_stats_updated")]
public class EventPlayerStatsUpdated : GameEvent
{
    public EventPlayerStatsUpdated(IntPtr pointer) : base(pointer){}
    public EventPlayerStatsUpdated(bool force) : base("player_stats_updated", force){}
    
    public bool Forceupload
    {
        get => Get<bool>("forceupload");
        set => Set<bool>("forceupload", value);
    }
}
#nullable restore
