#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("teamplay_round_start")]
public class EventTeamplayRoundStart : GameEvent
{
    public EventTeamplayRoundStart(IntPtr pointer) : base(pointer){}
    public EventTeamplayRoundStart(bool force) : base("teamplay_round_start", force){}
    // is this a full reset of the map
    public bool FullReset
    {
        get => Get<bool>("full_reset");
        set => Set<bool>("full_reset", value);
    }
}
#nullable restore
