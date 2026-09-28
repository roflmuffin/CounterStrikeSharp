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
    
    public int Entityid
    {
        get => Get<int>("entityid");
        set => Set<int>("entityid", value);
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
