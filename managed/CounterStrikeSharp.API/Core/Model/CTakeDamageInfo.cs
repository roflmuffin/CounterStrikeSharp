using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core.Bindings;

namespace CounterStrikeSharp.API.Core;

public partial class CTakeDamageInfo
{
    /// <summary>
    /// Retrieves the hitgroup
    /// </summary>
    /// <returns>
    /// Returns a <see cref="HitGroup_t"/> enumeration representing the player's current hit group,
    /// or <see cref="HitGroup_t.HITGROUP_INVALID"/> if the hit group cannot be determined.
    /// </returns>
    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.entity/take-damage-hit-group</c> is disabled.</exception>
    public HitGroup_t GetHitGroup()
    {
        var offset = EngineFeatures.Current.GetOffset(ManagedEngineFeatures.EntityTakeDamageHitGroup, "CTakeDamageInfo_HitGroup");
        IntPtr v4 = Marshal.ReadIntPtr(Handle, offset);

        if (v4 == nint.Zero)
        {
            return HitGroup_t.HITGROUP_INVALID;
        }

        IntPtr v1 = Marshal.ReadIntPtr(v4, 16);

        if (v1 == nint.Zero)
        {
            return HitGroup_t.HITGROUP_GENERIC;
        }

        return (HitGroup_t)Marshal.ReadInt32(v1, 56);
    }
}
