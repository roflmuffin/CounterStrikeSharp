#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("material_default_complete")]
public class EventMaterialDefaultComplete : GameEvent
{
    public EventMaterialDefaultComplete(IntPtr pointer) : base(pointer){}
    public EventMaterialDefaultComplete(bool force) : base("material_default_complete", force){}
    
}
#nullable restore
