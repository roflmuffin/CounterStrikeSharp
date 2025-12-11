#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_round_start_beep")]
public class EventCsRoundStartBeep : GameEvent
{
    public EventCsRoundStartBeep(IntPtr pointer) : base(pointer){}
    public EventCsRoundStartBeep(bool force) : base("cs_round_start_beep", force){}
    
}
#nullable restore
