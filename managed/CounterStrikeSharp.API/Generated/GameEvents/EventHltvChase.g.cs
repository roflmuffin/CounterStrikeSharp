#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_chase")]
public class EventHltvChase : GameEvent
{
    public EventHltvChase(IntPtr pointer) : base(pointer){}
    public EventHltvChase(bool force) : base("hltv_chase", force){}
    // camera distance
    public int Distance
    {
        get => Get<int>("distance");
        set => Set<int>("distance", value);
    }
// camera inertia
    public int Inertia
    {
        get => Get<int>("inertia");
        set => Set<int>("inertia", value);
    }
// diretcor suggests to show ineye
    public int Ineye
    {
        get => Get<int>("ineye");
        set => Set<int>("ineye", value);
    }
// view angle vertical
    public int Phi
    {
        get => Get<int>("phi");
        set => Set<int>("phi", value);
    }
// primary traget index
    public CCSPlayerController? Target1
    {
        get => GetPlayer("target1");
        set => SetPlayer("target1", value);
    }
// secondary traget index or 0
    public CCSPlayerController? Target2
    {
        get => GetPlayer("target2");
        set => SetPlayer("target2", value);
    }
// view angle horizontal
    public int Theta
    {
        get => Get<int>("theta");
        set => Set<int>("theta", value);
    }
}
#nullable restore
