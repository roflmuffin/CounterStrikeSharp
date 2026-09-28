#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("survival_no_respawns_warning")]
public class EventSurvivalNoRespawnsWarning : GameEvent
{
    public EventSurvivalNoRespawnsWarning(IntPtr pointer) : base(pointer){}
    public EventSurvivalNoRespawnsWarning(bool force) : base("survival_no_respawns_warning", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
