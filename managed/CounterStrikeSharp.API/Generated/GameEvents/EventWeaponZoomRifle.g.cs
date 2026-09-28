#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("weapon_zoom_rifle")]
public class EventWeaponZoomRifle : GameEvent
{
    public EventWeaponZoomRifle(IntPtr pointer) : base(pointer){}
    public EventWeaponZoomRifle(bool force) : base("weapon_zoom_rifle", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
