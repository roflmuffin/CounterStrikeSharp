#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("buymenu_open")]
public class EventBuymenuOpen : GameEvent
{
    public EventBuymenuOpen(IntPtr pointer) : base(pointer){}
    public EventBuymenuOpen(bool force) : base("buymenu_open", force){}
    
}
#nullable restore
