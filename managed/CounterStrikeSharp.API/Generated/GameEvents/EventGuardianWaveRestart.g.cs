#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("guardian_wave_restart")]
public class EventGuardianWaveRestart : GameEvent
{
    public EventGuardianWaveRestart(IntPtr pointer) : base(pointer){}
    public EventGuardianWaveRestart(bool force) : base("guardian_wave_restart", force){}
    
}
#nullable restore
