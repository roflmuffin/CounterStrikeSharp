#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("other_death")]
public class EventOtherDeath : GameEvent
{
    public EventOtherDeath(IntPtr pointer) : base(pointer){}
    public EventOtherDeath(bool force) : base("other_death", force){}
    // user ID who killed
    public int Attacker
    {
        get => Get<int>("attacker");
        set => Set<int>("attacker", value);
    }
// attacker was blind from flashbang
    public bool Attackerblind
    {
        get => Get<bool>("attackerblind");
        set => Set<bool>("attackerblind", value);
    }
// singals a headshot
    public bool Headshot
    {
        get => Get<bool>("headshot");
        set => Set<bool>("headshot", value);
    }
// kill happened without a scope, used for death notice icon
    public bool Noscope
    {
        get => Get<bool>("noscope");
        set => Set<bool>("noscope", value);
    }
// other entity ID who died
    public int Otherid
    {
        get => Get<int>("otherid");
        set => Set<int>("otherid", value);
    }
// other entity type
    public string Othertype
    {
        get => Get<string>("othertype");
        set => Set<string>("othertype", value);
    }
// number of objects shot penetrated before killing target
    public int Penetrated
    {
        get => Get<int>("penetrated");
        set => Set<int>("penetrated", value);
    }
// hitscan weapon went through smoke grenade
    public bool Thrusmoke
    {
        get => Get<bool>("thrusmoke");
        set => Set<bool>("thrusmoke", value);
    }
// weapon name killer used
    public string Weapon
    {
        get => Get<string>("weapon");
        set => Set<string>("weapon", value);
    }
// faux item id of weapon killer used
    public string WeaponFauxitemid
    {
        get => Get<string>("weapon_fauxitemid");
        set => Set<string>("weapon_fauxitemid", value);
    }
// inventory item id of weapon killer used
    public string WeaponItemid
    {
        get => Get<string>("weapon_itemid");
        set => Set<string>("weapon_itemid", value);
    }

    public string WeaponOriginalownerXuid
    {
        get => Get<string>("weapon_originalowner_xuid");
        set => Set<string>("weapon_originalowner_xuid", value);
    }
}
#nullable restore
