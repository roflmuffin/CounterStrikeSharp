#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ammo_refill")]
public class EventAmmoRefill : GameEvent
{
    public EventAmmoRefill(IntPtr pointer) : base(pointer){}
    public EventAmmoRefill(bool force) : base("ammo_refill", force){}
    
    public bool Success
    {
        get => Get<bool>("success");
        set => Set<bool>("success", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
