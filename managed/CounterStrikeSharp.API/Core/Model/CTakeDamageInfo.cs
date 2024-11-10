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
    public unsafe HitGroup_t HitGroup()
    {
        nint v4 = *(nint*)(this.Handle + 0x78);

        if (v4 == nint.Zero)
        {
            return HitGroup_t.HITGROUP_INVALID;
        }

        nint v1 = *(nint*)(v4 + 16);

        if (v1 == nint.Zero)
        {
            return HitGroup_t.HITGROUP_GENERIC;
        }

        return (HitGroup_t)(*(uint*)(v1 + 56));
    }
}
