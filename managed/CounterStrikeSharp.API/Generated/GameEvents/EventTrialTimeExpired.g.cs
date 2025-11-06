#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("trial_time_expired")]
public class EventTrialTimeExpired : GameEvent
{
    public EventTrialTimeExpired(IntPtr pointer) : base(pointer){}
    public EventTrialTimeExpired(bool force) : base("trial_time_expired", force){}
    // player whose time has expired
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
