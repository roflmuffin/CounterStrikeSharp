namespace CounterStrikeSharp.API
{
    public static class Guard
    {
        public static void IsValidEntity(CEntityInstance ent)
        {
            if (!ent.IsValid)
                throw new InvalidOperationException("Entity is not valid");
        }
		public static bool IsPlayerReady(CCSPlayerController? player, bool ignoreAuth = false)
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
	}
}
