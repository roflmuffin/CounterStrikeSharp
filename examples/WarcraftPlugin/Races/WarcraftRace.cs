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
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using WarcraftPlugin.Effects;
using CounterStrikeSharp.API.Modules.Events;

namespace WarcraftPlugin.Races
{
    public abstract class WarcraftAbility
    {
        public abstract string InternalName { get; }
        public abstract string DisplayName { get; }

        public abstract string GetDescription(int abilityLevel);

        public abstract bool HasCooldown { get; }
        public abstract float Cooldown { get; }
    }

    public class SimpleWarcraftAbility : WarcraftAbility
    {
        private Func<int, string> _descriptionGetter;

        public SimpleWarcraftAbility(string internalName, string displayName, Func<int, string> descriptionGetter)
        {
            InternalName = internalName;
            DisplayName = displayName;
            _descriptionGetter = descriptionGetter;
        }

        public override string InternalName { get; }
        public override string DisplayName { get; }

        public override string GetDescription(int abilityLevel)
        {
            return _descriptionGetter.Invoke(abilityLevel);
        }

        public override bool HasCooldown => false;
        public override float Cooldown => 0.0f;
    }

    public class SimpleCooldownAbility : SimpleWarcraftAbility
    {
        public override float Cooldown { get; }
        public override bool HasCooldown => true;

        public SimpleCooldownAbility(string internalName, string displayName, Func<int, string> descriptionGetter,
            float cooldown) : base(internalName, displayName, descriptionGetter)
        {
            Cooldown = cooldown;
        }
    }

    public abstract class WarcraftRace
    {
        public abstract string InternalName { get; }
        public abstract string DisplayName { get; }

        public WarcraftPlayer WarcraftPlayer { get; set; }
        public CCSPlayerController Player { get; set; }

        private List<WarcraftAbility> _abilities = new List<WarcraftAbility>();
        private Dictionary<string, Action<GameEvent>> _eventHandlers = new();
        private Dictionary<int, Action> _abilityHandlers = new Dictionary<int, Action>();

        public abstract void Register();

        public WarcraftAbility GetAbility(int index)
        {
            return _abilities[index];
        }

        protected void AddAbility(WarcraftAbility ability)
        {
            _abilities.Add(ability);
        }

        protected void HookEvent<T>(string eventName, Action<GameEvent> handler) where T : GameEvent
        {
            _eventHandlers[eventName] = handler;
        }

        protected void HookAbility(int abilityIndex, Action handler)
        {
            _abilityHandlers[abilityIndex] = handler;
        }

        public void InvokeEvent(string eventName, GameEvent @event)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName].Invoke(@event);
            }
        }

        public virtual void PlayerChangingToRace()
        {
        }

        public virtual void PlayerChangingToAnotherRace()
        {
        }

        public bool IsAbilityReady(int abilityIndex)
        {
            return WarcraftPlugin.Instance.CooldownManager.IsAvailable(WarcraftPlayer, abilityIndex);
        }

        public void StartCooldown(int abilityIndex)
        {
            var ability = _abilities[abilityIndex];
            WarcraftPlugin.Instance.CooldownManager.StartCooldown(WarcraftPlayer, abilityIndex, ability.Cooldown);
        }

        public void InvokeAbility(int abilityIndex)
        {
            if (_abilityHandlers.ContainsKey(abilityIndex))
            {
                _abilityHandlers[abilityIndex].Invoke();
            }
        }

        public void DispatchEffect(WarcraftEffect effect)
        {
            WarcraftPlugin.Instance.EffectManager.AddEffect(effect);
        }
    }
}