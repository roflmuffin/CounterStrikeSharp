#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("gg_killed_enemy")]
public class EventGgKilledEnemy : GameEvent
{
    public EventGgKilledEnemy(IntPtr pointer) : base(pointer){}
    public EventGgKilledEnemy(bool force) : base("gg_killed_enemy", force){}
    // user ID who killed
    public CCSPlayerController? Attackerid
    {
        get => GetPlayer("attackerid");
        set => SetPlayer("attackerid", value);
    }
// did killer kill with a bonus weapon?
    public bool Bonus
    {
        get => Get<bool>("bonus");
        set => Set<bool>("bonus", value);
    }
// did killer dominate victim with this kill
    public int Dominated
    {
        get => Get<int>("dominated");
        set => Set<int>("dominated", value);
    }
// did killer get revenge on victim with this kill
    public int Revenge
    {
        get => Get<int>("revenge");
        set => Set<int>("revenge", value);
    }
// user ID who died
    public CCSPlayerController? Victimid
    {
        get => GetPlayer("victimid");
        set => SetPlayer("victimid", value);
    }
}
#nullable restore
