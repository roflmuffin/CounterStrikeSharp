#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("helicopter_grenade_punt_miss")]
public class EventHelicopterGrenadePuntMiss : GameEvent
{
    public EventHelicopterGrenadePuntMiss(IntPtr pointer) : base(pointer){}
    public EventHelicopterGrenadePuntMiss(bool force) : base("helicopter_grenade_punt_miss", force){}
    
}
#nullable restore
