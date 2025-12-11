#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_replay_status")]
public class EventHltvReplayStatus : GameEvent
{
    public EventHltvReplayStatus(IntPtr pointer) : base(pointer){}
    public EventHltvReplayStatus(bool force) : base("hltv_replay_status", force){}
    // reason for hltv replay status change ()
    public long Reason
    {
        get => Get<long>("reason");
        set => Set<long>("reason", value);
    }
}
#nullable restore
