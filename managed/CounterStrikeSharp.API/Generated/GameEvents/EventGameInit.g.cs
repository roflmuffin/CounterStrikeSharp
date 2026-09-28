#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("game_init")]
public class EventGameInit : GameEvent
{
    public EventGameInit(IntPtr pointer) : base(pointer){}
    public EventGameInit(bool force) : base("game_init", force){}
    
}
#nullable restore
