#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("local_player_controller_team")]
public class EventLocalPlayerControllerTeam : GameEvent
{
    public EventLocalPlayerControllerTeam(IntPtr pointer) : base(pointer){}
    public EventLocalPlayerControllerTeam(bool force) : base("local_player_controller_team", force){}
    
}
#nullable restore
