#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_call_for_help")]
public class EventHostageCallForHelp : GameEvent
{
    public EventHostageCallForHelp(IntPtr pointer) : base(pointer){}
    public EventHostageCallForHelp(bool force) : base("hostage_call_for_help", force){}
    // hostage entity index
    public int Hostage
    {
        get => Get<int>("hostage");
        set => Set<int>("hostage", value);
    }
}
#nullable restore
