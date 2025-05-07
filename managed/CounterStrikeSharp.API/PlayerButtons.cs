/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

namespace CounterStrikeSharp.API
{
    [Flags]
    public enum PlayerButtons : UInt64
    {
        Attack = (1 << 0),
        Jump = (1 << 1),
        Duck = (1 << 2),
        Forward = (1 << 3),
        Back = (1 << 4),
        Use = (1 << 5),
        Cancel = (1 << 6),
        Left = (1 << 7),
        Right = (1 << 8),
        Moveleft = (1 << 9),
        Moveright = (1 << 10),
        Attack2 = (1 << 11),
        Run = (1 << 12),
        Reload = (1 << 13),
        Alt1 = (1 << 14),
        Alt2 = (1 << 15),
        Speed = (1 << 16),   /** Player is holding the speed key */
        Walk = (1 << 17),   /** Player holding walk key */
        Zoom = (1 << 18),   /** Zoom key for HUD zoom */
        Weapon1 = (1 << 19),   /** weapon defines these bits */
        Weapon2 = (1 << 20),   /** weapon defines these bits */
        Bullrush = (1 << 21),
        Grenade1 = (1 << 22),   /** grenade 1 */
        Grenade2 = (1 << 23),   /** grenade 2 */
        Attack3 = (1 << 24),
        Scoreboard = ((ulong)1 << 33),
        Inspect = ((ulong)1 << 35),
    }
}
