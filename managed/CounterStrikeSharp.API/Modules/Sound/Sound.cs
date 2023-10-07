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
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Players;
using CounterStrikeSharp.API.Modules.Sound.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Sound
{
    public static class Sound
    {
        public static bool PrecacheSound(string sound) => NativeAPI.PrecacheSound(sound, true);

        public static bool IsSoundPrecached(string sound) => NativeAPI.IsSoundPrecached(sound);

        public static float GetSoundDuration(string sound) => NativeAPI.GetSoundDuration(sound);

        public static void EmitSound(int client,
            string sound, 
            int entity = SoundSource.SOUND_FROM_PLAYER,
            SoundChannel channel = SoundChannel.CHAN_AUTO,
            SoundLevel level = SoundLevel.SNDLVL_NORM,
            float attenuation = SoundAttenuation.ATTN_NORM,
            SoundFlags flags = SoundFlags.SND_NOFLAGS,
            float volume = 1.0f,
            int pitch = SoundPitch.PITCH_NORM,
            Vector origin = null,
            Vector direction = null)
        {
            NativeAPI.EmitSound(client, entity, (int) channel, sound, volume, attenuation, (int) flags,
                pitch, origin?.Handle ?? IntPtr.Zero, direction?.Handle ?? IntPtr.Zero);
        }

        public static void EmitSoundToAll(string sound,
            int entity = SoundSource.SOUND_FROM_PLAYER,
            SoundChannel channel = SoundChannel.CHAN_AUTO,
            SoundLevel level = SoundLevel.SNDLVL_NORM,
            float attenuation = SoundAttenuation.ATTN_NORM,
            SoundFlags flags = SoundFlags.SND_NOFLAGS,
            float volume = 1.0f,
            int pitch = SoundPitch.PITCH_NORM,
            Vector origin = null,
            Vector direction = null)
        {
            for (int i = 1; i < 65; i++)
            {
                var client = Player.FromIndex(i);
                if (client?.IsValid == true)
                {
                    Sound.EmitSound(i, sound, entity, channel, level, attenuation, flags, volume, pitch, origin,
                        direction);
                }
            }
        }
    }
}