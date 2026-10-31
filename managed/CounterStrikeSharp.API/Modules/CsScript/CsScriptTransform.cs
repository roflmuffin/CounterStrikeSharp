using System.Numerics;

namespace CounterStrikeSharp.API.Modules.CsScript;

/// <summary>Changes to an entity's physics state. Null properties leave that component unchanged.</summary>
public sealed class CsScriptTransform
{
    public Vector3? Position { get; init; }
    /// <summary>Pitch, yaw, and roll in degrees, stored in X, Y, and Z.</summary>
    public Vector3? Angles { get; init; }
    public Vector3? Velocity { get; init; }
    public Vector3? AngularVelocity { get; init; }
}
