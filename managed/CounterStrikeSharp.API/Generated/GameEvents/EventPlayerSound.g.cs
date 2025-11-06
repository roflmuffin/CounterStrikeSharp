#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_sound")]
public class EventPlayerSound : GameEvent
{
    public EventPlayerSound(IntPtr pointer) : base(pointer){}
    public EventPlayerSound(bool force) : base("player_sound", force){}
    
    public float Duration
    {
        get => Get<float>("duration");
        set => Set<float>("duration", value);
    }

    public int Radius
    {
        get => Get<int>("radius");
        set => Set<int>("radius", value);
    }

    public bool Step
    {
        get => Get<bool>("step");
        set => Set<bool>("step", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
