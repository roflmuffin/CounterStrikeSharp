#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_versioninfo")]
public class EventHltvVersioninfo : GameEvent
{
    public EventHltvVersioninfo(IntPtr pointer) : base(pointer){}
    public EventHltvVersioninfo(bool force) : base("hltv_versioninfo", force){}
    
    public long Version
    {
        get => Get<long>("version");
        set => Set<long>("version", value);
    }
}
#nullable restore
