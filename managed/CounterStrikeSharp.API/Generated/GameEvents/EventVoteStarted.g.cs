#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_started")]
public class EventVoteStarted : GameEvent
{
    public EventVoteStarted(IntPtr pointer) : base(pointer){}
    public EventVoteStarted(bool force) : base("vote_started", force){}
    // entity id of the player who initiated the vote
    public long Initiator
    {
        get => Get<long>("initiator");
        set => Set<long>("initiator", value);
    }

    public string Issue
    {
        get => Get<string>("issue");
        set => Set<string>("issue", value);
    }

    public string Param1
    {
        get => Get<string>("param1");
        set => Set<string>("param1", value);
    }
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

    public string Votedata
    {
        get => Get<string>("votedata");
        set => Set<string>("votedata", value);
    }
}
#nullable restore
