#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ugc_map_info_received")]
public class EventUgcMapInfoReceived : GameEvent
{
    public EventUgcMapInfoReceived(IntPtr pointer) : base(pointer){}
    public EventUgcMapInfoReceived(bool force) : base("ugc_map_info_received", force){}
    
    public ulong PublishedFileId
    {
        get => Get<ulong>("published_file_id");
        set => Set<ulong>("published_file_id", value);
    }
}
#nullable restore
