#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ragdoll_dissolved")]
public class EventRagdollDissolved : GameEvent
{
    public EventRagdollDissolved(IntPtr pointer) : base(pointer){}
    public EventRagdollDissolved(bool force) : base("ragdoll_dissolved", force){}
    
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
}
#nullable restore
