#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("item_equip")]
public class EventItemEquip : GameEvent
{
    public EventItemEquip(IntPtr pointer) : base(pointer){}
    public EventItemEquip(bool force) : base("item_equip", force){}
    
    public bool Canzoom
    {
        get => Get<bool>("canzoom");
        set => Set<bool>("canzoom", value);
    }

    public long Defindex
    {
        get => Get<long>("defindex");
        set => Set<long>("defindex", value);
    }

    public bool Hassilencer
    {
        get => Get<bool>("hassilencer");
        set => Set<bool>("hassilencer", value);
    }

    public bool Hastracers
    {
        get => Get<bool>("hastracers");
        set => Set<bool>("hastracers", value);
    }

    public bool Ispainted
    {
        get => Get<bool>("ispainted");
        set => Set<bool>("ispainted", value);
    }

    public bool Issilenced
    {
        get => Get<bool>("issilenced");
        set => Set<bool>("issilenced", value);
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

    public int Weptype
    {
        get => Get<int>("weptype");
        set => Set<int>("weptype", value);
    }
}
#nullable restore
