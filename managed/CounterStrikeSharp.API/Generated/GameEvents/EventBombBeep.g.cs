#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bomb_beep")]
public class EventBombBeep : GameEvent
{
    public EventBombBeep(IntPtr pointer) : base(pointer){}
    public EventBombBeep(bool force) : base("bomb_beep", force){}
    // c4 entity
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
}
#nullable restore
