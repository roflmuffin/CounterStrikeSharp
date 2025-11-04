#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("molotov_detonate")]
public class EventMolotovDetonate : GameEvent
{
    public EventMolotovDetonate(IntPtr pointer) : base(pointer){}
    public EventMolotovDetonate(bool force) : base("molotov_detonate", force){}
    
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public virtual float X
    {
        get => Get<float>("x");
        set => Set<float>("x", value);
    }

    public virtual float Y
    {
        get => Get<float>("y");
        set => Set<float>("y", value);
    }

    public virtual float Z
    {
        get => Get<float>("z");
        set => Set<float>("z", value);
    }
}
#nullable restore
