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
    
    public virtual float Duration
    {
        get => Get<float>("duration");
        set => Set<float>("duration", value);
    }

    public virtual int Radius
    {
        get => Get<int>("radius");
        set => Set<int>("radius", value);
    }

    public virtual bool Step
    {
        get => Get<bool>("step");
        set => Set<bool>("step", value);
    }

    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
