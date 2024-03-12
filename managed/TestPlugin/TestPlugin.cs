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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

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

        public SampleConfig Config { get; set; }

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
            SetupMenus();
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

            VirtualFunctions.CBaseTrigger_StartTouchFunc.Hook(h =>
            {
                var trigger = h.GetParam<CBaseTrigger>(0);
                var entity = h.GetParam<CBaseEntity>(1);
                
                Logger.LogInformation("Trigger {Trigger} touched by {Entity}", trigger.DesignerName, entity.DesignerName);
                
                return HookResult.Continue;
            }, HookMode.Post);
            
            VirtualFunctions.CBaseTrigger_EndTouchFunc.Hook(h =>
            {
                var trigger = h.GetParam<CBaseTrigger>(0);
                var entity = h.GetParam<CBaseEntity>(1);
                
                Logger.LogInformation("Trigger left {Trigger} by {Entity}", trigger.DesignerName, entity.DesignerName);
                
                return HookResult.Continue;
            }, HookMode.Post);
            
            VirtualFunctions.UTIL_RemoveFunc.Hook(hook =>
            {
                var entityInstance = hook.GetParam<CEntityInstance>(0);
                Logger.LogInformation("Removed entity {EntityIndex}", entityInstance.Index);

                return HookResult.Continue;
            }, HookMode.Post);
            
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook((h =>
            {
                var victim = h.GetParam<CEntityInstance>(0);
                var damageInfo = h.GetParam<CTakeDamageInfo>(1);
                
                if (damageInfo.Inflictor.Value.DesignerName == "inferno")
                {
                    var inferno = new CInferno(damageInfo.Inflictor.Value.Handle);
                    Logger.LogInformation("Owner of inferno is {Owner}",  inferno.OwnerEntity);

                    if (victim == inferno.OwnerEntity.Value)
                    {
                        damageInfo.Damage = 0;
                    }
                    else
                    {
                        damageInfo.Damage = 150;
                    }
                }

                return HookResult.Continue;
            }), HookMode.Pre);

            // Precache resources
            RegisterListener<Listeners.OnServerPrecacheResources>((manifest) =>
            {
                manifest.AddResource("path/to/model");
                manifest.AddResource("path/to/material");
                manifest.AddResource("path/to/particle");
            });
        }
        
        public override void OnAllPluginsLoaded(bool hotReload)
        {
            Logger.LogInformation("All plugins loaded!");
        }

        private void SetupConvars()
        {
            RegisterListener<Listeners.OnMapStart>(name =>
            {
                var cheatsCvar = ConVar.Find("sv_cheats");
                cheatsCvar.SetValue(true);

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
            RegisterEventHandler<EventPlayerChat>(((@event, info) =>
            {
                var entity = new CCSPlayerController(NativeAPI.GetEntityFromIndex(@event.Userid));
                if (!entity.IsValid)
                {
                    Logger.LogInformation("invalid entity");
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
            
            RegisterEventHandler<EventGrenadeBounce>((@event, info) =>
            {
                Logger.LogInformation("Player {Player} grenade bounce", @event.Userid.PlayerName);

                return HookResult.Continue;
            }, HookMode.Pre);
            RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
            {
                if (!@event.Userid.IsValid) return 0;
                if (!@event.Userid.PlayerPawn.IsValid) return 0;

                Logger.LogInformation("Player spawned with entity index: {EntityIndex} & User ID: {UserId}",
                    @event.Userid.Index, @event.Userid.UserId);

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
                    var weaponServices = player.PlayerPawn.Value.WeaponServices.As<CCSPlayer_WeaponServices>();
                    player.PrintToChat(weaponServices.ActiveWeapon.Value?.DesignerName);    
                });
                
                // Set player to random colour
                player.PlayerPawn.Value.Render = Color.FromArgb(Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255), Random.Shared.Next(0, 255));
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseModelEntity", "m_clrRender");

                activeWeapon.ReserveAmmo[0] = 250;
                activeWeapon.Clip1 = 250;

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
                    //var player = new CCSPlayerController(entInst.Handle);
                    if (player.InGameMoneyServices != null) player.InGameMoneyServices.Account = 1337;
                }

                // Grab everything starting with cs_, but we'll only mainpulate cs_gamerules.
                // Note: this iterates through all entities, so is an expensive operation.
                var csEntities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_");
                Logger.LogInformation("Amount of cs_* entities: {Count}", csEntities.Count());

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
            RegisterListener<Listeners.OnMapStart>(mapName =>
            {
                Logger.LogInformation("Map {Map} has started!", mapName);
            });
            RegisterListener<Listeners.OnMapEnd>(() => { Logger.LogInformation($"Map has ended."); });
            RegisterListener<Listeners.OnClientConnect>((index, name, ip) =>
            {
                Logger.LogInformation("Client {Name} from {Ip} has connected!", name, ip);
            });
            RegisterListener<Listeners.OnClientAuthorized>((index, id) =>
            {
                Logger.LogInformation("Client {Index} with address {Id}", index, id);
            });

            RegisterListener<Listeners.OnEntitySpawned>(entity =>
            {
                var designerName = entity.DesignerName;

                switch (designerName)
                {
                    case "smokegrenade_projectile":
                        var projectile = entity.As<CSmokeGrenadeProjectile>();

                        Server.NextFrame(() =>
                        {
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
                            Logger.LogInformation("Smoke grenade spawned with color {SmokeColor}",
                                projectile.SmokeColor);
                        });
                        return;
                    case "flashbang_projectile":
                        var flashbang = entity.As<CBaseCSGrenadeProjectile>();

                        Server.NextFrame(() => { flashbang.Remove(); });
                        return;
                }
            });
        }

        private void SetupMenus()
        {
            // [Legacy] Chat Menu Example
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

            AddCommand("css_target", "Target Test", (player, info) =>
            {
                if (player == null) return;

                var targetResult = info.GetArgTargetResult(1);

                if (!targetResult.Any())
                {
                    player.PrintToChat("No players found.");
                    return;
                }
                
                foreach (var result in targetResult.Players)
                {
                    player.PrintToChat($"Target found: {result?.PlayerName}");
                }
            });
            
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
                    Logger.LogInformation(
                        "CounterStrikeSharp - a test command was called by {SteamID2} with {Arguments}",
                        ((SteamID)player.SteamID).SteamId2, info.ArgString);
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
                Logger.LogInformation("{PlayerName} just did a jointeam (pre) [{ArgString}]", player.PlayerName,
                    info.ArgString);

                return HookResult.Continue;
            });
        }

        private void SetupEntityOutputHooks()
        {
            HookEntityOutput("weapon_knife", "OnPlayerPickup", (output, name, activator, caller, value, delay) =>
            {
                Logger.LogInformation("weapon_knife called OnPlayerPickup ({name}, {activator}, {caller}, {delay})", output.Description.Name, activator.DesignerName, caller.DesignerName, delay);

                return HookResult.Continue;
            });
            
            HookEntityOutput("*", "*", (output, name, activator, caller, value, delay) =>
            {
                Logger.LogInformation("All EntityOutput ({name}, {activator}, {caller}, {delay})", output.Description.Name, activator.DesignerName, caller.DesignerName, delay);

                return HookResult.Continue;
            });
            
            HookEntityOutput("*", "OnStartTouch", (output, name, activator, caller, value, delay) =>
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

        [ConsoleCommand("css_killmeplease", "Kills the player")]
        [ConsoleCommand("css_killme", "Kills the player")]
        public void OnKillme(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            player.PlayerPawn.Value.CommitSuicide(true, true);
        }
        
        [CommandHelper(minArgs: 1, usage: "[weaponName]")]
        [ConsoleCommand("css_strip", "Removes weapon by name")]
        public void OnStripActiveWeapon(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;
            
            player.RemoveItemByDesignerName(command.GetArg(1));
        }
        
        [ConsoleCommand("css_stripweapons", "Removes player weapons")]
        public void OnStripWeapons(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            player.RemoveWeapons();
        }
        
        [ConsoleCommand("css_teleportup", "Teleports the player up")]
        public void OnTeleport(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

            player.PlayerPawn.Value.Teleport(player.PlayerPawn.Value.AbsOrigin.With(z: player.PlayerPawn.Value.AbsOrigin.Z + 100), player.PlayerPawn.Value.AbsRotation, new Vector(IntPtr.Zero));
        }
        
        [ConsoleCommand("css_respawn", "Respawns the player")]
        public void OnRespawn(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;
            if (!player.PlayerPawn.IsValid) return;

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
                var vData = weapon.Value.As<CCSWeaponBase>().VData;
                command.ReplyToCommand(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", vData.Name, vData.GearSlot, vData.Price, vData.WeaponCategory, vData.WeaponType, vData.KillAward));
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
