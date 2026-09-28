#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_cast_no")]
public class EventVoteCastNo : GameEvent
{
    public EventVoteCastNo(IntPtr pointer) : base(pointer){}
    public EventVoteCastNo(bool force) : base("vote_cast_no", force){}
    // entity id of the voter
    public long Entityid
    {
        get => Get<long>("entityid");
        set => Set<long>("entityid", value);
    }

    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }
}
#nullable restore
