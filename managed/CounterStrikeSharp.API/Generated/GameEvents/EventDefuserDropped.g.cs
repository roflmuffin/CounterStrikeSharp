#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("defuser_dropped")]
public class EventDefuserDropped : GameEvent
{
    public EventDefuserDropped(IntPtr pointer) : base(pointer){}
    public EventDefuserDropped(bool force) : base("defuser_dropped", force){}
    // defuser's entity ID
    public long Entityid
    {
        get => Get<long>("entityid");
        set => Set<long>("entityid", value);
    }
}
#nullable restore
