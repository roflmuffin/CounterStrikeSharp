#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("server_spawn")]
public class EventServerSpawn : GameEvent
{
    public EventServerSpawn(IntPtr pointer) : base(pointer){}
    public EventServerSpawn(bool force) : base("server_spawn", force){}
    // addon name
    public string Addonname
    {
        get => Get<string>("addonname");
        set => Set<string>("addonname", value);
    }
// hostame, IP or DNS name
    public string Address
    {
        get => Get<string>("address");
        set => Set<string>("address", value);
    }
// true if dedicated server
    public bool Dedicated
    {
        get => Get<bool>("dedicated");
        set => Set<bool>("dedicated", value);
    }
// game dir
    public string Game
    {
        get => Get<string>("game");
        set => Set<string>("game", value);
    }
// public host name
    public string Hostname
    {
        get => Get<string>("hostname");
        set => Set<string>("hostname", value);
    }
// map name
    public string Mapname
    {
        get => Get<string>("mapname");
        set => Set<string>("mapname", value);
    }
// max players
    public long Maxplayers
    {
        get => Get<long>("maxplayers");
        set => Set<long>("maxplayers", value);
    }
// WIN32, LINUX
    public string Os
    {
        get => Get<string>("os");
        set => Set<string>("os", value);
    }
// true if password protected
    public bool Password
    {
        get => Get<bool>("password");
        set => Set<bool>("password", value);
    }
// server port
    public int Port
    {
        get => Get<int>("port");
        set => Set<int>("port", value);
    }
}
#nullable restore
