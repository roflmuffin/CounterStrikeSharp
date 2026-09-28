#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("clientside_reload_custom_econ")]
public class EventClientsideReloadCustomEcon : GameEvent
{
    public EventClientsideReloadCustomEcon(IntPtr pointer) : base(pointer){}
    public EventClientsideReloadCustomEcon(bool force) : base("clientside_reload_custom_econ", force){}
    
    public string Steamid
    {
        get => Get<string>("steamid");
        set => Set<string>("steamid", value);
    }
}
#nullable restore
