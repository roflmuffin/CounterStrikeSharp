#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("announce_phase_end")]
public class EventAnnouncePhaseEnd : GameEvent
{
    public EventAnnouncePhaseEnd(IntPtr pointer) : base(pointer){}
    public EventAnnouncePhaseEnd(bool force) : base("announce_phase_end", force){}
    
}
#nullable restore
