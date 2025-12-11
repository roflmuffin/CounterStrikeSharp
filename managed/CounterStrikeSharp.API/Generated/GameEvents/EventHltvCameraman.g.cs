#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_cameraman")]
public class EventHltvCameraman : GameEvent
{
    public EventHltvCameraman(IntPtr pointer) : base(pointer){}
    public EventHltvCameraman(bool force) : base("hltv_cameraman", force){}
    // camera man entity index
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
