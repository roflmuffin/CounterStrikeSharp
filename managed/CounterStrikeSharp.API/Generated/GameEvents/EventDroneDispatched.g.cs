#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("drone_dispatched")]
public class EventDroneDispatched : GameEvent
{
    public EventDroneDispatched(IntPtr pointer) : base(pointer){}
    public EventDroneDispatched(bool force) : base("drone_dispatched", force){}
    
    public int DroneDispatchedParam
    {
        get => Get<int>("drone_dispatched");
        set => Set<int>("drone_dispatched", value);
    }

    public int Priority
    {
        get => Get<int>("priority");
        set => Set<int>("priority", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
