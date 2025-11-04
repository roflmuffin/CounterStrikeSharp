#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_death")]
public class EventPlayerDeath : GameEvent
{
    public EventPlayerDeath(IntPtr pointer) : base(pointer){}
    public EventPlayerDeath(bool force) : base("player_death", force){}
    // assister helped with a flash
    public virtual bool Assistedflash
    {
        get => Get<bool>("assistedflash");
        set => Set<bool>("assistedflash", value);
    }
// player who assisted in the kill
    public virtual CCSPlayerController? Assister
    {
        get => GetPlayer("assister");
        set => SetPlayer("assister", value);
    }
// user ID who killed
    public virtual CCSPlayerController? Attacker
    {
        get => GetPlayer("attacker");
        set => SetPlayer("attacker", value);
    }
// attacker was blind from flashbang
    public virtual bool Attackerblind
    {
        get => Get<bool>("attackerblind");
        set => Set<bool>("attackerblind", value);
    }
// attacker was in midair
    public virtual bool Attackerinair
    {
        get => Get<bool>("attackerinair");
        set => Set<bool>("attackerinair", value);
    }
// distance to victim in meters
    public virtual float Distance
    {
        get => Get<float>("distance");
        set => Set<float>("distance", value);
    }
// damage done to armor
    public virtual int DmgArmor
    {
        get => Get<int>("dmg_armor");
        set => Set<int>("dmg_armor", value);
    }
// damage done to health
    public virtual int DmgHealth
    {
        get => Get<int>("dmg_health");
        set => Set<int>("dmg_health", value);
    }
// did killer dominate victim with this kill
    public virtual int Dominated
    {
        get => Get<int>("dominated");
        set => Set<int>("dominated", value);
    }
// singals a headshot
    public virtual bool Headshot
    {
        get => Get<bool>("headshot");
        set => Set<bool>("headshot", value);
    }
// hitgroup that was damaged
    public virtual int Hitgroup
    {
        get => Get<int>("hitgroup");
        set => Set<int>("hitgroup", value);
    }
// if replay data is unavailable, this will be present and set to false
    public virtual bool Noreplay
    {
        get => Get<bool>("noreplay");
        set => Set<bool>("noreplay", value);
    }
// kill happened without a scope, used for death notice icon
    public virtual bool Noscope
    {
        get => Get<bool>("noscope");
        set => Set<bool>("noscope", value);
    }
// number of objects shot penetrated before killing target
    public virtual int Penetrated
    {
        get => Get<int>("penetrated");
        set => Set<int>("penetrated", value);
    }
// did killer get revenge on victim with this kill
    public virtual int Revenge
    {
        get => Get<int>("revenge");
        set => Set<int>("revenge", value);
    }
// hitscan weapon went through smoke grenade
    public virtual bool Thrusmoke
    {
        get => Get<bool>("thrusmoke");
        set => Set<bool>("thrusmoke", value);
    }
// user ID who died
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
// weapon name killer used
    public virtual string Weapon
    {
        get => Get<string>("weapon");
        set => Set<string>("weapon", value);
    }
// faux item id of weapon killer used
    public virtual string WeaponFauxitemid
    {
        get => Get<string>("weapon_fauxitemid");
        set => Set<string>("weapon_fauxitemid", value);
    }
// inventory item id of weapon killer used
    public virtual string WeaponItemid
    {
        get => Get<string>("weapon_itemid");
        set => Set<string>("weapon_itemid", value);
    }

    public virtual string WeaponOriginalownerXuid
    {
        get => Get<string>("weapon_originalowner_xuid");
        set => Set<string>("weapon_originalowner_xuid", value);
    }
// is the kill resulting in squad wipe
    public virtual int Wipe
    {
        get => Get<int>("wipe");
        set => Set<int>("wipe", value);
    }
}
#nullable restore
