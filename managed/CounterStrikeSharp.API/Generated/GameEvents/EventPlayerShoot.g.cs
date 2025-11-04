#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_shoot")]
public class EventPlayerShoot : GameEvent
{
    public EventPlayerShoot(IntPtr pointer) : base(pointer){}
    public EventPlayerShoot(bool force) : base("player_shoot", force){}
    // weapon mode
    public virtual int Mode
    {
        get => Get<int>("mode");
        set => Set<int>("mode", value);
    }
// user ID on server
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// weapon ID
    public virtual int Weapon
    {
        get => Get<int>("weapon");
        set => Set<int>("weapon", value);
    }
}
#nullable restore
