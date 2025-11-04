#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_pickup")]
public class EventBombPickup : GameEvent
{
    public EventBombPickup(IntPtr pointer) : base(pointer){}
    public EventBombPickup(bool force) : base("bomb_pickup", force){}
    // player pawn who picked up the bomb
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
