#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("smoke_beacon_paradrop")]
public class EventSmokeBeaconParadrop : GameEvent
{
    public EventSmokeBeaconParadrop(IntPtr pointer) : base(pointer){}
    public EventSmokeBeaconParadrop(bool force) : base("smoke_beacon_paradrop", force){}
    
    public int Paradrop
    {
        get => Get<int>("paradrop");
        set => Set<int>("paradrop", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
