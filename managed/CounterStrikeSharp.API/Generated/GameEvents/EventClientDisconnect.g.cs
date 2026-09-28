#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("client_disconnect")]
public class EventClientDisconnect : GameEvent
{
    public EventClientDisconnect(IntPtr pointer) : base(pointer){}
    public EventClientDisconnect(bool force) : base("client_disconnect", force){}
    
}
#nullable restore
