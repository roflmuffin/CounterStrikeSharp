#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("tagrenade_detonate")]
public class EventTagrenadeDetonate : GameEvent
{
    public EventTagrenadeDetonate(IntPtr pointer) : base(pointer){}
    public EventTagrenadeDetonate(bool force) : base("tagrenade_detonate", force){}
    
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

    public float X
    {
        get => Get<float>("x");
        set => Set<float>("x", value);
    }

    public float Y
    {
        get => Get<float>("y");
        set => Set<float>("y", value);
    }

    public float Z
    {
        get => Get<float>("z");
        set => Set<float>("z", value);
    }
}
#nullable restore
