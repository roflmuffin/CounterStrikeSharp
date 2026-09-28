#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_spawned")]
public class EventPlayerSpawned : GameEvent
{
    public EventPlayerSpawned(IntPtr pointer) : base(pointer){}
    public EventPlayerSpawned(bool force) : base("player_spawned", force){}
    // true if restart is pending
    public bool Inrestart
    {
        get => Get<bool>("inrestart");
        set => Set<bool>("inrestart", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
