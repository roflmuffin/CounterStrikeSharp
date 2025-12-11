#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("break_prop")]
public class EventBreakProp : GameEvent
{
    public EventBreakProp(IntPtr pointer) : base(pointer){}
    public EventBreakProp(bool force) : base("break_prop", force){}
    
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }

    public bool PlayerDropped
    {
        get => Get<bool>("player_dropped");
        set => Set<bool>("player_dropped", value);
    }

    public bool PlayerHeld
    {
        get => Get<bool>("player_held");
        set => Set<bool>("player_held", value);
    }

    public bool PlayerThrown
    {
        get => Get<bool>("player_thrown");
        set => Set<bool>("player_thrown", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
