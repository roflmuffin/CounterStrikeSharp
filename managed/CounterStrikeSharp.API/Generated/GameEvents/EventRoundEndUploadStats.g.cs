#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_end_upload_stats")]
public class EventRoundEndUploadStats : GameEvent
{
    public EventRoundEndUploadStats(IntPtr pointer) : base(pointer){}
    public EventRoundEndUploadStats(bool force) : base("round_end_upload_stats", force){}
    
}
#nullable restore
