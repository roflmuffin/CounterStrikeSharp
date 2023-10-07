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

namespace CounterStrikeSharp.API.Modules.Entities.Constants
{
    public enum ObserverMode
    {
		OBS_MODE_NONE = 0,  // not in spectator mode
        OBS_MODE_DEATHCAM,  // special mode for death cam animation
        OBS_MODE_FREEZECAM, // zooms to a target, and freeze-frames on them
        OBS_MODE_FIXED,     // view from a fixed camera position
        OBS_MODE_IN_EYE,    // follow a player in first person view
        OBS_MODE_CHASE,     // follow a player in third person view
        OBS_MODE_ROAMING,   // free roaming

        NUM_OBSERVER_MODES,
	}
}