#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("game_phase_changed")]
public class EventGamePhaseChanged : GameEvent
{
    public EventGamePhaseChanged(IntPtr pointer) : base(pointer){}
    public EventGamePhaseChanged(bool force) : base("game_phase_changed", force){}
    
    public int NewPhase
    {
        get => Get<int>("new_phase");
        set => Set<int>("new_phase", value);
    }
}
#nullable restore
