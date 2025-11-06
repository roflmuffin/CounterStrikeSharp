#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("survival_teammate_respawn")]
public class EventSurvivalTeammateRespawn : GameEvent
{
    public EventSurvivalTeammateRespawn(IntPtr pointer) : base(pointer){}
    public EventSurvivalTeammateRespawn(bool force) : base("survival_teammate_respawn", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
