#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ugc_file_download_start")]
public class EventUgcFileDownloadStart : GameEvent
{
    public EventUgcFileDownloadStart(IntPtr pointer) : base(pointer){}
    public EventUgcFileDownloadStart(bool force) : base("ugc_file_download_start", force){}
    // id of this specific content (may be image or map)
    public ulong Hcontent
    {
        get => Get<ulong>("hcontent");
        set => Set<ulong>("hcontent", value);
    }
// id of the associated content package
    public ulong PublishedFileId
    {
        get => Get<ulong>("published_file_id");
        set => Set<ulong>("published_file_id", value);
    }
}
#nullable restore
