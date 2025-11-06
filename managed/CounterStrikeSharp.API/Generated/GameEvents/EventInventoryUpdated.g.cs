#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("inventory_updated")]
public class EventInventoryUpdated : GameEvent
{
    public EventInventoryUpdated(IntPtr pointer) : base(pointer){}
    public EventInventoryUpdated(bool force) : base("inventory_updated", force){}
    
    public int Itemdef
    {
        get => Get<int>("itemdef");
        set => Set<int>("itemdef", value);
    }

    public long Itemid
    {
        get => Get<long>("itemid");
        set => Set<long>("itemid", value);
    }
}
#nullable restore
