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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API
{
	public class Players
	{
		/// <summary>
		/// Checks to see if a <see cref="CCSPlayerController"/> is ready to be used and manipulated.
		/// </summary>
		private static bool IsPlayerReady(CCSPlayerController? player, bool ignoreAuth = false)
		{
			if (player is null) return false;
			if (!player.IsValid) return false;
			if (player.Slot < 0) return false;
			if (player.UserId == -1) return false;
			if (player.IsHLTV) return false;
			if (player.Connected != PlayerConnectedState.PlayerConnected) return false;
			if (!ignoreAuth && player.AuthorizedSteamID is null) return false;

			return true;
		}

		/// <summary>
		/// Returns a list of <see cref="CCSPlayerController"/> which is guaranteed to have all valid players.
		/// </summary>
		public static List<CCSPlayerController> GetPlayers()
		{
			List<CCSPlayerController> players = new();

			for (int i = 0; i < Server.MaxPlayers; i++)
			{
				// This calls IsPlayerReady().
				var controller = GetPlayerFromSlot(i);

				if (controller is null || !controller.IsValid || controller.UserId == -1)
					continue;

				players.Add(controller);
			}

			return players;
		}

		/// <summary>
		/// Grabs a <see cref="CCSPlayerController"/> via entity index. 
		/// </summary>
		/// <param name="index">Entity index.</param>
		public static CCSPlayerController? GetPlayerFromIndex(int index)
		{
			var player = Utilities.GetEntityFromIndex<CCSPlayerController>(index);
			if (!IsPlayerReady(player)) return null;
			return player;
		}


		public static CCSPlayerController? GetPlayerFromSlot(int slot)
		{
			var player = Utilities.GetEntityFromIndex<CCSPlayerController>(slot + 1);
			if (!IsPlayerReady(player)) return null;
			return player;
		}

		public static CCSPlayerController? GetPlayerFromUserid(int userid)
		{
			var player = Utilities.GetEntityFromIndex<CCSPlayerController>((userid & 0xFF) + 1);
			if (!IsPlayerReady(player)) return null;
			return player;
		}

		public static CCSPlayerController? GetPlayerFromSteamId(ulong steamId)
		{
			var player = GetPlayers().FirstOrDefault(player => player.AuthorizedSteamID == (SteamID)steamId);
			if (!IsPlayerReady(player)) return null;
			return player;
		}
	}
}