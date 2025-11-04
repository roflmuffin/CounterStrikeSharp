#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("nextlevel_changed")]
public class EventNextlevelChanged : GameEvent
{
    public EventNextlevelChanged(IntPtr pointer) : base(pointer){}
    public EventNextlevelChanged(bool force) : base("nextlevel_changed", force){}
    
    public virtual string Mapgroup
    {
        get => Get<string>("mapgroup");
        set => Set<string>("mapgroup", value);
    }

    public virtual string Nextlevel
    {
        get => Get<string>("nextlevel");
        set => Set<string>("nextlevel", value);
    }

    public virtual string Skirmishmode
    {
        get => Get<string>("skirmishmode");
        set => Set<string>("skirmishmode", value);
    }
}
#nullable restore
