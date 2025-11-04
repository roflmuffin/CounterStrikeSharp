#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("demo_start")]
public class EventDemoStart : GameEvent
{
    public EventDemoStart(IntPtr pointer) : base(pointer){}
    public EventDemoStart(bool force) : base("demo_start", force){}
    // CSVCMsgList_GameEvents that are combat log events
    public virtual int DotaCombatlogList
    {
        get => Get<int>("dota_combatlog_list");
        set => Set<int>("dota_combatlog_list", value);
    }
// CSVCMsgList_GameEvents
    public virtual int DotaHeroChaseList
    {
        get => Get<int>("dota_hero_chase_list");
        set => Set<int>("dota_hero_chase_list", value);
    }
// CSVCMsgList_GameEvents
    public virtual int DotaPickHeroList
    {
        get => Get<int>("dota_pick_hero_list");
        set => Set<int>("dota_pick_hero_list", value);
    }

    public virtual int Local
    {
        get => Get<int>("local");
        set => Set<int>("local", value);
    }
}
#nullable restore
