#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("weapon_fire_on_empty")]
public class EventWeaponFireOnEmpty : GameEvent
{
    public EventWeaponFireOnEmpty(IntPtr pointer) : base(pointer){}
    public EventWeaponFireOnEmpty(bool force) : base("weapon_fire_on_empty", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// weapon name used
    public string Weapon
    {
        get => Get<string>("weapon");
        set => Set<string>("weapon", value);
    }
}
#nullable restore
