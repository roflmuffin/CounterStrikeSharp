#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_replay")]
public class EventHltvReplay : GameEvent
{
    public EventHltvReplay(IntPtr pointer) : base(pointer){}
    public EventHltvReplay(bool force) : base("hltv_replay", force){}
    // number of seconds in killer replay delay
    public long Delay
    {
        get => Get<long>("delay");
        set => Set<long>("delay", value);
    }
// reason for replay	(ReplayEventType_t)
    public long Reason
    {
        get => Get<long>("reason");
        set => Set<long>("reason", value);
    }
}
#nullable restore
