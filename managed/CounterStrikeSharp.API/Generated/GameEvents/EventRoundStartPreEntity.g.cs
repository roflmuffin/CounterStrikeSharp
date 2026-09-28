#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_start_pre_entity")]
public class EventRoundStartPreEntity : GameEvent
{
    public EventRoundStartPreEntity(IntPtr pointer) : base(pointer){}
    public EventRoundStartPreEntity(bool force) : base("round_start_pre_entity", force){}
    
}
#nullable restore
