#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("mb_input_lock_cancel")]
public class EventMbInputLockCancel : GameEvent
{
    public EventMbInputLockCancel(IntPtr pointer) : base(pointer){}
    public EventMbInputLockCancel(bool force) : base("mb_input_lock_cancel", force){}
    
}
#nullable restore
