#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_disconnect")]
public class EventPlayerDisconnect : GameEvent
{
    public EventPlayerDisconnect(IntPtr pointer) : base(pointer){}
    public EventPlayerDisconnect(bool force) : base("player_disconnect", force){}
    // player name
    public virtual string Name
    {
        get => Get<string>("name");
        set => Set<string>("name", value);
    }
// player network (i.e steam) id
    public virtual string Networkid
    {
        get => Get<string>("networkid");
        set => Set<string>("networkid", value);
    }

    public virtual int Playerid
    {
        get => Get<int>("PlayerID");
        set => Set<int>("PlayerID", value);
    }
// see networkdisconnect enum protobuf
    public virtual int Reason
    {
        get => Get<int>("reason");
        set => Set<int>("reason", value);
    }
// user ID on server
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// steam id
    public virtual ulong Xuid
    {
        get => Get<ulong>("xuid");
        set => Set<ulong>("xuid", value);
    }
}
#nullable restore
