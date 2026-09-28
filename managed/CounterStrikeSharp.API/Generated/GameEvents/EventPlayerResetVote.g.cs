#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_reset_vote")]
public class EventPlayerResetVote : GameEvent
{
    public EventPlayerResetVote(IntPtr pointer) : base(pointer){}
    public EventPlayerResetVote(bool force) : base("player_reset_vote", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public bool Vote
    {
        get => Get<bool>("vote");
        set => Set<bool>("vote", value);
    }
}
#nullable restore
