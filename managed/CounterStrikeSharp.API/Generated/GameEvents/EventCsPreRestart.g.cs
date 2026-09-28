#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_pre_restart")]
public class EventCsPreRestart : GameEvent
{
    public EventCsPreRestart(IntPtr pointer) : base(pointer){}
    public EventCsPreRestart(bool force) : base("cs_pre_restart", force){}
    
}
#nullable restore
