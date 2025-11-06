#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("item_remove")]
public class EventItemRemove : GameEvent
{
    public EventItemRemove(IntPtr pointer) : base(pointer){}
    public EventItemRemove(bool force) : base("item_remove", force){}
    
    public long Defindex
    {
        get => Get<long>("defindex");
        set => Set<long>("defindex", value);
    }
// either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
    public string Item
    {
        get => Get<string>("item");
        set => Set<string>("item", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
