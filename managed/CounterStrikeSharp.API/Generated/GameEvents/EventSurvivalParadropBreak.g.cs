#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("survival_paradrop_break")]
public class EventSurvivalParadropBreak : GameEvent
{
    public EventSurvivalParadropBreak(IntPtr pointer) : base(pointer){}
    public EventSurvivalParadropBreak(bool force) : base("survival_paradrop_break", force){}
    
    public int Entityid
    {
        get => Get<int>("entityid");
        set => Set<int>("entityid", value);
    }
}
#nullable restore
