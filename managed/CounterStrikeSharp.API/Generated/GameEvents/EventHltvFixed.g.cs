#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_fixed")]
public class EventHltvFixed : GameEvent
{
    public EventHltvFixed(IntPtr pointer) : base(pointer){}
    public EventHltvFixed(bool force) : base("hltv_fixed", force){}
    
    public float Fov
    {
        get => Get<float>("fov");
        set => Set<float>("fov", value);
    }

    public int Offset
    {
        get => Get<int>("offset");
        set => Set<int>("offset", value);
    }

    public int Phi
    {
        get => Get<int>("phi");
        set => Set<int>("phi", value);
    }
// camera position in world
    public long Posx
    {
        get => Get<long>("posx");
        set => Set<long>("posx", value);
    }

    public long Posy
    {
        get => Get<long>("posy");
        set => Set<long>("posy", value);
    }

    public long Posz
    {
        get => Get<long>("posz");
        set => Set<long>("posz", value);
    }
// follow this player
    public CCSPlayerController? Target
    {
        get => GetPlayer("target");
        set => SetPlayer("target", value);
    }
// camera angles
    public int Theta
    {
        get => Get<int>("theta");
        set => Set<int>("theta", value);
    }
}
#nullable restore
