#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("weapon_reload")]
public class EventWeaponReload : GameEvent
{
    public EventWeaponReload(IntPtr pointer) : base(pointer){}
    public EventWeaponReload(bool force) : base("weapon_reload", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
