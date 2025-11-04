#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("begin_new_match")]
public class EventBeginNewMatch : GameEvent
{
    public EventBeginNewMatch(IntPtr pointer) : base(pointer){}
    public EventBeginNewMatch(bool force) : base("begin_new_match", force){}
    
}
#nullable restore
