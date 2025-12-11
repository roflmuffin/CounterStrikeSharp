#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("spec_mode_updated")]
public class EventSpecModeUpdated : GameEvent
{
    public EventSpecModeUpdated(IntPtr pointer) : base(pointer){}
    public EventSpecModeUpdated(bool force) : base("spec_mode_updated", force){}
    // spectating player
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
