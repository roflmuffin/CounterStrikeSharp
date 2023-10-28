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

using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace WarcraftPlugin.Cooldowns
{
    public class CooldownManager
    {
        private float _tickRate = 0.25f;

        public void Initialize()
        {
            WarcraftPlugin.Instance.AddTimer(_tickRate, CooldownTick, TimerFlags.REPEAT);
        }

        private void CooldownTick()
        {
            foreach (var player in WarcraftPlugin.Instance.Players)
            {
                if (player == null) continue;
                for (int i = 0; i < 4; i++)
                {
                    if (player.AbilityCooldowns[i] >= 0)
                    {
                        var oldCooldown = player.AbilityCooldowns[i];
                        player.AbilityCooldowns[i] -= 0.25f;

                        if (oldCooldown > 0 && player.AbilityCooldowns[i] <= 0.0)
                        {
                            player.AbilityCooldowns[i] = 0.0f;
                            PlayEffects(player, i);
                        }
                    }
                }
            }
        }

        public bool IsAvailable(WarcraftPlayer player, int abilityIndex)
        {
            return player.AbilityCooldowns[abilityIndex] <= 0.0f;
        }

        public void StartCooldown(WarcraftPlayer player, int abilityIndex, float abilityCooldown)
        {
            player.AbilityCooldowns[abilityIndex] = abilityCooldown;
        }

        private void PlayEffects(WarcraftPlayer player, int abilityIndex)
        {
            var ability = player.GetRace().GetAbility(abilityIndex);

            // Sound.EmitSound(player.Index, "items/battery_pickup.wav", origin: player.GetPlayer().AbsOrigin);

            player.SetStatusMessage($"{ChatColors.Red}{ability.DisplayName}{ChatColors.Default} ready.");
        }
    }
}