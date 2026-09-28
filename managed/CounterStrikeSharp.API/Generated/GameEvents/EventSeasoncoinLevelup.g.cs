#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("seasoncoin_levelup")]
public class EventSeasoncoinLevelup : GameEvent
{
    public EventSeasoncoinLevelup(IntPtr pointer) : base(pointer){}
    public EventSeasoncoinLevelup(bool force) : base("seasoncoin_levelup", force){}
    
    public int Category
    {
        get => Get<int>("category");
        set => Set<int>("category", value);
    }

    public int Rank
    {
        get => Get<int>("rank");
        set => Set<int>("rank", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
