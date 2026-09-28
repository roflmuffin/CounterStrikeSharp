#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("store_pricesheet_updated")]
public class EventStorePricesheetUpdated : GameEvent
{
    public EventStorePricesheetUpdated(IntPtr pointer) : base(pointer){}
    public EventStorePricesheetUpdated(bool force) : base("store_pricesheet_updated", force){}
    
}
#nullable restore
