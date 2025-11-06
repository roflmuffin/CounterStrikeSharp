#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_game_disconnected")]
public class EventCsGameDisconnected : GameEvent
{
    public EventCsGameDisconnected(IntPtr pointer) : base(pointer){}
    public EventCsGameDisconnected(bool force) : base("cs_game_disconnected", force){}
    
}
#nullable restore
