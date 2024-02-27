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

using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;
namespace CounterStrikeSharp.API
{
	public class Players
	{
		/// <summary>
		/// Returns a list of <see cref="CCSPlayerController"/> which is guaranteed to have all valid players.
		/// </summary>
		public static List<CCSPlayerController> GetPlayers()
		{
			List<CCSPlayerController> players = new();

			for (int i = 0; i < Server.MaxPlayers; i++)
			{
				var controller = Utilities.GetEntityFromIndex<CCSPlayerController>(i + 1);
				if (!Guard.IsPlayerReady(controller)) 
					continue;

				players.Add(controller);
			}

			return players;
		}

		/// <summary>
		/// Grabs a <see cref="CCSPlayerController"/> via entity index. 
		/// </summary>
		/// <param name="index">Entity index.</param>
		/// <exception cref="InvalidOperationException">Player is not valid</exception>
		public static CCSPlayerController GetPlayerFromIndex(int index)
		{
			var player = Utilities.GetEntityFromIndex<CCSPlayerController>(index);
			if (!Guard.IsPlayerReady(player))
			{
				throw new InvalidOperationException("Player is not valid");
			}
			return player;
		}

		/// <summary>
		/// Grabs a <see cref="CCSPlayerController"/> via player slot. 
		/// </summary>
		/// <param name="slot">Player slot.</param>
		/// <exception cref="InvalidOperationException">Player is not valid</exception>
		public static CCSPlayerController GetPlayerFromSlot(int slot)
		{
			var player = Utilities.GetEntityFromIndex<CCSPlayerController>(slot + 1);
			if (!Guard.IsPlayerReady(player))
			{
				throw new InvalidOperationException("Player is not valid");
			}
			return player;
		}

		/// <summary>
		/// Grabs a <see cref="CCSPlayerController"/> via User ID. 
		/// </summary>
		/// <param name="userid">User ID.</param>
		/// <exception cref="InvalidOperationException">Player is not valid</exception>
		public static CCSPlayerController GetPlayerFromUserid(int userid)
		{
			var player = Utilities.GetEntityFromIndex<CCSPlayerController>((userid & 0xFF) + 1);
			if (!Guard.IsPlayerReady(player))
			{
				throw new InvalidOperationException("Player is not valid");
			}
			return player;
		}

		/// <summary>
		/// Grabs a <see cref="CCSPlayerController"/> via Steam ID. 
		/// </summary>
		/// <param name="steamId">SteamID.</param>
		/// <exception cref="InvalidOperationException">Player is not valid</exception>
		public static CCSPlayerController GetPlayerFromSteamId(ulong steamId)
		{
			var player = GetPlayers().FirstOrDefault(player => player.AuthorizedSteamID == (SteamID)steamId);
			if (!Guard.IsPlayerReady(player))
			{
				throw new InvalidOperationException("Player is not valid");
			}
			return player!;
		}
	}
}