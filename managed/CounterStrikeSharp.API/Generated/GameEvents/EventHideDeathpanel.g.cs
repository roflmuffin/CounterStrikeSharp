#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hide_deathpanel")]
public class EventHideDeathpanel : GameEvent
{
    public EventHideDeathpanel(IntPtr pointer) : base(pointer){}
    public EventHideDeathpanel(bool force) : base("hide_deathpanel", force){}
    
}
#nullable restore
