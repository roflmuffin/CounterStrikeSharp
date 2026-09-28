#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vip_escaped")]
public class EventVipEscaped : GameEvent
{
    public EventVipEscaped(IntPtr pointer) : base(pointer){}
    public EventVipEscaped(bool force) : base("vip_escaped", force){}
    // player who was the VIP
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
