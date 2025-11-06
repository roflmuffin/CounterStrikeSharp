#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("weapon_fire")]
public class EventWeaponFire : GameEvent
{
    public EventWeaponFire(IntPtr pointer) : base(pointer){}
    public EventWeaponFire(bool force) : base("weapon_fire", force){}
    // is weapon silenced
    public bool Silenced
    {
        get => Get<bool>("silenced");
        set => Set<bool>("silenced", value);
    }

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
