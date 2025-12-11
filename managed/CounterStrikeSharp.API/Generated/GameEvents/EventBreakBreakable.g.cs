#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("break_breakable")]
public class EventBreakBreakable : GameEvent
{
    public EventBreakBreakable(IntPtr pointer) : base(pointer){}
    public EventBreakBreakable(bool force) : base("break_breakable", force){}
    
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
