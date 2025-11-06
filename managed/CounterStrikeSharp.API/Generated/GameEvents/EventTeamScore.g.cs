#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("team_score")]
public class EventTeamScore : GameEvent
{
    public EventTeamScore(IntPtr pointer) : base(pointer){}
    public EventTeamScore(bool force) : base("team_score", force){}
    // total team score
    public int Score
    {
        get => Get<int>("score");
        set => Set<int>("score", value);
    }
// team id
    public int Teamid
    {
        get => Get<int>("teamid");
        set => Set<int>("teamid", value);
    }
}
#nullable restore
