#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("exit_bombzone")]
public class EventExitBombzone : GameEvent
{
    public EventExitBombzone(IntPtr pointer) : base(pointer){}
    public EventExitBombzone(bool force) : base("exit_bombzone", force){}
    
    public bool Hasbomb
    {
        get => Get<bool>("hasbomb");
        set => Set<bool>("hasbomb", value);
    }

    public bool Isplanted
    {
        get => Get<bool>("isplanted");
        set => Set<bool>("isplanted", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
