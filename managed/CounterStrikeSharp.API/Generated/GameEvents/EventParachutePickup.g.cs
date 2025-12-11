#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("parachute_pickup")]
public class EventParachutePickup : GameEvent
{
    public EventParachutePickup(IntPtr pointer) : base(pointer){}
    public EventParachutePickup(bool force) : base("parachute_pickup", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
