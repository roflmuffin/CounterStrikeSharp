#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("grenade_thrown")]
public class EventGrenadeThrown : GameEvent
{
    public EventGrenadeThrown(IntPtr pointer) : base(pointer){}
    public EventGrenadeThrown(bool force) : base("grenade_thrown", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// weapon name used
    public string Weapon
    {
        get => Get<string>("weapon");
        set => Set<string>("weapon", value);
    }
}
#nullable restore
