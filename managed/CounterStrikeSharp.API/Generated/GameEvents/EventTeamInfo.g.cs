#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("team_info")]
public class EventTeamInfo : GameEvent
{
    public EventTeamInfo(IntPtr pointer) : base(pointer){}
    public EventTeamInfo(bool force) : base("team_info", force){}
    // unique team id
    public int Teamid
    {
        get => Get<int>("teamid");
        set => Set<int>("teamid", value);
    }
// team name eg "Team Blue"
    public string Teamname
    {
        get => Get<string>("teamname");
        set => Set<string>("teamname", value);
    }
}
#nullable restore
