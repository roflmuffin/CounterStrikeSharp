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
	public enum RenderFx
    {
        RENDERFX_NONE = 0,
        RENDERFX_PULSE_SLOW,
        RENDERFX_PULSE_FAST,
        RENDERFX_PULSE_SLOW_WIDE,
        RENDERFX_PULSE_FAST_WIDE,
        RENDERFX_FADE_SLOW,
        RENDERFX_FADE_FAST,
        RENDERFX_SOLID_SLOW,
        RENDERFX_SOLID_FAST,
        RENDERFX_STROBE_SLOW,
        RENDERFX_STROBE_FAST,
        RENDERFX_STROBE_FASTER,
        RENDERFX_FLICKER_SLOW,
        RENDERFX_FLICKER_FAST,
        RENDERFX_NO_DISSIPATION,
        RENDERFX_DISTORT,           /**< Distort/scale/translate flicker */
        RENDERFX_HOLOGRAM,          /**< kRenderFxDistort + distance fade */
        RENDERFX_EXPLODE,           /**< Scale up really big! */
        RENDERFX_GLOWSHELL,         /**< Glowing Shell */
        RENDERFX_CLAMP_MIN_SCALE,   /**< Keep this sprite from getting very small (SPRITES only!) */
        RENDERFX_ENV_RAIN,          /**< for environmental rendermode, make rain */
        RENDERFX_ENV_SNOW,          /**<  "        "            "    , make snow */
        RENDERFX_SPOTLIGHT,         /**< TEST CODE for experimental spotlight */
        RENDERFX_RAGDOLL,           /**< HACKHACK: TEST CODE for signalling death of a ragdoll character */
        RENDERFX_PULSE_FAST_WIDER,
        RENDERFX_MAX
    };
}