#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_officially_ended")]
public class EventRoundOfficiallyEnded : GameEvent
{
    public EventRoundOfficiallyEnded(IntPtr pointer) : base(pointer){}
    public EventRoundOfficiallyEnded(bool force) : base("round_officially_ended", force){}
    
}
#nullable restore
