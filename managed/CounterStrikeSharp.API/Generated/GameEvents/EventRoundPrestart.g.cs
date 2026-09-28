#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_prestart")]
public class EventRoundPrestart : GameEvent
{
    public EventRoundPrestart(IntPtr pointer) : base(pointer){}
    public EventRoundPrestart(bool force) : base("round_prestart", force){}
    
}
#nullable restore
