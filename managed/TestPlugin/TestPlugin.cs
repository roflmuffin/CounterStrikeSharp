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
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;

namespace TestPlugin
{
    [MinimumApiVersion(1)]
    public class SamplePlugin : BasePlugin
    {
        public override string ModuleName => "Sample Plugin";
        public override string ModuleVersion => "v1.0.0";

        public override string ModuleAuthor => "Roflmuffin";

        public override string ModuleDescription => "A playground of features used for testing";

        public override void Load(bool hotReload)
        {
            Console.WriteLine(
                $"Test Plugin has been loaded, and the hot reload flag was {hotReload}, path is {ModulePath}");

            Console.WriteLine($"Max Players: {Server.MaxPlayers}");

            SetupConvars();
            SetupGameEvents();
            SetupListeners();
            SetupCommands();
            SetupMenus();

            // ValveInterface provides pointers to loaded modules via Interface Name exposed from the engine (e.g. Source2Server001)
            var server = ValveInterface.Server;
            Log($"Server pointer found @ {server.Pointer:X}");

            // You can use `ModuleDirectory` to get the directory of the plugin (for storing config files, saving database files etc.)
            File.WriteAllText(Path.Join(ModuleDirectory, "example.txt"),
                $"Test file created by TestPlugin at {DateTime.Now}");

            // Execute a server command as if typed into the server console.
            Server.ExecuteCommand("find \"cssharp\"");

            // Example vfunc call that usually gets the game event manager pointer
            // by calling the func at offset 91 then subtracting 8 from the result pointer.
            // This value is asserted against the native code that points to the same function.
            var virtualFunc = VirtualFunction.Create<IntPtr>(server.Pointer, 91);
            var result = virtualFunc() - 8;
            Log($"Result of virtual func call is {result:X}");
        }

        private void SetupConvars()
        {
            RegisterListener<Listeners.OnMapStart>(name =>
            {
                var cheatsCvar = ConVar.Find("sv_cheats");
                cheatsCvar.SetValue(true);

                var numericCvar = ConVar.Find("mp_warmuptime");
                Console.WriteLine($"mp_warmuptime = {numericCvar?.GetPrimitiveValue<float>()}");

                var stringCvar = ConVar.Find("sv_skyname");
                Console.WriteLine($"sv_skyname = {stringCvar?.StringValue}");

                var fogCvar = ConVar.Find("fog_color");
                Console.WriteLine($"fog_color = {fogCvar?.GetNativeValue<Vector>()}");
            });
        }

        private void SetupGameEvents()
        {
            // Register Game Event Handlers
            RegisterEventHandler<EventPlayerConnect>(GenericEventHandler, HookMode.Pre);
            RegisterEventHandler<EventPlayerChat>(((@event, info) =>
            {
                var entity = new CCSPlayerController(NativeAPI.GetEntityFromIndex(@event.Userid));
                if (!entity.IsValid)
                {
                    Log("invalid entity");
                    return HookResult.Continue;
                }

                entity.PrintToChat($"You said {@event.Text}");
                return HookResult.Continue;
            }));
            RegisterEventHandler<EventPlayerDeath>((@event, info) =>
            {
                // You can use `info.DontBroadcast` to set the dont broadcast flag on the event.
                if (new Random().NextSingle() > 0.5f)
                {
                    @event.Attacker.PrintToChat($"Skipping player_death broadcast at {Server.CurrentTime}");
                    info.DontBroadcast = true;
                }

                return HookResult.Continue;
            }, HookMode.Pre);
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

                // Set player to random colour
                player.PlayerPawn.Value.Render = Color.FromArgb(Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255), Random.Shared.Next(0, 255));
                
                Server.NextFrame(() =>
                {
                    player.PrintToCenter(string.Join("\n", weapons.Select(x => x.Value.DesignerName)));
                });

                activeWeapon.ReserveAmmo[0] = 250;
                activeWeapon.Clip1 = 250;

                Log(
                    $"Pawn Position: {pawn.CBodyComponent?.SceneNode?.AbsOrigin} @{pawn.CBodyComponent?.SceneNode.Rotation}");

                char randomColourChar = (char)new Random().Next(0, 16);
                Server.PrintToChatAll($"Random String with Random Colour: {randomColourChar}{new Random().Next()}");

                pawn.Health += 5;

                Log(
                    $"Found steamID {new SteamID(player.SteamID)} for player {player.PlayerName}:{pawn.Health}|{pawn.InBuyZone}");
                Log($"{@event.Userid}, {@event.X},{@event.Y},{@event.Z}");

                return HookResult.Continue;
            });
            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                // Grab all cs_player_controller entities and set their cash value to $1337.
                var playerEntities = Utilities.GetPlayers();
                Log($"cs_player_controller count: {playerEntities.Count()}");

                foreach (var player in playerEntities)
                {
                    //var player = new CCSPlayerController(entInst.Handle);
                    if (player.InGameMoneyServices != null) player.InGameMoneyServices.Account = 1337;
                }

                // Grab everything starting with cs_, but we'll only mainpulate cs_gamerules.
                // Note: this iterates through all entities, so is an expensive operation.
                var csEntities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_");
                Log($"Amount of cs_* entities: {csEntities.Count()}");

                foreach (var entity in csEntities)
                {
                    if (entity.DesignerName != "cs_gamerules") continue;
                    var gamerulesEnt = new CCSGameRules(entity.Handle);
                    gamerulesEnt.CTTimeOutActive = true;
                }

                return HookResult.Continue;
            });
        }

        private void SetupListeners()
        {
            // Hook global listeners defined by CounterStrikeSharp
            RegisterListener<Listeners.OnMapStart>(mapName => { Log($"Map {mapName} has started!"); });
            RegisterListener<Listeners.OnMapEnd>(() => { Log($"Map has ended."); });
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

                switch (designerName)
                {
                    case "smokegrenade_projectile":
                        var projectile = new CSmokeGrenadeProjectile(entity.Handle);

                        Server.NextFrame(() =>
                        {
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            Log($"Smoke grenade spawned with color {projectile.SmokeColor}");
                        });
                        return;
                    case "flashbang_projectile":
                        var flashbang = new CBaseCSGrenadeProjectile(entity.Handle);

                        Server.NextFrame(() => { flashbang.Remove(); });
                        return;
                }
            });
        }

        private void SetupMenus()
        {
            // Chat Menu Example
            var largeMenu = new ChatMenu("Test Menu");
            for (int i = 1; i < 26; i++)
            {
                var i1 = i;
                largeMenu.AddMenuOption(new Random().NextSingle().ToString(),
                    (player, option) => player.PrintToChat($"You just selected {option.Text}"), i1 % 5 == 0);
            }

            var giveItemMenu = new ChatMenu("Small Menu");
            var handleGive = (CCSPlayerController player, ChatMenuOption option) =>
            {
                player.GiveNamedItem(option.Text);
            };

            giveItemMenu.AddMenuOption("weapon_ak47", handleGive);
            giveItemMenu.AddMenuOption("weapon_p250", handleGive);

            AddCommand("css_menu", "Opens example menu", (player, info) => { ChatMenus.OpenMenu(player, largeMenu); });
            AddCommand("css_gunmenu", "Gun Menu", (player, info) => { ChatMenus.OpenMenu(player, giveItemMenu); });

            for (int i = 1; i <= 9; i++)
            {
                AddCommand("css_" + i, "Command Key Handler", (player, info) =>
                {
                    if (player == null) return;
                    var key = Convert.ToInt32(info.GetArg(0).Split("_")[1]);
                    ChatMenus.OnKeyPress(player, key);
                });
            }
        }

        private void SetupCommands()
        {
            // Adds a new server console command
            AddCommand("cssharp_info", "A test command",
                (player, info) =>
                {
                    if (player == null) return;
                    Log(
                        $"CounterStrikeSharp - a test command was called by {new SteamID(player.SteamID).SteamId2} with {info.ArgString}");
                });

            AddCommand("css_changeteam", "change team", (player, info) =>
            {
                if (player?.IsValid != true) return;

                if ((CsTeam)player.TeamNum == CsTeam.Terrorist)
                {
                    player.ChangeTeam(CsTeam.CounterTerrorist);
                }
                else
                {
                    player.ChangeTeam(CsTeam.Terrorist);
                }
            });

            // Listens for any client use of the command `jointeam`.
            AddCommandListener("jointeam", (player, info) =>
            {
                Log($"{player.PlayerName} just did a jointeam (pre) [{info.ArgString}]");

                return HookResult.Continue;
            });
        }

        [GameEventHandler]
        public HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            Log($"Player {@event.Name} has connected! (post)");

            return HookResult.Continue;
        }

        [GameEventHandler(HookMode.Pre)]
        public HookResult OnPlayerConnectPre(EventPlayerConnect @event, GameEventInfo info)
        {
            Log($"Player {@event.Name} has connected! (pre)");

            return HookResult.Continue;
        }

        [ConsoleCommand("css_killmeplease", "Kills the player")]
        [ConsoleCommand("css_killme", "Kills the player")]
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

        [ConsoleCommand("css_changelevel", "Changes map")]
        public void OnCommandChangeMap(CCSPlayerController? player, CommandInfo command)
        {
            var mapName = command.ArgByIndex(1);
            if (Server.IsMapValid(mapName))
            {
                Server.ExecuteCommand($"changelevel \"{mapName}\"");
            }
            else
            {
                player.PrintToChat($"Level  \"{mapName}\" is invalid.");
            }
        }

        [ConsoleCommand("css_guns", "List guns")]
        public void OnCommandGuns(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            foreach (var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
            {
                // We don't currently have a `ReplyToCommand` equivalent so just print to chat for now.
                player.PrintToChat(weapon.Value.DesignerName);
            }
        }

        [ConsoleCommand("css_pause", "Pause Game")]
        public void OnCommandPause(CCSPlayerController? player, CommandInfo command)
        {
            Log("Pause");
        }

        [ConsoleCommand("css_give", "Give named item")]
        public void OnCommandGive(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            player.GiveNamedItem(command.ArgByIndex(1));
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