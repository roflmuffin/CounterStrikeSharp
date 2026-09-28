#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("set_instructor_group_enabled")]
public class EventSetInstructorGroupEnabled : GameEvent
{
    public EventSetInstructorGroupEnabled(IntPtr pointer) : base(pointer){}
    public EventSetInstructorGroupEnabled(bool force) : base("set_instructor_group_enabled", force){}
    
    public int Enabled
    {
        get => Get<int>("enabled");
        set => Set<int>("enabled", value);
    }

    public string Group
    {
        get => Get<string>("group");
        set => Set<string>("group", value);
    }
}
#nullable restore
