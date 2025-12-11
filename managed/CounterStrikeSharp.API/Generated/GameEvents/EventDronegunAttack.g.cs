#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("dronegun_attack")]
public class EventDronegunAttack : GameEvent
{
    public EventDronegunAttack(IntPtr pointer) : base(pointer){}
    public EventDronegunAttack(bool force) : base("dronegun_attack", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
