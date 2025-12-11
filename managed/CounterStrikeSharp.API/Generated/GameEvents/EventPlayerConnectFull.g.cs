#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_connect_full")]
public class EventPlayerConnectFull : GameEvent
{
    public EventPlayerConnectFull(IntPtr pointer) : base(pointer){}
    public EventPlayerConnectFull(bool force) : base("player_connect_full", force){}
    // user ID on server (unique on server)
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
