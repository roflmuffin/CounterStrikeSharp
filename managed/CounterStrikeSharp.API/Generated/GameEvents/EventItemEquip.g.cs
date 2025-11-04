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
    
    public virtual bool Canzoom
    {
        get => Get<bool>("canzoom");
        set => Set<bool>("canzoom", value);
    }

    public virtual long Defindex
    {
        get => Get<long>("defindex");
        set => Set<long>("defindex", value);
    }

    public virtual bool Hassilencer
    {
        get => Get<bool>("hassilencer");
        set => Set<bool>("hassilencer", value);
    }

    public virtual bool Hastracers
    {
        get => Get<bool>("hastracers");
        set => Set<bool>("hastracers", value);
    }

    public virtual bool Ispainted
    {
        get => Get<bool>("ispainted");
        set => Set<bool>("ispainted", value);
    }

    public virtual bool Issilenced
    {
        get => Get<bool>("issilenced");
        set => Set<bool>("issilenced", value);
    }
// either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
    public virtual string Item
    {
        get => Get<string>("item");
        set => Set<string>("item", value);
    }

    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public virtual int Weptype
    {
        get => Get<int>("weptype");
        set => Set<int>("weptype", value);
    }
}
#nullable restore
