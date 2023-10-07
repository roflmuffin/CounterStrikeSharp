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

namespace CounterStrikeSharp.API.Modules.Sound.Constants
{
    public enum SoundChannel
    {
        CHAN_REPLACE	= -1,
        CHAN_AUTO		= 0,
        CHAN_WEAPON		= 1,
        CHAN_VOICE		= 2,
        CHAN_ITEM		= 3,
        CHAN_BODY		= 4,
        CHAN_STREAM		= 5,		// allocate stream channel from the static or dynamic area
        CHAN_STATIC		= 6,		// allocate channel from the static area 
        CHAN_VOICE_BASE	= 7,		// allocate channel for network voice data
        CHAN_USER_BASE = 135
    }
}