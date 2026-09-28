#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("dm_bonus_weapon_start")]
public class EventDmBonusWeaponStart : GameEvent
{
    public EventDmBonusWeaponStart(IntPtr pointer) : base(pointer){}
    public EventDmBonusWeaponStart(bool force) : base("dm_bonus_weapon_start", force){}
    // Loadout position of the bonus weapon
    public int Pos
    {
        get => Get<int>("Pos");
        set => Set<int>("Pos", value);
    }
// The length of time that this bonus lasts
    public int Time
    {
        get => Get<int>("time");
        set => Set<int>("time", value);
    }
}
#nullable restore
