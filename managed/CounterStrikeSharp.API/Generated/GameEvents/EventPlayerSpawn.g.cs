#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_spawn")]
public class EventPlayerSpawn : GameEvent
{
    public EventPlayerSpawn(IntPtr pointer) : base(pointer){}
    public EventPlayerSpawn(bool force) : base("player_spawn", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
