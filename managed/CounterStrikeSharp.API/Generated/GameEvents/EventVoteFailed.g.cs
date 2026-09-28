#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_failed")]
public class EventVoteFailed : GameEvent
{
    public EventVoteFailed(IntPtr pointer) : base(pointer){}
    public EventVoteFailed(bool force) : base("vote_failed", force){}
    // this event is reliable
    public int Reliable
    {
        get => Get<int>("reliable");
        set => Set<int>("reliable", value);
    }

    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }
}
#nullable restore
