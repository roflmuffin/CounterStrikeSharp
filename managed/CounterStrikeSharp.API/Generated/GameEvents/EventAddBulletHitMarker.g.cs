#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("add_bullet_hit_marker")]
public class EventAddBulletHitMarker : GameEvent
{
    public EventAddBulletHitMarker(IntPtr pointer) : base(pointer){}
    public EventAddBulletHitMarker(bool force) : base("add_bullet_hit_marker", force){}
    
    public int AngX
    {
        get => Get<int>("ang_x");
        set => Set<int>("ang_x", value);
    }

    public int AngY
    {
        get => Get<int>("ang_y");
        set => Set<int>("ang_y", value);
    }

    public int AngZ
    {
        get => Get<int>("ang_z");
        set => Set<int>("ang_z", value);
    }

    public int Bone
    {
        get => Get<int>("bone");
        set => Set<int>("bone", value);
    }

    public bool Hit
    {
        get => Get<bool>("hit");
        set => Set<bool>("hit", value);
    }

    public int PosX
    {
        get => Get<int>("pos_x");
        set => Set<int>("pos_x", value);
    }

    public int PosY
    {
        get => Get<int>("pos_y");
        set => Set<int>("pos_y", value);
    }

    public int PosZ
    {
        get => Get<int>("pos_z");
        set => Set<int>("pos_z", value);
    }

    public int StartX
    {
        get => Get<int>("start_x");
        set => Set<int>("start_x", value);
    }

    public int StartY
    {
        get => Get<int>("start_y");
        set => Set<int>("start_y", value);
    }

    public int StartZ
    {
        get => Get<int>("start_z");
        set => Set<int>("start_z", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
