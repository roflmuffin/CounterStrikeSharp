#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("parachute_deploy")]
public class EventParachuteDeploy : GameEvent
{
    public EventParachuteDeploy(IntPtr pointer) : base(pointer){}
    public EventParachuteDeploy(bool force) : base("parachute_deploy", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
