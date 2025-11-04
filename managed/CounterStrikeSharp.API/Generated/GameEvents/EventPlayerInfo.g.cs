#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_info")]
public class EventPlayerInfo : GameEvent
{
    public EventPlayerInfo(IntPtr pointer) : base(pointer){}
    public EventPlayerInfo(bool force) : base("player_info", force){}
    // true if player is a AI bot
    public virtual bool Bot
    {
        get => Get<bool>("bot");
        set => Set<bool>("bot", value);
    }
// player name
    public virtual string Name
    {
        get => Get<string>("name");
        set => Set<string>("name", value);
    }
// player network (i.e steam) id
    public virtual ulong Steamid
    {
        get => Get<ulong>("steamid");
        set => Set<ulong>("steamid", value);
    }
// user ID on server (unique on server)
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
