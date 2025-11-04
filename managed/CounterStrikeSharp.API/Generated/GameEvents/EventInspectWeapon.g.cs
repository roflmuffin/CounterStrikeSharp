#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("inspect_weapon")]
public class EventInspectWeapon : GameEvent
{
    public EventInspectWeapon(IntPtr pointer) : base(pointer){}
    public EventInspectWeapon(bool force) : base("inspect_weapon", force){}
    
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
