#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("gc_connected")]
public class EventGcConnected : GameEvent
{
    public EventGcConnected(IntPtr pointer) : base(pointer){}
    public EventGcConnected(bool force) : base("gc_connected", force){}
    
}
#nullable restore
