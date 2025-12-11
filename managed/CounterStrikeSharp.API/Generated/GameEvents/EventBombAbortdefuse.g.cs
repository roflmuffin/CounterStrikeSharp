#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_abortdefuse")]
public class EventBombAbortdefuse : GameEvent
{
    public EventBombAbortdefuse(IntPtr pointer) : base(pointer){}
    public EventBombAbortdefuse(bool force) : base("bomb_abortdefuse", force){}
    // player who was defusing
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
