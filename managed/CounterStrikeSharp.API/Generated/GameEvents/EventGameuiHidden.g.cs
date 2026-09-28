#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("gameui_hidden")]
public class EventGameuiHidden : GameEvent
{
    public EventGameuiHidden(IntPtr pointer) : base(pointer){}
    public EventGameuiHidden(bool force) : base("gameui_hidden", force){}
    
}
#nullable restore
