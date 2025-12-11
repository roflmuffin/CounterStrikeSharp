#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_team")]
public class EventPlayerTeam : GameEvent
{
    public EventPlayerTeam(IntPtr pointer) : base(pointer){}
    public EventPlayerTeam(bool force) : base("player_team", force){}
    // team change because player disconnects
    public bool Disconnect
    {
        get => Get<bool>("disconnect");
        set => Set<bool>("disconnect", value);
    }
// true if player is a bot
    public bool Isbot
    {
        get => Get<bool>("isbot");
        set => Set<bool>("isbot", value);
    }

    public string Name
    {
        get => Get<string>("name");
        set => Set<string>("name", value);
    }
// old team id
    public int Oldteam
    {
        get => Get<int>("oldteam");
        set => Set<int>("oldteam", value);
    }

    public bool Silent
    {
        get => Get<bool>("silent");
        set => Set<bool>("silent", value);
    }
// team id
    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }
// player
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
