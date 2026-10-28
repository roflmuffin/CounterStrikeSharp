#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_exploded")]
public class EventBombExploded : GameEvent
{
    public EventBombExploded(IntPtr pointer) : base(pointer){}
    public EventBombExploded(bool force) : base("bomb_exploded", force){}
    
    public int C4
    {
        get => Get<int>("c4");
        set => Set<int>("c4", value);
    }
// bombsite index
    public int Site
    {
        get => Get<int>("site");
        set => Set<int>("site", value);
    }
// player who planted the bomb
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
