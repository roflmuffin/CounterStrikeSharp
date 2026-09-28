#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("show_survival_respawn_status")]
public class EventShowSurvivalRespawnStatus : GameEvent
{
    public EventShowSurvivalRespawnStatus(IntPtr pointer) : base(pointer){}
    public EventShowSurvivalRespawnStatus(bool force) : base("show_survival_respawn_status", force){}
    
    public long Duration
    {
        get => Get<long>("duration");
        set => Set<long>("duration", value);
    }

    public string LocToken
    {
        get => Get<string>("loc_token");
        set => Set<string>("loc_token", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
