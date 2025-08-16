using System.Runtime.InteropServices;

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
    [Obsolete("Use HitGroupId instead.")]
    public HitGroup_t GetHitGroup()
    {
        return HitGroupId;
    }
}
