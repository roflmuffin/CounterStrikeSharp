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
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

namespace TestPlugin
{
    public class SamplePlugin : BasePlugin
    {
        public override string ModuleName => "Sample Plugin";
        public override string ModuleVersion => "v1.0.0";

        public override void Load(bool hotReload)
        {
            Console.WriteLine($"Test Plugin has been loaded, and the hot reload flag was {hotReload}");

            OnClientConnect += args =>
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Client {args.Name} from {args.Address} has connected!");
                Console.ResetColor();
            };
            
            RegisterEventHandler<PlayerConnect>("player_connect", (@event) =>
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Event found {@event.Handle:X}, player name: {@event.Name}, bot: ${@event.Bot}");
                Console.ResetColor();
            });
        }
    }
}