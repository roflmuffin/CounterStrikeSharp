#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_poststart")]
public class EventRoundPoststart : GameEvent
{
    public EventRoundPoststart(IntPtr pointer) : base(pointer){}
    public EventRoundPoststart(bool force) : base("round_poststart", force){}
    
}
#nullable restore
