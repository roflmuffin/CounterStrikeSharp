#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("survival_no_respawns_final")]
public class EventSurvivalNoRespawnsFinal : GameEvent
{
    public EventSurvivalNoRespawnsFinal(IntPtr pointer) : base(pointer){}
    public EventSurvivalNoRespawnsFinal(bool force) : base("survival_no_respawns_final", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
