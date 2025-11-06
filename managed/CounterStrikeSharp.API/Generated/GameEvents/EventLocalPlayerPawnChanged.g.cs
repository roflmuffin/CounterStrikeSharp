#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("local_player_pawn_changed")]
public class EventLocalPlayerPawnChanged : GameEvent
{
    public EventLocalPlayerPawnChanged(IntPtr pointer) : base(pointer){}
    public EventLocalPlayerPawnChanged(bool force) : base("local_player_pawn_changed", force){}
    
}
#nullable restore
