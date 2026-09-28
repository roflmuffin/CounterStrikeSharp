#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("survival_paradrop_spawn")]
public class EventSurvivalParadropSpawn : GameEvent
{
    public EventSurvivalParadropSpawn(IntPtr pointer) : base(pointer){}
    public EventSurvivalParadropSpawn(bool force) : base("survival_paradrop_spawn", force){}
    
    public int Entityid
    {
        get => Get<int>("entityid");
        set => Set<int>("entityid", value);
    }
}
#nullable restore
