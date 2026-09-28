#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("buytime_ended")]
public class EventBuytimeEnded : GameEvent
{
    public EventBuytimeEnded(IntPtr pointer) : base(pointer){}
    public EventBuytimeEnded(bool force) : base("buytime_ended", force){}
    
}
#nullable restore
