#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_freeze_end")]
public class EventRoundFreezeEnd : GameEvent
{
    public EventRoundFreezeEnd(IntPtr pointer) : base(pointer){}
    public EventRoundFreezeEnd(bool force) : base("round_freeze_end", force){}
    
}
#nullable restore
