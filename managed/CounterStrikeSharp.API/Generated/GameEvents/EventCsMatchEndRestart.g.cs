#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_match_end_restart")]
public class EventCsMatchEndRestart : GameEvent
{
    public EventCsMatchEndRestart(IntPtr pointer) : base(pointer){}
    public EventCsMatchEndRestart(bool force) : base("cs_match_end_restart", force){}
    
}
#nullable restore
