#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("weapon_zoom")]
public class EventWeaponZoom : GameEvent
{
    public EventWeaponZoom(IntPtr pointer) : base(pointer){}
    public EventWeaponZoom(bool force) : base("weapon_zoom", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
