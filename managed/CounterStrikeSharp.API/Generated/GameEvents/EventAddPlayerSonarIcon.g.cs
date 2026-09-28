#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("add_player_sonar_icon")]
public class EventAddPlayerSonarIcon : GameEvent
{
    public EventAddPlayerSonarIcon(IntPtr pointer) : base(pointer){}
    public EventAddPlayerSonarIcon(bool force) : base("add_player_sonar_icon", force){}
    
    public float PosX
    {
        get => Get<float>("pos_x");
        set => Set<float>("pos_x", value);
    }

    public float PosY
    {
        get => Get<float>("pos_y");
        set => Set<float>("pos_y", value);
    }

    public float PosZ
    {
        get => Get<float>("pos_z");
        set => Set<float>("pos_z", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
