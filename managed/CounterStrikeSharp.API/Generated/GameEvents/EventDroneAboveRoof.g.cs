#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("drone_above_roof")]
public class EventDroneAboveRoof : GameEvent
{
    public EventDroneAboveRoof(IntPtr pointer) : base(pointer){}
    public EventDroneAboveRoof(bool force) : base("drone_above_roof", force){}
    
    public int Cargo
    {
        get => Get<int>("cargo");
        set => Set<int>("cargo", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
