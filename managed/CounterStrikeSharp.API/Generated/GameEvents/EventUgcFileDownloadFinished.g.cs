#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("ugc_file_download_finished")]
public class EventUgcFileDownloadFinished : GameEvent
{
    public EventUgcFileDownloadFinished(IntPtr pointer) : base(pointer){}
    public EventUgcFileDownloadFinished(bool force) : base("ugc_file_download_finished", force){}
    // id of this specific content (may be image or map)
    public ulong Hcontent
    {
        get => Get<ulong>("hcontent");
        set => Set<ulong>("hcontent", value);
    }
}
#nullable restore
