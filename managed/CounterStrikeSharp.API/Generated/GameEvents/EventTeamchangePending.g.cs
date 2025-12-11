#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("teamchange_pending")]
public class EventTeamchangePending : GameEvent
{
    public EventTeamchangePending(IntPtr pointer) : base(pointer){}
    public EventTeamchangePending(bool force) : base("teamchange_pending", force){}
    
    public int Toteam
    {
        get => Get<int>("toteam");
        set => Set<int>("toteam", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
