#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_falldamage")]
public class EventPlayerFalldamage : GameEvent
{
    public EventPlayerFalldamage(IntPtr pointer) : base(pointer){}
    public EventPlayerFalldamage(bool force) : base("player_falldamage", force){}
    
    public float Damage
    {
        get => Get<float>("damage");
        set => Set<float>("damage", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
