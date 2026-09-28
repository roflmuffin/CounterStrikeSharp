#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("drop_rate_modified")]
public class EventDropRateModified : GameEvent
{
    public EventDropRateModified(IntPtr pointer) : base(pointer){}
    public EventDropRateModified(bool force) : base("drop_rate_modified", force){}
    
}
#nullable restore
