#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("start_vote")]
public class EventStartVote : GameEvent
{
    public EventStartVote(IntPtr pointer) : base(pointer){}
    public EventStartVote(bool force) : base("start_vote", force){}
    
    public int Type
    {
        get => Get<int>("type");
        set => Set<int>("type", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public int VoteParameter
    {
        get => Get<int>("vote_parameter");
        set => Set<int>("vote_parameter", value);
    }
}
#nullable restore
