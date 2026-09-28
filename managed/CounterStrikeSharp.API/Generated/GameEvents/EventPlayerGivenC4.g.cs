#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_given_c4")]
public class EventPlayerGivenC4 : GameEvent
{
    public EventPlayerGivenC4(IntPtr pointer) : base(pointer){}
    public EventPlayerGivenC4(bool force) : base("player_given_c4", force){}
    // user ID who received the c4
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
