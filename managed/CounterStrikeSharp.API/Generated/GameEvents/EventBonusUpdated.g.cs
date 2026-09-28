#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bonus_updated")]
public class EventBonusUpdated : GameEvent
{
    public EventBonusUpdated(IntPtr pointer) : base(pointer){}
    public EventBonusUpdated(bool force) : base("bonus_updated", force){}
    
    public int Numadvanced
    {
        get => Get<int>("numadvanced");
        set => Set<int>("numadvanced", value);
    }

    public int Numbronze
    {
        get => Get<int>("numbronze");
        set => Set<int>("numbronze", value);
    }

    public int Numgold
    {
        get => Get<int>("numgold");
        set => Set<int>("numgold", value);
    }

    public int Numsilver
    {
        get => Get<int>("numsilver");
        set => Set<int>("numsilver", value);
    }
}
#nullable restore
