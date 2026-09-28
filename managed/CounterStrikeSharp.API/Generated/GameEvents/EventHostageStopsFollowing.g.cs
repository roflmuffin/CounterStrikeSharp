#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_stops_following")]
public class EventHostageStopsFollowing : GameEvent
{
    public EventHostageStopsFollowing(IntPtr pointer) : base(pointer){}
    public EventHostageStopsFollowing(bool force) : base("hostage_stops_following", force){}
    // hostage entity index
    public int Hostage
    {
        get => Get<int>("hostage");
        set => Set<int>("hostage", value);
    }
// player who rescued the hostage
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
