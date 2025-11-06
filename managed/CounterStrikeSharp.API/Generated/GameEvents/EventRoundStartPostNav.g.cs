#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_start_post_nav")]
public class EventRoundStartPostNav : GameEvent
{
    public EventRoundStartPostNav(IntPtr pointer) : base(pointer){}
    public EventRoundStartPostNav(bool force) : base("round_start_post_nav", force){}
    
}
#nullable restore
