#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("enter_rescue_zone")]
public class EventEnterRescueZone : GameEvent
{
    public EventEnterRescueZone(IntPtr pointer) : base(pointer){}
    public EventEnterRescueZone(bool force) : base("enter_rescue_zone", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
