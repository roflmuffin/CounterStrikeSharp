#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("start_halftime")]
public class EventStartHalftime : GameEvent
{
    public EventStartHalftime(IntPtr pointer) : base(pointer){}
    public EventStartHalftime(bool force) : base("start_halftime", force){}
    
}
#nullable restore
