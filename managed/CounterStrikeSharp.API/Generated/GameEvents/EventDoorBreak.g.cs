#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("door_break")]
public class EventDoorBreak : GameEvent
{
    public EventDoorBreak(IntPtr pointer) : base(pointer){}
    public EventDoorBreak(bool force) : base("door_break", force){}
    
    public long Dmgstate
    {
        get => Get<long>("dmgstate");
        set => Set<long>("dmgstate", value);
    }

    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
}
#nullable restore
