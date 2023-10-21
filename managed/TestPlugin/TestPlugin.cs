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
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace TestPlugin
{
    public class SamplePlugin : BasePlugin
    {
        public override string ModuleName => "Sample Plugin";
        public override string ModuleVersion => "v1.0.0";

        public override void Load(bool hotReload)
        {
            Console.WriteLine(
                $"Test Plugin has been loaded, and the hot reload flag was {hotReload}, path is {ModulePath}");

            // ValveInterface provides pointers to loaded modules via Interface Name exposed from the engine (e.g. Source2Server001)
            var server = ValveInterface.Server;
            Log($"Server pointer found @ {server.Pointer:X}");
            
            // 	inline void(FASTCALL *ClientPrint)(CBasePlayerController *player, int msg_dest, const char *msg_name, const char *param1, const char *param2, const char *param3, const char *param4);
            var sigVirtualFunc = VirtualFunction.Create<IntPtr, int, string, IntPtr, IntPtr, IntPtr, IntPtr>(
                server.Pointer, GameData.GetSignature("ClientPrint"));

            var printAllFunc =
                VirtualFunction.Create<int, string, IntPtr, IntPtr, IntPtr, IntPtr>(server.Pointer,
                    GameData.GetSignature("UTIL_ClientPrintAll"));

            var switchTeamFunc = VirtualFunction.Create<IntPtr, int>(server.Pointer,
                GameData.GetSignature("CCSPlayerController_SwitchTeam"));
            
            // Register Game Event Handlers
            RegisterEventHandler<EventPlayerConnect>(GenericEventHandler);
            RegisterEventHandler<EventPlayerJump>(@event =>
            {
                sigVirtualFunc(@event.Userid.Handle, 2, "Test", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            });
            RegisterEventHandler<EventPlayerSpawn>(@event =>
            {
                if (!@event.Userid.IsValid) return;
                if (!@event.Userid.m_hPlayerPawn.IsValid) return;
                
                Log($"Player spawned with entity index: {@event.Userid.EntityIndex} & User ID: {@event.Userid.UserId}");
            });
            RegisterEventHandler<EventPlayerBlind>(GenericEventHandler);
            RegisterEventHandler<EventBulletImpact>(@event =>
            {
                var player = @event.Userid;
                var pawn = player.m_hPlayerPawn.Value;
                
                char randomColourChar = (char)new Random().Next(0, 16);
                printAllFunc(3, $"Random String with Random Colour: {randomColourChar}{new Random().Next()}", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                
                pawn.m_iHealth += 5;

                Log(
                    $"Found steamID {new SteamID(player.m_steamID)} for player {player.m_iszPlayerName}:{pawn.m_iHealth}|{pawn.m_bInBuyZone}");
                Log($"{@event.Userid}, {@event.X},{@event.Y},{@event.Z}");
            });
            RegisterEventHandler<EventRoundStart>(@event =>
            {
                // Grab all cs_player_controller entities and set their cash value to $1337.
                var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
                Log($"cs_player_controller count: {playerEntities.Count<CCSPlayerController>()}");

                foreach (var player in playerEntities)
                {
                    //var player = new CCSPlayerController(entInst.Handle);
                    player.m_pInGameMoneyServices.Value.m_iAccount = 1337;
                }

                // Grab everything starting with cs_, but we'll only mainpulate cs_gamerules.
                var csEntities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_");
                Log($"Amount of cs_* entities: {csEntities.Count<CBaseEntity>()}");

                foreach (var entity in csEntities)
                {
                    if (entity.DesignerName != "cs_gamerules") continue;
                    var gamerulesEnt = new CCSGameRules(entity.Handle);
                    gamerulesEnt.m_bCTTimeOutActive = true;
                }
            });

            // Hook global listeners defined by CounterStrikeSharp
            RegisterListener<Listeners.OnMapStart>(mapName => { Log($"Map {mapName} has started!"); });
            RegisterListener<Listeners.OnClientConnect>((index, name, ip) =>
            {
                Log($"Client {name} from {ip} has connected!");
            });
            RegisterListener<Listeners.OnClientAuthorized>((index, id) =>
            {
                Log($"Client {index} with address {id}");
            });

            RegisterListener<Listeners.OnEntitySpawned>(entity =>
            {

                var designerName = entity.DesignerName;
                if (designerName != "smokegrenade_projectile") return;

                var projectile = new CSmokeGrenadeProjectile(entity.Handle);

                Server.NextFrame(() =>
                {
                    projectile.m_vSmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                    projectile.m_vSmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                    projectile.m_vSmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                    Log($"Smoke grenade spawned with color {projectile.m_vSmokeColor}");
                });
            });

            // You can use `ModuleDirectory` to get the directory of the plugin (for storing config files, saving database files etc.)
            File.WriteAllText(Path.Join(ModuleDirectory, "example.txt"),
                $"Test file created by TestPlugin at {DateTime.Now}");

            

            // Execute a server command as if typed into the server console.
            Server.ExecuteCommand("find \"cssharp\"");

            // Adds a new server console command
            AddCommand("cssharp_info", "A test command",
                (clientIndex, info) =>
                {
                    Log($"CounterStrikeSharp - a test command was called by {clientIndex} with {info.ArgString}");
                });

            // Example vfunc call that usually gets the game event manager pointer
            // by calling the func at offset 91 then subtracting 8 from the result pointer.
            // This value is asserted against the native code that points to the same function.
            var virtualFunc = VirtualFunction.CreateFunc<IntPtr>(server.Pointer, 91);
            var result = virtualFunc.Invoke() - 8;
            Log($"Result of virtual func call is {result:X}");

            
        }

        [GameEventHandler]
        public void OnPlayerConnect(EventPlayerConnect @event)
        {
            Log($"Player {@event.Name} has connected!");
        }

        [ConsoleCommand("cssharp_attribute", "This is a custom attribute event")]
        public void OnCommand(int client, CommandInfo command)
        {
            Log("cssharp_attribute called!");
        }

        private void GenericEventHandler<T>(T @event) where T : GameEvent
        {
            Log($"Event found {@event.Handle:X}, event name: {@event.EventName}");
        }

        private void Log(string message)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}