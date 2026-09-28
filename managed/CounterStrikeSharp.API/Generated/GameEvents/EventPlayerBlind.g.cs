#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_blind")]
public class EventPlayerBlind : GameEvent
{
    public EventPlayerBlind(IntPtr pointer) : base(pointer){}
    public EventPlayerBlind(bool force) : base("player_blind", force){}
    // user ID who threw the flash
    public CCSPlayerController? Attacker
    {
        get => GetPlayer("attacker");
        set => SetPlayer("attacker", value);
    }

    public float BlindDuration
    {
        get => Get<float>("blind_duration");
        set => Set<float>("blind_duration", value);
    }
// the flashbang going off
    public int Entityid
    {
        get => Get<int>("entityid");
        set => Set<int>("entityid", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
