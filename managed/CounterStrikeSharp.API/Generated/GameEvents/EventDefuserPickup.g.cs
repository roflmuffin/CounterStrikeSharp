#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("defuser_pickup")]
public class EventDefuserPickup : GameEvent
{
    public EventDefuserPickup(IntPtr pointer) : base(pointer){}
    public EventDefuserPickup(bool force) : base("defuser_pickup", force){}
    // defuser's entity ID
    public long Entityid
    {
        get => Get<long>("entityid");
        set => Set<long>("entityid", value);
    }
// player who picked up the defuser
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
