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
using System.Linq;

namespace WarcraftPlugin.Races
{
    public class RaceManager
    {
        private Dictionary<string, Type> _races = new Dictionary<string, Type>();
        private Dictionary<string, WarcraftRace> _raceObjects = new Dictionary<string, WarcraftRace>();

        public void Initialize()
        {
            RegisterRace<RaceUndeadScourge>();
            RegisterRace<RaceHumanAlliance>();
        }

        private void RegisterRace<T>() where T : WarcraftRace, new()
        {
            var race = new T();
            race.Register();
            _races[race.InternalName] = typeof(T);
            _raceObjects[race.InternalName] = race;
        }

        public WarcraftRace InstantiateRace(string name)
        {
            if (!_races.ContainsKey(name)) throw new Exception("Race not found: " + name);

            var race = (WarcraftRace)Activator.CreateInstance(_races[name]);
            race.Register();

            return race;
        }

        public WarcraftRace[] GetAllRaces()
        {
            return _raceObjects.Values.ToArray();
        }

        public WarcraftRace GetRace(string name)
        {
            return _raceObjects.ContainsKey(name) ? _raceObjects[name] : null;
        }
    }
}