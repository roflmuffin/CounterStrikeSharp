#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_prev_next_spectator")]
public class EventCsPrevNextSpectator : GameEvent
{
    public EventCsPrevNextSpectator(IntPtr pointer) : base(pointer){}
    public EventCsPrevNextSpectator(bool force) : base("cs_prev_next_spectator", force){}
    
    public bool Next
    {
        get => Get<bool>("next");
        set => Set<bool>("next", value);
    }
}
#nullable restore
