#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("gameinstructor_draw")]
public class EventGameinstructorDraw : GameEvent
{
    public EventGameinstructorDraw(IntPtr pointer) : base(pointer){}
    public EventGameinstructorDraw(bool force) : base("gameinstructor_draw", force){}
    
}
#nullable restore
