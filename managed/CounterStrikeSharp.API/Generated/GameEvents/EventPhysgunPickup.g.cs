#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("physgun_pickup")]
public class EventPhysgunPickup : GameEvent
{
    public EventPhysgunPickup(IntPtr pointer) : base(pointer){}
    public EventPhysgunPickup(bool force) : base("physgun_pickup", force){}
    // entity picked up
    public IntPtr Target
    {
        get => Get<IntPtr>("target");
        set => Set<IntPtr>("target", value);
    }
}
#nullable restore
