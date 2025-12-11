#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("jointeam_failed")]
public class EventJointeamFailed : GameEvent
{
    public EventJointeamFailed(IntPtr pointer) : base(pointer){}
    public EventJointeamFailed(bool force) : base("jointeam_failed", force){}
    // 0 = team_full
    public int Reason
    {
        get => Get<int>("reason");
        set => Set<int>("reason", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
