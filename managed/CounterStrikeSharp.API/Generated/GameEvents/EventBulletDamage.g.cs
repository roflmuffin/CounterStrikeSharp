#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bullet_damage")]
public class EventBulletDamage : GameEvent
{
    public EventBulletDamage(IntPtr pointer) : base(pointer){}
    public EventBulletDamage(bool force) : base("bullet_damage", force){}
    // aim punch x
    public float AimPunchX
    {
        get => Get<float>("aim_punch_x");
        set => Set<float>("aim_punch_x", value);
    }
// aim punch y
    public float AimPunchY
    {
        get => Get<float>("aim_punch_y");
        set => Set<float>("aim_punch_y", value);
    }
// aim punch z
    public float AimPunchZ
    {
        get => Get<float>("aim_punch_z");
        set => Set<float>("aim_punch_z", value);
    }
// player index who attacked
    public CCSPlayerController? Attacker
    {
        get => GetPlayer("attacker");
        set => SetPlayer("attacker", value);
    }
// attack tick
    public int AttackTickCount
    {
        get => Get<int>("attack_tick_count");
        set => Set<int>("attack_tick_count", value);
    }
// attack frac
    public float AttackTickFrac
    {
        get => Get<float>("attack_tick_frac");
        set => Set<float>("attack_tick_frac", value);
    }
// direction vector of the bullet
    public float DamageDirX
    {
        get => Get<float>("damage_dir_x");
        set => Set<float>("damage_dir_x", value);
    }
// direction vector of the bullet
    public float DamageDirY
    {
        get => Get<float>("damage_dir_y");
        set => Set<float>("damage_dir_y", value);
    }
// direction vector of the bullet
    public float DamageDirZ
    {
        get => Get<float>("damage_dir_z");
        set => Set<float>("damage_dir_z", value);
    }
// how far the bullet travelled before it hit the player
    public float Distance
    {
        get => Get<float>("distance");
        set => Set<float>("distance", value);
    }
// air inaccuracy
    public float InaccuracyAir
    {
        get => Get<float>("inaccuracy_air");
        set => Set<float>("inaccuracy_air", value);
    }
// move inaccuracy
    public float InaccuracyMove
    {
        get => Get<float>("inaccuracy_move");
        set => Set<float>("inaccuracy_move", value);
    }
// total inaccuracy
    public float InaccuracyTotal
    {
        get => Get<float>("inaccuracy_total");
        set => Set<float>("inaccuracy_total", value);
    }
// was the shooter jumping?
    public bool InAir
    {
        get => Get<bool>("in_air");
        set => Set<bool>("in_air", value);
    }
// was the shooter noscoped?
    public bool NoScope
    {
        get => Get<bool>("no_scope");
        set => Set<bool>("no_scope", value);
    }
// how many surfaces were penetrated
    public int NumPenetrations
    {
        get => Get<int>("num_penetrations");
        set => Set<int>("num_penetrations", value);
    }
// recoil index. Yes this is really a float.
    public float RecoilIndex
    {
        get => Get<float>("recoil_index");
        set => Set<float>("recoil_index", value);
    }
// render tick
    public int RenderTickCount
    {
        get => Get<int>("render_tick_count");
        set => Set<int>("render_tick_count", value);
    }
// render frac
    public float RenderTickFrac
    {
        get => Get<float>("render_tick_frac");
        set => Set<float>("render_tick_frac", value);
    }
// shoot angle x
    public float ShootAngX
    {
        get => Get<float>("shoot_ang_x");
        set => Set<float>("shoot_ang_x", value);
    }
// shoot angle y
    public float ShootAngY
    {
        get => Get<float>("shoot_ang_y");
        set => Set<float>("shoot_ang_y", value);
    }
// shoot angle z
    public float ShootAngZ
    {
        get => Get<float>("shoot_ang_z");
        set => Set<float>("shoot_ang_z", value);
    }
// lag compensation type
    public int Type
    {
        get => Get<int>("type");
        set => Set<int>("type", value);
    }
// player index who was hurt
    public CCSPlayerController? Victim
    {
        get => GetPlayer("victim");
        set => SetPlayer("victim", value);
    }
}
#nullable restore
