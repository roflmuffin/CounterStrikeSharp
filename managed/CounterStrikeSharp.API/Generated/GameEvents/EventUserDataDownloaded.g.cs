#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("user_data_downloaded")]
public class EventUserDataDownloaded : GameEvent
{
    public EventUserDataDownloaded(IntPtr pointer) : base(pointer){}
    public EventUserDataDownloaded(bool force) : base("user_data_downloaded", force){}
    
}
#nullable restore
