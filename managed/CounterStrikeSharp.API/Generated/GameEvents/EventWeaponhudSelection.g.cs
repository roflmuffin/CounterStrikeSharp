#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("weaponhud_selection")]
public class EventWeaponhudSelection : GameEvent
{
    public EventWeaponhudSelection(IntPtr pointer) : base(pointer){}
    public EventWeaponhudSelection(bool force) : base("weaponhud_selection", force){}
    // Weapon entity index
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
// EWeaponHudSelectionMode (switch / pickup / drop)
    public int Mode
    {
        get => Get<int>("mode");
        set => Set<int>("mode", value);
    }
// Player who this event applies to
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
