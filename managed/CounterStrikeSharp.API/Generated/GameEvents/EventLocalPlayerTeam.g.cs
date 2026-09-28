#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("local_player_team")]
public class EventLocalPlayerTeam : GameEvent
{
    public EventLocalPlayerTeam(IntPtr pointer) : base(pointer){}
    public EventLocalPlayerTeam(bool force) : base("local_player_team", force){}
    
}
#nullable restore
