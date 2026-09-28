#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cart_updated")]
public class EventCartUpdated : GameEvent
{
    public EventCartUpdated(IntPtr pointer) : base(pointer){}
    public EventCartUpdated(bool force) : base("cart_updated", force){}
    
}
#nullable restore
