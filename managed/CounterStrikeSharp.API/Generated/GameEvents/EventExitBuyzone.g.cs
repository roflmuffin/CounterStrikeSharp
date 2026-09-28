#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("exit_buyzone")]
public class EventExitBuyzone : GameEvent
{
    public EventExitBuyzone(IntPtr pointer) : base(pointer){}
    public EventExitBuyzone(bool force) : base("exit_buyzone", force){}
    
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
