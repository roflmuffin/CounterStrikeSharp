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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace TestPlugin
{
    public class SampleConfig : BasePluginConfig
    {
        [JsonPropertyName("IsPluginEnabled")] public bool IsPluginEnabled { get; set; } = true;

        [JsonPropertyName("LogPrefix")] public string LogPrefix { get; set; } = "CSSharp";
    }

    [MinimumApiVersion(80)]
    public class SamplePlugin : BasePlugin, IPluginConfig<SampleConfig>
    {
        public override string ModuleName => "Sample Plugin";
        public override string ModuleVersion => "v1.0.0";

        public override string ModuleAuthor => "Roflmuffin";

        public override string ModuleDescription => "A playground of features used for testing";

        public SampleConfig Config { get; set; } = null!;

        // This method is called right before `Load` is called
        public void OnConfigParsed(SampleConfig config)
        {
            // Save config instance
            Config = config;
        }

        private TestInjectedClass _testInjectedClass;

        public SamplePlugin(TestInjectedClass testInjectedClass)
        {
            _testInjectedClass = testInjectedClass;
        }

        public override void Load(bool hotReload)
        {
            // Basic usage of the configuration system
            if (!Config.IsPluginEnabled)
            {
                Logger.LogWarning($"{Config.LogPrefix} {ModuleName} is disabled");
                return;
            }

            Logger.LogInformation(
                $"Test Plugin has been loaded, and the hot reload flag was {hotReload}, path is {ModulePath}");

            VirtualFunctions.SwitchTeamFunc.Hook(hook =>
            {
                Logger.LogInformation("Switch team func called");
                return HookResult.Continue;
            }, HookMode.Pre);

            SetupConvars();
            SetupGameEvents();
            SetupListeners();
            SetupCommands();
            SetupEntityOutputHooks();

            // ValveInterface provides pointers to loaded modules via Interface Name exposed from the engine (e.g. Source2Server001)
            var server = ValveInterface.Server;
            Logger.LogInformation("Server pointer found @ {Pointer:X}", server.Pointer);

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
            Logger.LogInformation("Result of virtual func call is {Pointer:X}", result);

            _testInjectedClass.Hello();
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            Logger.LogInformation("All plugins loaded!");
        }

        private void SetupConvars()
        {
            RegisterListener<Listeners.OnMapStart>(name =>
            {
                ConVar.Find("sv_cheats")?.SetValue(true);

                var numericCvar = ConVar.Find("mp_warmuptime");
                Logger.LogInformation("mp_warmuptime = {Value}", numericCvar?.GetPrimitiveValue<float>());

                var stringCvar = ConVar.Find("sv_skyname");
                Logger.LogInformation("sv_skyname = {Value}", stringCvar?.StringValue);

                var fogCvar = ConVar.Find("fog_color");
                Logger.LogInformation("fog_color = {Value}", fogCvar?.GetNativeValue<Vector>());
            });
        }

        private void SetupGameEvents()
        {
            // Register Game Event Handlers
            RegisterEventHandler<EventPlayerConnect>(GenericEventHandler, HookMode.Pre);
            RegisterEventHandler<EventPlayerBlind>(GenericEventHandler);

            // Mirrors a chat message back to the player
            RegisterEventHandler<EventPlayerChat>(((@event, _) =>
            {
                var player = @event.Userid;
                if (player == null) return HookResult.Continue;

                player.PrintToChat($"You said {@event.Text}");
                return HookResult.Continue;
            }));

            RegisterEventHandler<EventPlayerDeath>((@event, info) =>
            {
                // You can use `info.DontBroadcast` to set the don't broadcast flag on the event.
                if (new Random().NextSingle() > 0.5f)
                {
                    @event.Attacker?.PrintToChat($"Skipping player_death broadcast at {Server.CurrentTime}");
                    info.DontBroadcast = true;
                }

                if (@event.Attacker != null)
                {
                    var message = UserMessage.FromPartialName("Shake");
                    Logger.LogInformation("Created user message CCSUsrMsg_Shake {Message:x}", message.Handle);

                    message.SetFloat("duration", 2);
                    message.SetFloat("amplitude", 5);
                    message.SetFloat("frequency", 10f);
                    message.SetInt("command", 0);

                    message.Send(@event.Attacker);
                }

                return HookResult.Continue;
            }, HookMode.Pre);

            RegisterEventHandler<EventGrenadeBounce>((@event, info) =>
            {
                Logger.LogInformation("Player \"{Player}\" grenade bounce", @event.Userid!.PlayerName);

                return HookResult.Continue;
            }, HookMode.Pre);

            RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
            {
                var player = @event.Userid;
                var playerPawn = player?.PlayerPawn.Get();
                if (player == null || playerPawn == null) return HookResult.Continue;

                Logger.LogInformation("Player spawned with entity index: {EntityIndex} & User ID: {UserId}",
                    playerPawn.Index, player.UserId);

                return HookResult.Continue;
            });

            RegisterEventHandler<EventBulletImpact>((@event, info) =>
            {
                var player = @event.Userid;
                var pawn = player?.PlayerPawn.Get();
                var activeWeapon = pawn?.WeaponServices?.ActiveWeapon.Get();

                if (pawn == null) return HookResult.Continue;

                Server.NextFrame(() =>
                {
                    player?.PrintToChat(activeWeapon?.DesignerName ?? "No Active Weapon");
                });

                // Set player to random colour
                pawn.Render = Color.FromArgb(Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255), Random.Shared.Next(0, 255));
                Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

                // Give player 5 health and set their reserve ammo to 250
                if (activeWeapon != null)
                {
                    activeWeapon.ReserveAmmo[0] = 250;
                    activeWeapon.Clip1 = 250;
                }

                pawn.Health += 5;
                Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");

                return HookResult.Continue;
            });
            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                // Grab all cs_player_controller entities and set their cash value to $1337.
                var playerEntities = Utilities.GetPlayers();
                Logger.LogInformation($"cs_player_controller count: {playerEntities.Count()}");

                foreach (var player in playerEntities)
                {
                    player.InGameMoneyServices!.Account = 1337;
                }

                // Grab everything starting with cs_, but we'll only manipulate cs_gamerules.
                // Note: this iterates through all entities, so is an expensive operation.
                var csEntities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_").ToArray();
                Logger.LogInformation("Amount of cs_* entities: {Count}", csEntities.Length);

                foreach (var entity in csEntities.Where(x => x.DesignerName == "cs_gamerules"))
                {
                    // It's safe to cast to `CCSGameRules` here as we know the entity is a cs_gamerules entity.
                    var gameRules = entity.As<CCSGameRules>();
                    gameRules.CTTimeOutActive = true;
                }

                return HookResult.Continue;
            });
        }

        private void SetupListeners()
        {
            // Precache resources
            RegisterListener<Listeners.OnServerPrecacheResources>((manifest) =>
            {
                manifest.AddResource("path/to/model");
                manifest.AddResource("path/to/material");
                manifest.AddResource("path/to/particle");
            });

            // Hook global listeners defined by CounterStrikeSharp
            RegisterListener<Listeners.OnMapStart>(mapName =>
            {
                Logger.LogInformation("Map {Map} has started!", mapName);
            });
            RegisterListener<Listeners.OnMapEnd>(() => { Logger.LogInformation($"Map has ended."); });
            RegisterListener<Listeners.OnClientConnect>((playerSlot, name, ip) =>
            {
                Logger.LogInformation("Client {Name} from {Ip} has connected!", name, ip);
            });
            RegisterListener<Listeners.OnClientAuthorized>((playerSlot, steamId) =>
            {
                Logger.LogInformation("Client {Index} with address {Id}", playerSlot, steamId);
            });

            RegisterListener<Listeners.OnEntitySpawned>(entity =>
            {
                switch (entity.DesignerName)
                {
                    case "smokegrenade_projectile":
                        var projectile = entity.As<CSmokeGrenadeProjectile>();

                        Server.NextFrame(() =>
                        {
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            projectile.SmokeColor.Y = Random.Shared.NextSingle() * 255.0f;
                            projectile.SmokeColor.Z = Random.Shared.NextSingle() * 255.0f;
                            Logger.LogInformation("Smoke grenade spawned with color {SmokeColor}",
                                projectile.SmokeColor);
                        });
                        return;
                    case "flashbang_projectile":
                        var flashbang = entity.As<CBaseCSGrenadeProjectile>();

                        // Server.NextFrame(() => { flashbang.Remove(); });
                        return;
                }
            });

            // Hide every door (prop_door_rotating) for everyone as a test
            RegisterListener<Listeners.CheckTransmit>((CCheckTransmitInfoList infoList) =>
            {
                IEnumerable<CPropDoorRotating> doors = Utilities.FindAllEntitiesByDesignerName<CPropDoorRotating>("prop_door_rotating");

                if (!doors.Any())
                    return;

                foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
                {
                    if (player == null)
                        continue;

                    foreach (CPropDoorRotating door in doors)
                    {
                        info.TransmitEntities.Remove(door);
                    }
                }
            });
        }

        private void SetupCommands()
        {
            // Adds a new server console command
            AddCommand("cssharp_info", "A test command",
                (player, info) =>
                {
                    if (player == null) return;
                    Logger.LogInformation(
                        "CounterStrikeSharp - a test command was called by {SteamID2} with {Arguments}",
                        ((SteamID)player.SteamID).SteamId2, info.ArgString);
                });

            AddCommand("css_changeteam", "change team", (player, _) =>
            {
                if (player == null) return;

                player.ChangeTeam((CsTeam)player.TeamNum == CsTeam.Terrorist ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
            });

            // Listens for any client use of the command `jointeam`.
            AddCommandListener("jointeam", (player, info) =>
            {
                Logger.LogInformation("{PlayerName} just did a jointeam (pre) [{ArgString}]", player?.PlayerName,
                    info.ArgString);

                return HookResult.Continue;
            });
        }

        private void SetupEntityOutputHooks()
        {
            HookEntityOutput("weapon_knife", "OnPlayerPickup", (output, _, activator, caller, _, delay) =>
            {
                Logger.LogInformation("weapon_knife called OnPlayerPickup ({name}, {activator}, {caller}, {delay})", output.Description.Name, activator.DesignerName, caller.DesignerName, delay);

                return HookResult.Continue;
            });

            HookEntityOutput("*", "*", (output, _, activator, caller, _, delay) =>
            {
                Logger.LogInformation("All EntityOutput ({name}, {activator}, {caller}, {delay})", output.Description.Name, activator.DesignerName, caller.DesignerName, delay);

                return HookResult.Continue;
            });

            HookEntityOutput("*", "OnStartTouch", (_, name, activator, caller, _, delay) =>
            {
                Logger.LogInformation("OnStartTouch: ({name}, {activator}, {caller}, {delay})", name, activator.DesignerName, caller.DesignerName, delay);
                return HookResult.Continue;
            });
        }

        [GameEventHandler]
        public HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            Logger.LogInformation("Player {Name} has connected! (post)", @event.Name);

            return HookResult.Continue;
        }

        [GameEventHandler(HookMode.Pre)]
        public HookResult OnPlayerConnectPre(EventPlayerConnect @event, GameEventInfo info)
        {
            Logger.LogInformation("Player {Name} has connected! (pre)", @event.Name);

            return HookResult.Continue;
        }

        [ListenerHandler<Listeners.OnClientPutInServer>]
        public void OnClientPutInServer(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);

            if (player == null || player.IsBot) return;

            player.PrintToChat("Welcome to the server!");
        }

        [ConsoleCommand("css_testinput", "Test AcceptInput and AddEntityIOEvent")]
        public void OnTestInput(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Get();
            if (pawn == null) return;

            pawn!.AcceptInput("SetHealth", null, null, "50");
            pawn!.AddEntityIOEvent("SetHealth", null, null, "75", 5);
        }

        [ConsoleCommand("css_killmeplease", "Kills the player")]
        [ConsoleCommand("css_killme", "Kills the player")]
        public void OnKillme(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Get();
            if (pawn == null) return;

            pawn.CommitSuicide(true, true);
        }

        [CommandHelper(minArgs: 1, usage: "[weaponName]")]
        [ConsoleCommand("css_strip", "Removes weapon by name")]
        public void OnStripActiveWeapon(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.PlayerPawn.Get() == null) return;

            player.RemoveItemByDesignerName(command.GetArg(1));
        }

        [ConsoleCommand("css_stripweapons", "Removes player weapons")]
        public void OnStripWeapons(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.PlayerPawn.Get() == null) return;

            player.RemoveWeapons();
        }

        [ConsoleCommand("css_teleportup", "Teleports the player up")]
        public void OnTeleport(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Get();
            if (pawn == null) return;

            pawn.Teleport(pawn.AbsOrigin!.With(z: pawn.AbsOrigin.Z + 100), pawn.AbsRotation, new Vector(IntPtr.Zero));
        }

        [ConsoleCommand("css_respawn", "Respawns the player")]
        public void OnRespawn(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.PlayerPawn.Get() == null) return;

            player.Respawn();
        }

        [ConsoleCommand("css_break", "Breaks the breakable entities")]
        public void OnBreakCommand(CCSPlayerController? player, CommandInfo command)
        {
            var entities = Utilities.FindAllEntitiesByDesignerName<CBreakable>("prop_dynamic")
                .Concat(Utilities.FindAllEntitiesByDesignerName<CBreakable>("func_breakable"));
            foreach (var entity in entities)
            {
                entity.AcceptInput("Break");
            }
        }

        [ConsoleCommand("css_fov", "Sets the player's FOV")]
        [CommandHelper(minArgs: 1, usage: "[fov]")]
        public void OnFovCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            if (!Int32.TryParse(command.GetArg(1), out var desiredFov)) return;

            player.DesiredFOV = (uint)desiredFov;
            Utilities.SetStateChanged(player, "CBasePlayerController", "m_iDesiredFOV");
        }

        [ConsoleCommand("cssharp_attribute", "This is a custom attribute event")]
        public void OnCommand(CCSPlayerController? player, CommandInfo command)
        {
            command.ReplyToCommand("cssharp_attribute called");
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
                command.ReplyToCommand($"Level  \"{mapName}\" is invalid.");
            }
        }

        [ConsoleCommand("css_guns", "List guns")]
        public void OnCommandGuns(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Get();
            if (pawn == null) return;

            foreach (var weapon in pawn.WeaponServices!.MyWeapons)
            {
                var vData = weapon.Get()?.As<CCSWeaponBase>().VData;
                if (vData == null) continue;

                command.ReplyToCommand(
                    $"{vData.Name}, {vData.GearSlot}, {vData.Price}, {vData.WeaponCategory}, {vData.WeaponType}, {vData.KillAward}");
            }
        }

        [ConsoleCommand("css_entities", "List entities")]
        public void OnCommandEntities(CCSPlayerController? player, CommandInfo command)
        {
            foreach (var entity in Utilities.GetAllEntities())
            {
                command.ReplyToCommand($"{entity.Index}:{entity.DesignerName}");
            }

            foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_"))
            {
                command.ReplyToCommand($"{entity.Index}:{entity.DesignerName}");
            }
        }

        [ConsoleCommand("css_colors", "List Chat Colors")]
        public void OnCommandColors(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            for (int i = 0; i < 16; i++)
            {
                command.ReplyToCommand($" {(char)i}Color 0x{i:x}");
            }
        }

        [ConsoleCommand("css_localetest", "Test Translations")]
        public void OnCommandLocaleTest(CCSPlayerController? player, CommandInfo command)
        {
            Logger.LogInformation("Current Culture is {Culture}", CultureInfo.CurrentCulture);
            command.ReplyToCommand(Localizer["testPlugin.maxPlayersAnnouncement", Server.MaxPlayers]);
        }

        [ConsoleCommand("css_sound", "Play a sound to client")]
        public void OnCommandSound(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            player.ExecuteClientCommand($"play sounds/ui/counter_beep.vsnd");
        }

        [ConsoleCommand("css_pause", "Pause Game")]
        public void OnCommandPause(CCSPlayerController? player, CommandInfo command)
        {
            Logger.LogInformation("Pause");
        }

        [ConsoleCommand("css_give", "Give named item")]
        public void OnCommandGive(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            player.GiveNamedItem(command.ArgByIndex(1));
        }

        [ConsoleCommand("css_giveenum", "giveenum")]
        public void OnCommandGiveEnum(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.IsValid) return;

            player.GiveNamedItem(CsItem.M4A1);
            player.GiveNamedItem(CsItem.HEGrenade);
            player.GiveNamedItem(CsItem.Kevlar);
            player.GiveNamedItem(CsItem.Tec9);
        }

        [ConsoleCommand("css_give", "give")]
        public void OnCommandGiveItems(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.IsValid) return;

            player.GiveNamedItem("weapon_m4a1");
            player.GiveNamedItem("weapon_hegrenade");
            player.GiveNamedItem("item_kevlar");
            player.GiveNamedItem("weapon_tec9");
        }

        private HookResult GenericEventHandler<T>(T @event, GameEventInfo info) where T : GameEvent
        {
            Logger.LogInformation("Event found {Pointer:X}, event name: {EventName}, dont broadcast: {DontBroadcast}",
                @event.Handle, @event.EventName, info.DontBroadcast);

            return HookResult.Continue;
        }

        [EntityOutputHook("*", "OnPlayerPickup")]
        public HookResult OnPickup(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
        {
            Logger.LogInformation("[EntityOutputHook Attribute] Called OnPlayerPickup ({name}, {activator}, {caller}, {delay})", name, activator.DesignerName, caller.DesignerName, delay);

            return HookResult.Continue;
        }
    }
}
