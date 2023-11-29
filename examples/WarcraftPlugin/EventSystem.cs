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
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Utils;

namespace WarcraftPlugin
{
    public class EventSystem
    {
        private WarcraftPlugin _plugin;

        public EventSystem(WarcraftPlugin plugin)
        {
            _plugin = plugin;
        }

        public void Initialize()
        {
            _plugin.RegisterEventHandler<EventPlayerDeath>(PlayerDeathHandler);
            _plugin.RegisterEventHandler<EventPlayerSpawn>(PlayerSpawnHandler);
            _plugin.RegisterEventHandler<EventPlayerHurt>(PlayerHurtHandler);
        }

        private HookResult PlayerHurtHandler(EventPlayerHurt @event, GameEventInfo _)
        {
            var victim = @event.Userid;
            var attacker = @event.Attacker;

            victim?.GetWarcraftPlayer()?.GetRace()?.InvokeEvent("player_hurt", @event);
            attacker?.GetWarcraftPlayer()?.GetRace()?.InvokeEvent("player_hurt_other", @event);
            
            return HookResult.Continue;
        }

        private HookResult PlayerSpawnHandler(EventPlayerSpawn @event, GameEventInfo _)
        {
            var player = @event.Userid;
            var race = player.GetWarcraftPlayer()?.GetRace();

            if (race != null)
            {
                var name = @event.EventName;
                Server.NextFrame(() =>
                {
                    WarcraftPlugin.Instance.EffectManager.ClearEffects(player);
                    race.InvokeEvent(name, @event);
                });
            }

            return HookResult.Continue;
        }

        private HookResult PlayerDeathHandler(EventPlayerDeath @event, GameEventInfo _)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            var headshot = @event.Headshot;

            if (attacker.IsValid && victim.IsValid && (attacker != victim) && !attacker.IsBot)
            {
                var weaponName = attacker.PlayerPawn.Value.WeaponServices.ActiveWeapon.Value.DesignerName;

                int xpToAdd = 0;
                int xpHeadshot = 0;
                int xpKnife = 0;

                xpToAdd = _plugin.XpPerKill;

                if (headshot)
                    xpHeadshot = Convert.ToInt32(_plugin.XpPerKill * _plugin.XpHeadshotModifier);

                if (weaponName == "weapon_knife")
                    xpKnife = Convert.ToInt32(_plugin.XpPerKill * _plugin.XpKnifeModifier);

                xpToAdd += xpHeadshot + xpKnife;

                _plugin.XpSystem.AddXp(attacker, xpToAdd);

                string hsBonus = "";
                if (xpHeadshot != 0)
                {
                    hsBonus = $"(+{xpHeadshot} HS bonus)";
                }

                string knifeBonus = "";
                if (xpKnife != 0)
                {
                    knifeBonus = $"(+{xpKnife} knife bonus)";
                }

                string xpString = $" {ChatColors.Gold}+{xpToAdd} XP {ChatColors.Default}for killing {ChatColors.Green}{victim.PlayerName} {ChatColors.Default}{hsBonus}{knifeBonus}";

                _plugin.GetWcPlayer(attacker).SetStatusMessage(xpString);
                attacker.PrintToChat(xpString);
            }

            victim?.GetWarcraftPlayer()?.GetRace()?.InvokeEvent("player_death", @event);
            attacker?.GetWarcraftPlayer()?.GetRace()?.InvokeEvent("player_kill", @event);

            WarcraftPlugin.Instance.EffectManager.ClearEffects(victim);
            
            return HookResult.Continue;
        }
    }
}