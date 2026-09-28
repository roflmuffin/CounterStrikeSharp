#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("mb_input_lock_success")]
public class EventMbInputLockSuccess : GameEvent
{
    public EventMbInputLockSuccess(IntPtr pointer) : base(pointer){}
    public EventMbInputLockSuccess(bool force) : base("mb_input_lock_success", force){}
    
}
#nullable restore
