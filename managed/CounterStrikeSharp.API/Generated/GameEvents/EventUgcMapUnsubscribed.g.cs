#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ugc_map_unsubscribed")]
public class EventUgcMapUnsubscribed : GameEvent
{
    public EventUgcMapUnsubscribed(IntPtr pointer) : base(pointer){}
    public EventUgcMapUnsubscribed(bool force) : base("ugc_map_unsubscribed", force){}
    
    public ulong PublishedFileId
    {
        get => Get<ulong>("published_file_id");
        set => Set<ulong>("published_file_id", value);
    }
}
#nullable restore
