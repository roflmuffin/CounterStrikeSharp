#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("buymenu_close")]
public class EventBuymenuClose : GameEvent
{
    public EventBuymenuClose(IntPtr pointer) : base(pointer){}
    public EventBuymenuClose(bool force) : base("buymenu_close", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
