#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_hurt")]
public class EventPlayerHurt : GameEvent
{
    public EventPlayerHurt(IntPtr pointer) : base(pointer){}
    public EventPlayerHurt(bool force) : base("player_hurt", force){}
    // remaining armor points
    public int Armor
    {
        get => Get<int>("armor");
        set => Set<int>("armor", value);
    }
// player who attacked
    public CCSPlayerController? Attacker
    {
        get => GetPlayer("attacker");
        set => SetPlayer("attacker", value);
    }
// damage done to armor
    public int DmgArmor
    {
        get => Get<int>("dmg_armor");
        set => Set<int>("dmg_armor", value);
    }
// damage done to health
    public int DmgHealth
    {
        get => Get<int>("dmg_health");
        set => Set<int>("dmg_health", value);
    }
// remaining health points
    public int Health
    {
        get => Get<int>("health");
        set => Set<int>("health", value);
    }
// hitgroup that was damaged
    public int Hitgroup
    {
        get => Get<int>("hitgroup");
        set => Set<int>("hitgroup", value);
    }
// player who was hurt
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// weapon name attacker used, if not the world
    public string Weapon
    {
        get => Get<string>("weapon");
        set => Set<string>("weapon", value);
    }
}
#nullable restore
