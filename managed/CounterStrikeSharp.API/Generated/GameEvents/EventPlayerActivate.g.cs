#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_activate")]
public class EventPlayerActivate : GameEvent
{
    public EventPlayerActivate(IntPtr pointer) : base(pointer){}
    public EventPlayerActivate(bool force) : base("player_activate", force){}
    // user ID on server
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
