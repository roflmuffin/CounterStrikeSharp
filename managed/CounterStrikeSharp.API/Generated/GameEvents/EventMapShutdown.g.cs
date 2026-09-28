#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("map_shutdown")]
public class EventMapShutdown : GameEvent
{
    public EventMapShutdown(IntPtr pointer) : base(pointer){}
    public EventMapShutdown(bool force) : base("map_shutdown", force){}
    
}
#nullable restore
