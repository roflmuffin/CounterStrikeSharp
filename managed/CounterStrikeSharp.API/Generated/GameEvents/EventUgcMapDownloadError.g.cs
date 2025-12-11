#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ugc_map_download_error")]
public class EventUgcMapDownloadError : GameEvent
{
    public EventUgcMapDownloadError(IntPtr pointer) : base(pointer){}
    public EventUgcMapDownloadError(bool force) : base("ugc_map_download_error", force){}
    
    public long ErrorCode
    {
        get => Get<long>("error_code");
        set => Set<long>("error_code", value);
    }

    public ulong PublishedFileId
    {
        get => Get<ulong>("published_file_id");
        set => Set<ulong>("published_file_id", value);
    }
}
#nullable restore
