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
    public enum SoundLevel
    {
        SNDLVL_NONE			= 0,

        SNDLVL_20dB			= 20,			// rustling leaves
        SNDLVL_25dB			= 25,			// whispering
        SNDLVL_30dB			= 30,			// library
        SNDLVL_35dB			= 35,
        SNDLVL_40dB			= 40,
        SNDLVL_45dB			= 45,			// refrigerator

        SNDLVL_50dB			= 50,	// 3.9	// average home
        SNDLVL_55dB			= 55,	// 3.0

        SNDLVL_IDLE			= 60,	// 2.0	
        SNDLVL_60dB			= 60,	// 2.0	// normal conversation, clothes dryer

        SNDLVL_65dB			= 65,	// 1.5	// washing machine, dishwasher
        SNDLVL_STATIC		= 66,	// 1.25

        SNDLVL_70dB			= 70,	// 1.0	// car, vacuum cleaner, mixer, electric sewing machine

        SNDLVL_NORM			= 75,
        SNDLVL_75dB			= 75,	// 0.8	// busy traffic

        SNDLVL_80dB			= 80,	// 0.7	// mini-bike, alarm clock, noisy restaurant, office tabulator, outboard motor, passing snowmobile
        SNDLVL_TALKING		= 80,	// 0.7
        SNDLVL_85dB			= 85,	// 0.6	// average factory, electric shaver
        SNDLVL_90dB			= 90,	// 0.5	// screaming child, passing motorcycle, convertible ride on frw
        SNDLVL_95dB			= 95,
        SNDLVL_100dB		= 100,	// 0.4	// subway train, diesel truck, woodworking shop, pneumatic drill, boiler shop, jackhammer
        SNDLVL_105dB		= 105,			// helicopter, power mower
        SNDLVL_110dB		= 110,			// snowmobile drvrs seat, inboard motorboat, sandblasting
        SNDLVL_120dB		= 120,			// auto horn, propeller aircraft
        SNDLVL_130dB		= 130,			// air raid siren

        SNDLVL_GUNFIRE		= 140,	// 0.27	// THRESHOLD OF PAIN, gunshot, jet engine
        SNDLVL_140dB		= 140,	// 0.2

        SNDLVL_150dB		= 150,	// 0.2

        SNDLVL_180dB		= 180,			// rocket launching
    }
}