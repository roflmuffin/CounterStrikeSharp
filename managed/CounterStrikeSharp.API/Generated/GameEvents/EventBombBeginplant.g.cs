#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_beginplant")]
public class EventBombBeginplant : GameEvent
{
    public EventBombBeginplant(IntPtr pointer) : base(pointer){}
    public EventBombBeginplant(bool force) : base("bomb_beginplant", force){}
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
