#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("open_crate_instr")]
public class EventOpenCrateInstr : GameEvent
{
    public EventOpenCrateInstr(IntPtr pointer) : base(pointer){}
    public EventOpenCrateInstr(bool force) : base("open_crate_instr", force){}
    // crate entindex
    public virtual int Subject
    {
        get => Get<int>("subject");
        set => Set<int>("subject", value);
    }
// type of crate (metal, wood, or paradrop)
    public virtual string Type
    {
        get => Get<string>("type");
        set => Set<string>("type", value);
    }
// player entindex
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
