#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("endmatch_mapvote_selecting_map")]
public class EventEndmatchMapvoteSelectingMap : GameEvent
{
    public EventEndmatchMapvoteSelectingMap(IntPtr pointer) : base(pointer){}
    public EventEndmatchMapvoteSelectingMap(bool force) : base("endmatch_mapvote_selecting_map", force){}
    // Number of "ties"
    public int Count
    {
        get => Get<int>("count");
        set => Set<int>("count", value);
    }

    public int Slot1
    {
        get => Get<int>("slot1");
        set => Set<int>("slot1", value);
    }

    public int Slot10
    {
        get => Get<int>("slot10");
        set => Set<int>("slot10", value);
    }

    public int Slot2
    {
        get => Get<int>("slot2");
        set => Set<int>("slot2", value);
    }

    public int Slot3
    {
        get => Get<int>("slot3");
        set => Set<int>("slot3", value);
    }

    public int Slot4
    {
        get => Get<int>("slot4");
        set => Set<int>("slot4", value);
    }

    public int Slot5
    {
        get => Get<int>("slot5");
        set => Set<int>("slot5", value);
    }

    public int Slot6
    {
        get => Get<int>("slot6");
        set => Set<int>("slot6", value);
    }

    public int Slot7
    {
        get => Get<int>("slot7");
        set => Set<int>("slot7", value);
    }

    public int Slot8
    {
        get => Get<int>("slot8");
        set => Set<int>("slot8", value);
    }

    public int Slot9
    {
        get => Get<int>("slot9");
        set => Set<int>("slot9", value);
    }
}
#nullable restore
