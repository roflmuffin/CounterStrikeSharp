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
    public HitGroup_t GetHitGroup()
    {
        IntPtr v4 = Marshal.ReadIntPtr(Handle, 0x70);

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
