#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("map_transition")]
public class EventMapTransition : GameEvent
{
    public EventMapTransition(IntPtr pointer) : base(pointer){}
    public EventMapTransition(bool force) : base("map_transition", force){}
    
}
#nullable restore
