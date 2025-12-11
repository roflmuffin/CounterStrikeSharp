#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("sfuievent")]
public class EventSfuievent : GameEvent
{
    public EventSfuievent(IntPtr pointer) : base(pointer){}
    public EventSfuievent(bool force) : base("sfuievent", force){}
    
    public string Action
    {
        get => Get<string>("action");
        set => Set<string>("action", value);
    }

    public string Data
    {
        get => Get<string>("data");
        set => Set<string>("data", value);
    }

    public int Slot
    {
        get => Get<int>("slot");
        set => Set<int>("slot", value);
    }
}
#nullable restore
