#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("client_loadout_changed")]
public class EventClientLoadoutChanged : GameEvent
{
    public EventClientLoadoutChanged(IntPtr pointer) : base(pointer){}
    public EventClientLoadoutChanged(bool force) : base("client_loadout_changed", force){}
    
}
#nullable restore
