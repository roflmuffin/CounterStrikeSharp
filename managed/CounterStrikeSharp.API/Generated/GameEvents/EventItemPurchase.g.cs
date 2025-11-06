#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("item_purchase")]
public class EventItemPurchase : GameEvent
{
    public EventItemPurchase(IntPtr pointer) : base(pointer){}
    public EventItemPurchase(bool force) : base("item_purchase", force){}
    
    public int Loadout
    {
        get => Get<int>("loadout");
        set => Set<int>("loadout", value);
    }

    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public string Weapon
    {
        get => Get<string>("weapon");
        set => Set<string>("weapon", value);
    }
}
#nullable restore
