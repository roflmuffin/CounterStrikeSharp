#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ammo_pickup")]
public class EventAmmoPickup : GameEvent
{
    public EventAmmoPickup(IntPtr pointer) : base(pointer){}
    public EventAmmoPickup(bool force) : base("ammo_pickup", force){}
    // the weapon entindex
    public long Index
    {
        get => Get<long>("index");
        set => Set<long>("index", value);
    }
// either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
    public string Item
    {
        get => Get<string>("item");
        set => Set<string>("item", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
