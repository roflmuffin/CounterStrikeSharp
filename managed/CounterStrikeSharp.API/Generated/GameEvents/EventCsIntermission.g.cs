#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_intermission")]
public class EventCsIntermission : GameEvent
{
    public EventCsIntermission(IntPtr pointer) : base(pointer){}
    public EventCsIntermission(bool force) : base("cs_intermission", force){}
    
}
#nullable restore
