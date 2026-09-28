#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_announce_final")]
public class EventRoundAnnounceFinal : GameEvent
{
    public EventRoundAnnounceFinal(IntPtr pointer) : base(pointer){}
    public EventRoundAnnounceFinal(bool force) : base("round_announce_final", force){}
    
}
#nullable restore
