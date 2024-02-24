﻿/*
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

namespace CounterStrikeSharp.API.Modules.Utils;

public class ChatColors
{
    public static char Default = '\x01';
    public static char White = '\x01';
    public static char DarkRed = '\x02';
    public static char Green = '\x04';
    public static char LightYellow = '\x09';
    public static char LightBlue = '\x0B';
    public static char Olive = '\x05';
    public static char Lime = '\x06';
    public static char Red = '\x07';
    public static char LightPurple = '\x03';
    public static char Purple = '\x0E';
    public static char Grey = '\x08';
    public static char Yellow = '\x09';
    public static char Gold = '\x10';
    public static char Silver = '\x0A';
    public static char Blue = '\x0B';
    public static char DarkBlue = '\x0C';
    public static char BlueGrey = '\x0A';
    public static char Magenta = '\x0E';
    public static char LightRed = '\x0F';
    public static char Orange = '\x10';

    [Obsolete("Use ChatColors.DarkRed instead.")]
    public static char Darkred = '\x02';

    
    /// <summary>
    /// Returns a chat color based on a team.
    /// <remarks>Blue for CT, Yellow for T, LightPurple for Spectator</remarks>
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static char ForTeam(CsTeam team)
    {
        switch (team)
        {
            case CsTeam.None:
                return White;
            case CsTeam.Spectator:
                return LightPurple;
            case CsTeam.CounterTerrorist:
                return LightBlue;
            case CsTeam.Terrorist:
                return Yellow;
            default:
                throw new ArgumentException($"Invalid team: ${team}");
        }
    }

    /// <summary>
    /// Returns a chat color for a player based on their team.
    /// <remarks>Blue for CT, Yellow for T, LightPurple for Spectator</remarks>
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static char ForPlayer(CCSPlayerController player)
    {
        return ForTeam(player.Team);
    }
}