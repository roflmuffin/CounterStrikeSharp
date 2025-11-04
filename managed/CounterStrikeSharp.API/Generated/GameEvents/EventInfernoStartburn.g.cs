#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("inferno_startburn")]
public class EventInfernoStartburn : GameEvent
{
    public EventInfernoStartburn(IntPtr pointer) : base(pointer){}
    public EventInfernoStartburn(bool force) : base("inferno_startburn", force){}
    
    public virtual int Entityid
    {
        get => Get<int>("entityid");
        set => Set<int>("entityid", value);
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
