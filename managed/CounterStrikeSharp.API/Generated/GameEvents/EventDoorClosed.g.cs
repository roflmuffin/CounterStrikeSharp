#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("door_closed")]
public class EventDoorClosed : GameEvent
{
    public EventDoorClosed(IntPtr pointer) : base(pointer){}
    public EventDoorClosed(bool force) : base("door_closed", force){}
    
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
// Who closed the door
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
