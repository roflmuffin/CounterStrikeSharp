#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_connect")]
public class EventPlayerConnect : GameEvent
{
    public EventPlayerConnect(IntPtr pointer) : base(pointer){}
    public EventPlayerConnect(bool force) : base("player_connect", force){}
    
    public bool Bot
    {
        get => Get<bool>("bot");
        set => Set<bool>("bot", value);
    }
// player name
    public string Name
    {
        get => Get<string>("name");
        set => Set<string>("name", value);
    }
// player network (i.e steam) id
    public string Networkid
    {
        get => Get<string>("networkid");
        set => Set<string>("networkid", value);
    }
// user ID on server (unique on server)
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// steam id
    public ulong Xuid
    {
        get => Get<ulong>("xuid");
        set => Set<ulong>("xuid", value);
    }
}
#nullable restore
