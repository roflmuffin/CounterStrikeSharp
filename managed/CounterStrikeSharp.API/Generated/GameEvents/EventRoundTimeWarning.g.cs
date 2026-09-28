#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_time_warning")]
public class EventRoundTimeWarning : GameEvent
{
    public EventRoundTimeWarning(IntPtr pointer) : base(pointer){}
    public EventRoundTimeWarning(bool force) : base("round_time_warning", force){}
    
}
#nullable restore
