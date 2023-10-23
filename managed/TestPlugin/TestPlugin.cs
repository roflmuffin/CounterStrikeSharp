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
            
            Console.WriteLine($"Max Players: {Server.MaxPlayers}");

            // ValveInterface provides pointers to loaded modules via Interface Name exposed from the engine (e.g. Source2Server001)
            var server = ValveInterface.Server;
            Log($"Server pointer found @ {server.Pointer:X}");

            // 	inline void(FASTCALL *ClientPrint)(CBasePlayerController *player, int msg_dest, const char *msg_name, const char *param1, const char *param2, const char *param3, const char *param4);
            var sigVirtualFunc = VirtualFunction.CreateVoid<IntPtr, int, string, IntPtr, IntPtr, IntPtr, IntPtr>(
                GameData.GetSignature("ClientPrint"));

            var printAllFunc =
                VirtualFunction.CreateVoid<int, string, IntPtr, IntPtr, IntPtr, IntPtr>(
                    GameData.GetSignature("UTIL_ClientPrintAll"));

            var switchTeamFunc =
                VirtualFunction.CreateVoid<IntPtr, int>(GameData.GetSignature("CCSPlayerController_SwitchTeam"));
            // Register Game Event Handlers
            RegisterEventHandler<EventPlayerConnect>(GenericEventHandler);
            RegisterEventHandler<EventPlayerDeath>((@event, info) =>
            {
                // You can use `info.DontBroadcast` to set the dont broadcast flag on the event.
                if (new Random().NextSingle() > 0.5f)
                {
                    @event.Attacker.PrintToChat($"Skipping player_death broadcast at {Server.CurrentTime}");
                    info.DontBroadcast = true;
                }

                return HookResult.Continue;
            });
            RegisterEventHandler<EventPlayerJump>((@event, info) =>
            {
                sigVirtualFunc(@event.Userid.Handle, 2, "Test", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                return HookResult.Continue;
            });
            RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
            {
                if (!@event.Userid.IsValid) return 0;
                if (!@event.Userid.PlayerPawn.IsValid) return 0;

                Log($"Player spawned with entity index: {@event.Userid.EntityIndex} & User ID: {@event.Userid.UserId}");

                return HookResult.Continue;
            });
            RegisterEventHandler<EventPlayerBlind>(GenericEventHandler);
            RegisterEventHandler<EventBulletImpact>((@event, info) =>
            {
                var player = @event.Userid;
                var pawn = player.PlayerPawn.Value;
                var activeWeapon = @event.Userid.PlayerPawn.Value.WeaponServices?.ActiveWeapon.Value;
                var weapons = @event.Userid.PlayerPawn.Value.WeaponServices?.MyWeapons;

                Server.NextFrame(() =>
                {
                    player.PrintToCenter(string.Join("\n", weapons.Select(x => x.Value.DesignerName)));
                });

                activeWeapon.ReserveAmmo[0] = 250;
                activeWeapon.Clip1 = 250;

                VirtualFunctions.GiveNamedItem(pawn.ItemServices.Handle, "weapon_ak47", 0, 0, 0, 0);

                Log(
                    $"Pawn Position: {pawn.CBodyComponent?.SceneNode?.AbsOrigin} @{pawn.CBodyComponent?.SceneNode.Rotation}");

                char randomColourChar = (char)new Random().Next(0, 16);
                printAllFunc(3, $"Random String with Random Colour: {randomColourChar}{new Random().Next()}",
                    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                pawn.Health += 5;

                Log(
                    $"Found steamID {new SteamID(player.SteamID)} for player {player.PlayerName}:{pawn.Health}|{pawn.InBuyZone}");
                Log($"{@event.Userid}, {@event.X},{@event.Y},{@event.Z}");

                return HookResult.Continue;
            });
            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                // Grab all cs_player_controller entities and set their cash value to $1337.
                var playerEntities =
                    Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
                Log($"cs_player_controller count: {playerEntities.Count<CCSPlayerController>()}");

                foreach (var player in playerEntities)
                {
                    //var player = new CCSPlayerController(entInst.Handle);
                    if (player.InGameMoneyServices != null) player.InGameMoneyServices.Account = 1337;
                }

                // Grab everything starting with cs_, but we'll only mainpulate cs_gamerules.
                var csEntities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_");
                Log($"Amount of cs_* entities: {csEntities.Count<CBaseEntity>()}");

                foreach (var entity in csEntities)
                {
                    if (entity.DesignerName != "cs_gamerules") continue;
                    var gamerulesEnt = new CCSGameRules(entity.Handle);
                    gamerulesEnt.CTTimeOutActive = true;
                }

                return HookResult.Continue;
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
                    projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                    projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                    projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                    Log($"Smoke grenade spawned with color {projectile.SmokeColor}");
                });
            });

            // You can use `ModuleDirectory` to get the directory of the plugin (for storing config files, saving database files etc.)
            File.WriteAllText(Path.Join(ModuleDirectory, "example.txt"),
                $"Test file created by TestPlugin at {DateTime.Now}");


            // Execute a server command as if typed into the server console.
            Server.ExecuteCommand("find \"cssharp\"");

            // Adds a new server console command
            AddCommand("cssharp_info", "A test command",
                (player, info) =>
                {
                    if (player == null) return;
                    Log(
                        $"CounterStrikeSharp - a test command was called by {new SteamID(player.SteamID).SteamId2} with {info.ArgString}");
                });

            // Example vfunc call that usually gets the game event manager pointer
            // by calling the func at offset 91 then subtracting 8 from the result pointer.
            // This value is asserted against the native code that points to the same function.
            var virtualFunc = VirtualFunction.Create<IntPtr>(server.Pointer, 91);
            var result = virtualFunc() - 8;
            Log($"Result of virtual func call is {result:X}");
        }

        [GameEventHandler]
        public HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            Log($"Player {@event.Name} has connected!");

            return HookResult.Continue;
        }
        
        [ConsoleCommand("killme", "Kills the player")]
        public void OnKillme(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;
            
            player.PlayerPawn.Value.CommitSuicide(true, true);
        }
        
        [ConsoleCommand("cssharp_attribute", "This is a custom attribute event")]
        public void OnCommand(CCSPlayerController? player, CommandInfo command)
        {
            Log("cssharp_attribute called!");
        }

        private HookResult GenericEventHandler<T>(T @event, GameEventInfo info) where T : GameEvent
        {
            Log($"Event found {@event.Handle:X}, event name: {@event.EventName} dont broadcast: {info.DontBroadcast}");

            return HookResult.Continue;
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