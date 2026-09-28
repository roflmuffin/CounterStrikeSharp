#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("event_ticket_modified")]
public class EventEventTicketModified : GameEvent
{
    public EventEventTicketModified(IntPtr pointer) : base(pointer){}
    public EventEventTicketModified(bool force) : base("event_ticket_modified", force){}
    
}
#nullable restore
