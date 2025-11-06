#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_cast")]
public class EventVoteCast : GameEvent
{
    public EventVoteCast(IntPtr pointer) : base(pointer){}
    public EventVoteCast(bool force) : base("vote_cast", force){}
    
    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }
// player who voted
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// which option the player voted on
    public int VoteOption
    {
        get => Get<int>("vote_option");
        set => Set<int>("vote_option", value);
    }
}
#nullable restore
