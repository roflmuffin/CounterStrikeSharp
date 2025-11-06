#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("choppers_incoming_warning")]
public class EventChoppersIncomingWarning : GameEvent
{
    public EventChoppersIncomingWarning(IntPtr pointer) : base(pointer){}
    public EventChoppersIncomingWarning(bool force) : base("choppers_incoming_warning", force){}
    
    public bool Global
    {
        get => Get<bool>("global");
        set => Set<bool>("global", value);
    }
}
#nullable restore
