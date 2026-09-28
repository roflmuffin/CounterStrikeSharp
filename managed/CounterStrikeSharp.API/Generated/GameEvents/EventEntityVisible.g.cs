#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("entity_visible")]
public class EventEntityVisible : GameEvent
{
    public EventEntityVisible(IntPtr pointer) : base(pointer){}
    public EventEntityVisible(bool force) : base("entity_visible", force){}
    // Classname of the entity they see
    public string Classname
    {
        get => Get<string>("classname");
        set => Set<string>("classname", value);
    }
// name of the entity they see
    public string Entityname
    {
        get => Get<string>("entityname");
        set => Set<string>("entityname", value);
    }
// Entindex of the entity they see
    public int Subject
    {
        get => Get<int>("subject");
        set => Set<int>("subject", value);
    }
// The player who sees the entity
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
