#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_rescued_all")]
public class EventHostageRescuedAll : GameEvent
{
    public EventHostageRescuedAll(IntPtr pointer) : base(pointer){}
    public EventHostageRescuedAll(bool force) : base("hostage_rescued_all", force){}
    
}
#nullable restore
