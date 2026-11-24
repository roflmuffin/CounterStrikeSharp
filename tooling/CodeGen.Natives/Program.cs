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

using CodeGen.Natives.Scripts;

namespace CodeGen.Natives
{
    class Program
    {
        // No arguments: run every generator. Otherwise only the named ones: natives, engine-features, game-events.
        static void Main(string[] args)
        {
            bool Run(string generator) => args.Length == 0 || args.Contains(generator);

            if (Run("natives")) Generators.GenerateNatives();
            if (Run("engine-features")) Generators.GenerateEngineFeatures();
            if (Run("game-events")) Generators.GenerateGameEvents().GetAwaiter().GetResult();
        }
    }
}