#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("grenade_bounce")]
public class EventGrenadeBounce : GameEvent
{
    public EventGrenadeBounce(IntPtr pointer) : base(pointer){}
    public EventGrenadeBounce(bool force) : base("grenade_bounce", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
