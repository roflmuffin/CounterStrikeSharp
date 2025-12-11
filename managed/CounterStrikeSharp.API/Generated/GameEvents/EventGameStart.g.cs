#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("game_start")]
public class EventGameStart : GameEvent
{
    public EventGameStart(IntPtr pointer) : base(pointer){}
    public EventGameStart(bool force) : base("game_start", force){}
    // frag limit
    public long Fraglimit
    {
        get => Get<long>("fraglimit");
        set => Set<long>("fraglimit", value);
    }
// round objective
    public string Objective
    {
        get => Get<string>("objective");
        set => Set<string>("objective", value);
    }
// max round
    public long Roundslimit
    {
        get => Get<long>("roundslimit");
        set => Set<long>("roundslimit", value);
    }
// time limit
    public long Timelimit
    {
        get => Get<long>("timelimit");
        set => Set<long>("timelimit", value);
    }
}
#nullable restore
