#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_round_final_beep")]
public class EventCsRoundFinalBeep : GameEvent
{
    public EventCsRoundFinalBeep(IntPtr pointer) : base(pointer){}
    public EventCsRoundFinalBeep(bool force) : base("cs_round_final_beep", force){}
    
}
#nullable restore
