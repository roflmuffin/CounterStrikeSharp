#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_abortplant")]
public class EventBombAbortplant : GameEvent
{
    public EventBombAbortplant(IntPtr pointer) : base(pointer){}
    public EventBombAbortplant(bool force) : base("bomb_abortplant", force){}
    // bombsite index
    public int Site
    {
        get => Get<int>("site");
        set => Set<int>("site", value);
    }
// player who is planting the bomb
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
