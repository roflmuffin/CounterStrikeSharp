#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("show_deathpanel")]
public class EventShowDeathpanel : GameEvent
{
    public EventShowDeathpanel(IntPtr pointer) : base(pointer){}
    public EventShowDeathpanel(bool force) : base("show_deathpanel", force){}
    
    public int DamageGiven
    {
        get => Get<int>("damage_given");
        set => Set<int>("damage_given", value);
    }

    public int DamageTaken
    {
        get => Get<int>("damage_taken");
        set => Set<int>("damage_taken", value);
    }

    public int HitsGiven
    {
        get => Get<int>("hits_given");
        set => Set<int>("hits_given", value);
    }

    public int HitsTaken
    {
        get => Get<int>("hits_taken");
        set => Set<int>("hits_taken", value);
    }
// entindex of the killer entity
    public IntPtr Killer
    {
        get => Get<IntPtr>("killer");
        set => Set<IntPtr>("killer", value);
    }

    public CCSPlayerController? KillerController
    {
        get => GetPlayer("killer_controller");
        set => SetPlayer("killer_controller", value);
    }
// endindex of the one who was killed
    public CCSPlayerController? Victim
    {
        get => GetPlayer("victim");
        set => SetPlayer("victim", value);
    }
}
#nullable restore
