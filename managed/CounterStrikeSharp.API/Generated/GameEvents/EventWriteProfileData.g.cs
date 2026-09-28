#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("write_profile_data")]
public class EventWriteProfileData : GameEvent
{
    public EventWriteProfileData(IntPtr pointer) : base(pointer){}
    public EventWriteProfileData(bool force) : base("write_profile_data", force){}
    
}
#nullable restore
