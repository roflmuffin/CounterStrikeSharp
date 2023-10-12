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
using System.IO;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
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
            
            AddCommand("cssharp_info", "A test command", (clientIndex, info) =>
            {
                Console.WriteLine($"CounterStrikeSharp - a test command was called by {clientIndex} with {info.ArgString}");
            });

            OnMapStart += args =>
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Map {args.MapName} has started!");
                Console.ResetColor();
            };

            OnClientConnect += args =>
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Client {args.Name} from {args.Address} has connected!");
                Console.ResetColor();
            };

            RegisterEventHandler<EventPlayerConnect>(GenericEventHandler);
            RegisterEventHandler<EventPlayerSpawn>(GenericEventHandler);
            RegisterEventHandler<EventPlayerBlind>(GenericEventHandler);
        }

        [GameEventHandler]
        public void OnPlayerConnect(EventPlayerConnect @event)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"Player {@event.Name} has connected!");
            Console.ResetColor();
        }

        [ConsoleCommand("cssharp_attribute", "This is a custom attribute event")]
        public void OnCommand(int client, CommandInfo command)
        {
            Console.WriteLine("cssharp_attribute called!");
        }

        private void GenericEventHandler<T>(T @event) where T : GameEvent
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(
                $"Event found {@event.Handle:X}, event name: {@event.EventName}");
            Console.ResetColor();
        }
    }
}