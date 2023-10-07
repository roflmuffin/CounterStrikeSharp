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

using System;

namespace CounterStrikeSharp.API
{
    [Flags]
    public enum PlayerButtons
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
        Score = (1 << 16),   /**< Used by client.dll for when scoreboard is held down */
        Speed = (1 << 17),   /**< Player is holding the speed key */
        Walk = (1 << 18),   /**< Player holding walk key */
        Zoom = (1 << 19),   /**< Zoom key for HUD zoom */
        Weapon1 = (1 << 20),   /**< weapon defines these bits */
        Weapon2 = (1 << 21),   /**< weapon defines these bits */
        Bullrush = (1 << 22),
        Grenade1 = (1 << 23),   /**< grenade 1 */
        Grenade2 = (1 << 24),   /**< grenade 2 */
        Attack3 = (1 << 25)
    }
}