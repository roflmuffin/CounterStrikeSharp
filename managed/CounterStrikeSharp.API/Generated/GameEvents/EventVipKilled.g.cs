#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vip_killed")]
public class EventVipKilled : GameEvent
{
    public EventVipKilled(IntPtr pointer) : base(pointer){}
    public EventVipKilled(bool force) : base("vip_killed", force){}
    // user ID who killed the VIP
    public CCSPlayerController? Attacker
    {
        get => GetPlayer("attacker");
        set => SetPlayer("attacker", value);
    }
// player who was the VIP
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
