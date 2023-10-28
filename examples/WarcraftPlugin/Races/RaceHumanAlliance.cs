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
using System.Drawing;
using WarcraftPlugin.Effects;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace WarcraftPlugin.Races
{
    public class RaceHumanAlliance : WarcraftRace
    {
        public override string InternalName => "human_alliance";
        public override string DisplayName => "Human Alliance";

        private Timer _invisiblityTimer = null;
        private bool _isInvisible = false;

        public override void Register()
        {
            AddAbility(new SimpleWarcraftAbility("invisibility", "Invisibility",
                i =>
                    $"Every 1 second, you will become invisible for {ChatColors.Green}0.15/0.30/0.45/0.60/0.75{ChatColors.Default} seconds"));
            AddAbility(new SimpleWarcraftAbility("devotion", "Devotion",
                i => $"Spawn with an extra {ChatColors.Yellow}10/20/30/40/50{ChatColors.Default} HP."));
            AddAbility(new SimpleWarcraftAbility("freeze", "Freeze",
                i =>
                    $"When you hit an enemy, there is a {ChatColors.Blue}5/10/15/20/25%{ChatColors.Default} chance you will freeze them in place for 1 second."));
            AddAbility(new SimpleCooldownAbility("dash", "Dash",
                i =>
                    $"When you press your ultimate key, you will be launched at {ChatColors.Red}extreme speed{ChatColors.Default} in the direction you are facing.",
                3.0f));

            HookEvent<EventPlayerDeath>("player_death", PlayerDeath);
            HookEvent<EventPlayerSpawn>("player_spawn", PlayerSpawn);
            HookEvent<EventPlayerHurt>("player_hurt_other", PlayerHurtOther);

            HookAbility(3, Ultimate);
        }

        private void Ultimate()
        {
            if (WarcraftPlayer.GetAbilityLevel(3) < 1) return;

            if (IsAbilityReady(3))
            {
                Dash(1000f);
                StartCooldown(3);
            }
        }

        private void Dash(float distance)
        {
            var directionVec = new Vector();

            NativeAPI.AngleVectors(Player.PlayerPawn.Value.EyeAngles.Handle, directionVec.Handle, IntPtr.Zero,
                IntPtr.Zero);

            // Always shoot us up a little bit if were on the ground and not aiming up.
            if (Player.GroundEntity.IsValid != null && directionVec.Z < 0.275)
            {
                directionVec.Z = 0.275f;
            }

            directionVec *= distance;

            // Player.PlayerPawn.Value.BaseVelocity = directionVec;
        }

        private void PlayerHurtOther(GameEvent obj)
        {
            var @event = (EventPlayerHurt)obj;
            if (!@event.Userid.IsValid || !@event.Userid.PawnIsAlive) return;

            double rolledValue = new Random().NextDouble();
            float chanceToStun = WarcraftPlayer.GetAbilityLevel(2) * 0.05f;

            if (rolledValue < chanceToStun)
            {
                var victim = @event.Userid;
                DispatchEffect(new FreezeEffect(Player, victim, 1.0f));
            }
        }

        private void PlayerSpawn(GameEvent obj)
        {
            Player.PlayerPawn.Value.Health = 100 + (10 * WarcraftPlayer.GetAbilityLevel(1));

            if (_invisiblityTimer != null)
            {
                _invisiblityTimer.Kill();
            }

            _invisiblityTimer = WarcraftPlugin.Instance.AddTimer(WarcraftPlayer.GetAbilityLevel(0) * 0.15f,
                ToggleInvisiblity, TimerFlags.REPEAT);


            ResetColor();
            _isInvisible = false;
        }

        public override void PlayerChangingToAnotherRace()
        {
            base.PlayerChangingToAnotherRace();
            _invisiblityTimer?.Kill();
            MakeVisible();
        }

        private void ToggleInvisiblity()
        {
            if (_isInvisible)
                MakeVisible();
            else
                MakeInvisible();

            _isInvisible = !_isInvisible;
        }

        private void MakeInvisible()
        {
            if (!Player.PawnIsAlive)
            {
                ResetColor();
                return;
            }
        }

        private void MakeVisible()
        {
            if (!Player.PawnIsAlive)
            {
                ResetColor();
                return;
            }

            ResetColor();
        }

        private void PlayerDeath(GameEvent obj)
        {
            ResetColor();
        }

        private void ResetColor()
        {
            // Player.RenderMode = RenderMode.RENDER_NORMAL;
            // Player.Color = Color.FromArgb(255, 255, 255, 255);
        }
    }

    public class FreezeEffect : WarcraftEffect
    {
        public FreezeEffect(CCSPlayerController owner, CCSPlayerController target, float duration) : base(owner, target,
            duration)
        {
        }

        public override void OnStart()
        {
            Target.GetWarcraftPlayer()?.SetStatusMessage($"{ChatColors.Blue}[FROZEN]{ChatColors.Default}", Duration);
            // Target.MoveType = MoveType.MOVETYPE_NONE;
            // Target.RenderMode = RenderMode.RENDER_TRANSCOLOR;
            // Target.Color = Color.FromArgb(255, 50, 50, 255);
            Console.WriteLine("Added freeze");
        }

        public override void OnTick()
        {
            base.OnTick();
            Console.WriteLine("Freeze tick");
        }

        public override void OnFinish()
        {
            // Target.MoveType = MoveType.MOVETYPE_ISOMETRIC;
            // Target.RenderMode = RenderMode.RENDER_NORMAL;
            // Target.Color = Color.FromArgb(255, 255, 255, 255);
            Console.WriteLine("Freeze finished");
        }
    }
}