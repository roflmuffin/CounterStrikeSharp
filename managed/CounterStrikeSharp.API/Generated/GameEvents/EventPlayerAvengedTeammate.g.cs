#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_avenged_teammate")]
public class EventPlayerAvengedTeammate : GameEvent
{
    public EventPlayerAvengedTeammate(IntPtr pointer) : base(pointer){}
    public EventPlayerAvengedTeammate(bool force) : base("player_avenged_teammate", force){}
    
    public CCSPlayerController? AvengedPlayerId
    {
        get => GetPlayer("avenged_player_id");
        set => SetPlayer("avenged_player_id", value);
    }

    public CCSPlayerController? AvengerId
    {
        get => GetPlayer("avenger_id");
        set => SetPlayer("avenger_id", value);
    }
}
#nullable restore
