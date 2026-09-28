#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("loot_crate_visible")]
public class EventLootCrateVisible : GameEvent
{
    public EventLootCrateVisible(IntPtr pointer) : base(pointer){}
    public EventLootCrateVisible(bool force) : base("loot_crate_visible", force){}
    // crate entindex
    public int Subject
    {
        get => Get<int>("subject");
        set => Set<int>("subject", value);
    }
// type of crate (metal, wood, or paradrop)
    public string Type
    {
        get => Get<string>("type");
        set => Set<string>("type", value);
    }
// player entindex
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
