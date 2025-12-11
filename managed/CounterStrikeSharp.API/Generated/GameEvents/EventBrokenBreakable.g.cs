#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("broken_breakable")]
public class EventBrokenBreakable : GameEvent
{
    public EventBrokenBreakable(IntPtr pointer) : base(pointer){}
    public EventBrokenBreakable(bool force) : base("broken_breakable", force){}
    
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
// BREAK_GLASS, BREAK_WOOD, etc
    public int Material
    {
        get => Get<int>("material");
        set => Set<int>("material", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
