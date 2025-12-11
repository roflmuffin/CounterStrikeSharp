#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_dropped")]
public class EventBombDropped : GameEvent
{
    public EventBombDropped(IntPtr pointer) : base(pointer){}
    public EventBombDropped(bool force) : base("bomb_dropped", force){}
    
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
// player who dropped the bomb
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
