#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_changed_mode")]
public class EventHltvChangedMode : GameEvent
{
    public EventHltvChangedMode(IntPtr pointer) : base(pointer){}
    public EventHltvChangedMode(bool force) : base("hltv_changed_mode", force){}
    
    public long Newmode
    {
        get => Get<long>("newmode");
        set => Set<long>("newmode", value);
    }

    public long ObsTarget
    {
        get => Get<long>("obs_target");
        set => Set<long>("obs_target", value);
    }

    public long Oldmode
    {
        get => Get<long>("oldmode");
        set => Set<long>("oldmode", value);
    }
}
#nullable restore
