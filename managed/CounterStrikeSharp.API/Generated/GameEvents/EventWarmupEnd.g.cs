#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("warmup_end")]
public class EventWarmupEnd : GameEvent
{
    public EventWarmupEnd(IntPtr pointer) : base(pointer){}
    public EventWarmupEnd(bool force) : base("warmup_end", force){}
    
}
#nullable restore
