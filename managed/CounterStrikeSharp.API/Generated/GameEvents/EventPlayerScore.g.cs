#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_score")]
public class EventPlayerScore : GameEvent
{
    public EventPlayerScore(IntPtr pointer) : base(pointer){}
    public EventPlayerScore(bool force) : base("player_score", force){}
    // # of deaths
    public virtual int Deaths
    {
        get => Get<int>("deaths");
        set => Set<int>("deaths", value);
    }
// # of kills
    public virtual int Kills
    {
        get => Get<int>("kills");
        set => Set<int>("kills", value);
    }
// total game score
    public virtual int Score
    {
        get => Get<int>("score");
        set => Set<int>("score", value);
    }
// user ID on server
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
