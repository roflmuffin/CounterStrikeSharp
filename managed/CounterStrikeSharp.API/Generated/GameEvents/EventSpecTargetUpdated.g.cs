#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("spec_target_updated")]
public class EventSpecTargetUpdated : GameEvent
{
    public EventSpecTargetUpdated(IntPtr pointer) : base(pointer){}
    public EventSpecTargetUpdated(bool force) : base("spec_target_updated", force){}
    // ehandle of the target
    public IntPtr Target
    {
        get => Get<IntPtr>("target");
        set => Set<IntPtr>("target", value);
    }
// spectating player
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
