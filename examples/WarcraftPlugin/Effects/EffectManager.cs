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
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace WarcraftPlugin.Effects
{
    public class EffectManager
    {
        private List<WarcraftEffect> _effects = new List<WarcraftEffect>();
        private float _tickRate = 0.25f;

        public void Initialize()
        {
            WarcraftPlugin.Instance.AddTimer(_tickRate, EffectTick, TimerFlags.REPEAT);
        }

        public void AddEffect(WarcraftEffect effect)
        {
            _effects.Add(effect);
            effect.OnStart();
        }

        private void EffectTick()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];

                effect.RemainingDuration -= _tickRate;
                effect.OnTick();

                if (effect.RemainingDuration <= 0)
                {
                    effect.RemainingDuration = 0;
                    effect.OnFinish();
                    _effects.RemoveAt(i);
                }
            }
        }

        public void ClearEffects(CCSPlayerController player)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];
                if (effect.Target?.Handle == player.Handle)
                {
                    effect.OnFinish();
                    _effects.RemoveAt(i);
                }
            }
        }
    }
}