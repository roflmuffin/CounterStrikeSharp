#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("enter_buyzone")]
public class EventEnterBuyzone : GameEvent
{
    public EventEnterBuyzone(IntPtr pointer) : base(pointer){}
    public EventEnterBuyzone(bool force) : base("enter_buyzone", force){}
    
    public bool Canbuy
    {
        get => Get<bool>("canbuy");
        set => Set<bool>("canbuy", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
