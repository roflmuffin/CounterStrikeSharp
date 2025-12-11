#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("exit_rescue_zone")]
public class EventExitRescueZone : GameEvent
{
    public EventExitRescueZone(IntPtr pointer) : base(pointer){}
    public EventExitRescueZone(bool force) : base("exit_rescue_zone", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
