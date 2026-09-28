#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("demo_stop")]
public class EventDemoStop : GameEvent
{
    public EventDemoStop(IntPtr pointer) : base(pointer){}
    public EventDemoStop(bool force) : base("demo_stop", force){}
    
}
#nullable restore
