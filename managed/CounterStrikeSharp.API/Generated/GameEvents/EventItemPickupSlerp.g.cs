#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("item_pickup_slerp")]
public class EventItemPickupSlerp : GameEvent
{
    public EventItemPickupSlerp(IntPtr pointer) : base(pointer){}
    public EventItemPickupSlerp(bool force) : base("item_pickup_slerp", force){}
    
    public int Behavior
    {
        get => Get<int>("behavior");
        set => Set<int>("behavior", value);
    }

    public int Index
    {
        get => Get<int>("index");
        set => Set<int>("index", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
