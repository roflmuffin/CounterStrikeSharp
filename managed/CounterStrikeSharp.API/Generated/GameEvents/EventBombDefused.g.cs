#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_defused")]
public class EventBombDefused : GameEvent
{
    public EventBombDefused(IntPtr pointer) : base(pointer){}
    public EventBombDefused(bool force) : base("bomb_defused", force){}
    // bombsite index
    public int Site
    {
        get => Get<int>("site");
        set => Set<int>("site", value);
    }
// player who defused the bomb
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
