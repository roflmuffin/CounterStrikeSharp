#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("dynamic_shadow_light_changed")]
public class EventDynamicShadowLightChanged : GameEvent
{
    public EventDynamicShadowLightChanged(IntPtr pointer) : base(pointer){}
    public EventDynamicShadowLightChanged(bool force) : base("dynamic_shadow_light_changed", force){}
    
}
#nullable restore
