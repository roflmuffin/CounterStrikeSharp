#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("gameinstructor_nodraw")]
public class EventGameinstructorNodraw : GameEvent
{
    public EventGameinstructorNodraw(IntPtr pointer) : base(pointer){}
    public EventGameinstructorNodraw(bool force) : base("gameinstructor_nodraw", force){}
    
}
#nullable restore
