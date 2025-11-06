#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("door_close")]
public class EventDoorClose : GameEvent
{
    public EventDoorClose(IntPtr pointer) : base(pointer){}
    public EventDoorClose(bool force) : base("door_close", force){}
    // Is the door a checkpoint door
    public bool Checkpoint
    {
        get => Get<bool>("checkpoint");
        set => Set<bool>("checkpoint", value);
    }
// Who closed the door
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
