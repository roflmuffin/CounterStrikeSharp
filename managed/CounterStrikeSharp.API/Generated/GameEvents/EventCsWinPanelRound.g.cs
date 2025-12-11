#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_win_panel_round")]
public class EventCsWinPanelRound : GameEvent
{
    public EventCsWinPanelRound(IntPtr pointer) : base(pointer){}
    public EventCsWinPanelRound(bool force) : base("cs_win_panel_round", force){}
    // define in cs_gamerules.h
    public int FinalEvent
    {
        get => Get<int>("final_event");
        set => Set<int>("final_event", value);
    }

    public long FunfactData1
    {
        get => Get<long>("funfact_data1");
        set => Set<long>("funfact_data1", value);
    }

    public long FunfactData2
    {
        get => Get<long>("funfact_data2");
        set => Set<long>("funfact_data2", value);
    }

    public long FunfactData3
    {
        get => Get<long>("funfact_data3");
        set => Set<long>("funfact_data3", value);
    }

    public CCSPlayerController? FunfactPlayer
    {
        get => GetPlayer("funfact_player");
        set => SetPlayer("funfact_player", value);
    }

    public string FunfactToken
    {
        get => Get<string>("funfact_token");
        set => Set<string>("funfact_token", value);
    }

    public bool ShowTimerAttack
    {
        get => Get<bool>("show_timer_attack");
        set => Set<bool>("show_timer_attack", value);
    }

    public bool ShowTimerDefend
    {
        get => Get<bool>("show_timer_defend");
        set => Set<bool>("show_timer_defend", value);
    }

    public int TimerTime
    {
        get => Get<int>("timer_time");
        set => Set<int>("timer_time", value);
    }
}
#nullable restore
