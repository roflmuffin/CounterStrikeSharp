#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_jump")]
public class EventPlayerJump : GameEvent
{
    public EventPlayerJump(IntPtr pointer) : base(pointer){}
    public EventPlayerJump(bool force) : base("player_jump", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
