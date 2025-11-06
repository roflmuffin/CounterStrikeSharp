#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_ping_stop")]
public class EventPlayerPingStop : GameEvent
{
    public EventPlayerPingStop(IntPtr pointer) : base(pointer){}
    public EventPlayerPingStop(bool force) : base("player_ping_stop", force){}
    
    public int Entityid
    {
        get => Get<int>("entityid");
        set => Set<int>("entityid", value);
    }
}
#nullable restore
