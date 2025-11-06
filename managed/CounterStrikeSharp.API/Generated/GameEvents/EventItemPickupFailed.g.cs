#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("item_pickup_failed")]
public class EventItemPickupFailed : GameEvent
{
    public EventItemPickupFailed(IntPtr pointer) : base(pointer){}
    public EventItemPickupFailed(bool force) : base("item_pickup_failed", force){}
    
    public string Item
    {
        get => Get<string>("item");
        set => Set<string>("item", value);
    }

    public int Limit
    {
        get => Get<int>("limit");
        set => Set<int>("limit", value);
    }

    public int Reason
    {
        get => Get<int>("reason");
        set => Set<int>("reason", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
