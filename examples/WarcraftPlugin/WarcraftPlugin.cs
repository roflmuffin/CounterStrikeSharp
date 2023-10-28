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
using WarcraftPlugin.Cooldowns;
using WarcraftPlugin.Effects;
using WarcraftPlugin.Races;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace WarcraftPlugin
{
    public static class WarcraftPlayerExtensions
    {
        public static WarcraftPlayer GetWarcraftPlayer(this CCSPlayerController player)
        {
            return WarcraftPlugin.Instance.GetWcPlayer(player);
        }
    }

    public class XpSystem
    {
        private readonly WarcraftPlugin _plugin;

        public XpSystem(WarcraftPlugin plugin)
        {
            _plugin = plugin;
        }

        private List<int> _levelXpRequirement = new List<int>(new int[256]);

        public void GenerateXpCurve(int initial, float modifier, int maxLevel)
        {
            for (int i = 1; i <= maxLevel; i++)
            {
                if (i == 1)
                    _levelXpRequirement[i] = initial;
                else
                    _levelXpRequirement[i] = Convert.ToInt32(_levelXpRequirement[i - 1] * modifier);
            }
        }

        public int GetXpForLevel(int level)
        {
            return _levelXpRequirement[level];
        }

        public void AddXp(CCSPlayerController attacker, int xpToAdd)
        {
            var wcPlayer = _plugin.GetWcPlayer(attacker);
            if (wcPlayer == null) return;

            if (wcPlayer.GetLevel() >= WarcraftPlugin.MaxLevel) return;

            wcPlayer.currentXp += xpToAdd;

            while (wcPlayer.currentXp >= wcPlayer.amountToLevel)
            {
                wcPlayer.currentXp = wcPlayer.currentXp - wcPlayer.amountToLevel;
                GrantLevel(wcPlayer);

                if (wcPlayer.GetLevel() >= WarcraftPlugin.MaxLevel) return;
            }
        }

        public void GrantLevel(WarcraftPlayer wcPlayer)
        {
            if (wcPlayer.GetLevel() >= WarcraftPlugin.MaxLevel) return;

            wcPlayer.currentLevel += 1;

            RecalculateXpForLevel(wcPlayer);
            PerformLevelupEvents(wcPlayer);
        }

        private void PerformLevelupEvents(WarcraftPlayer wcPlayer)
        {
            if (GetFreeSkillPoints(wcPlayer) > 0)
                WarcraftPlugin.Instance.ShowSkillPointMenu(wcPlayer);

            var soundToPlay = "warcraft/ui/questcompleted.mp3";
            if (wcPlayer.currentLevel == WarcraftPlugin.MaxLevel)
                soundToPlay = "warcraft/ui/gamefound.mp3";


            // Sound.EmitSound(wcPlayer.Index, soundToPlay);
        }

        public void RecalculateXpForLevel(WarcraftPlayer wcPlayer)
        {
            if (wcPlayer.currentLevel == WarcraftPlugin.MaxLevel)
            {
                wcPlayer.amountToLevel = 0;
                return;
            }

            wcPlayer.amountToLevel = GetXpForLevel(wcPlayer.currentLevel);
        }

        public int GetFreeSkillPoints(WarcraftPlayer wcPlayer)
        {
            int totalPointsUsed = 0;

            for (int i = 0; i < 4; i++)
            {
                totalPointsUsed += wcPlayer.GetAbilityLevel(i);
            }

            int level = wcPlayer.GetLevel();
            if (level > WarcraftPlugin.MaxLevel)
                level = WarcraftPlugin.MaxSkillLevel;

            return level - totalPointsUsed;
        }
    }

    public class WarcraftPlayer
    {
        private int _playerIndex;
        public int Index => _playerIndex;
        public bool IsMaxLevel => currentLevel == WarcraftPlugin.MaxLevel;
        public CCSPlayerController GetPlayer() => Player;

        public CCSPlayerController Player { get; init; }

        public int currentXp;
        public int currentLevel;
        public int amountToLevel;
        public string raceName;
        public string statusMessage;

        private List<int> _abilityLevels = new List<int>(new int[4]);
        public List<float> AbilityCooldowns = new List<float>(new float[4]);

        private WarcraftRace _race;

        public WarcraftPlayer(CCSPlayerController player)
        {
            Player = player;
        }

        public void LoadFromDatabase(DatabaseRaceInformation dbRace, XpSystem xpSystem)
        {
            currentLevel = dbRace.CurrentLevel;
            currentXp = dbRace.CurrentXp;
            raceName = dbRace.RaceName;
            amountToLevel = xpSystem.GetXpForLevel(currentLevel);

            _abilityLevels[0] = dbRace.Ability1Level;
            _abilityLevels[1] = dbRace.Ability2Level;
            _abilityLevels[2] = dbRace.Ability3Level;
            _abilityLevels[3] = dbRace.Ability4Level;

            _race = WarcraftPlugin.Instance.raceManager.InstantiateRace(raceName);
            _race.WarcraftPlayer = this;
            _race.Player = Player;
        }

        public int GetLevel()
        {
            if (currentLevel > WarcraftPlugin.MaxLevel) return WarcraftPlugin.MaxLevel;

            return currentLevel;
        }

        public override string ToString()
        {
            return
                $"[{_playerIndex}]: {{raceName={raceName}, currentLevel={currentLevel}, currentXp={currentXp}, amountToLevel={amountToLevel}}}";
        }

        public int GetAbilityLevel(int abilityIndex)
        {
            return _abilityLevels[abilityIndex];
        }

        public void SetAbilityLevel(int abilityIndex, int value)
        {
            _abilityLevels[abilityIndex] = value;
        }

        public WarcraftRace GetRace()
        {
            return _race;
        }

        public void SetStatusMessage(string status, float duration = 2f)
        {
            statusMessage = status;
            new Timer(duration, () => statusMessage = null, 0);
            GetPlayer().PrintToChat(" " + status);
        }

        public void GrantAbilityLevel(int abilityIndex)
        {
            _abilityLevels[abilityIndex] += 1;
        }
    }

    public class WarcraftPlugin : BasePlugin
    {
        private static WarcraftPlugin _instance;
        public static WarcraftPlugin Instance => _instance;

        public override string ModuleName => "Warcraft";
        public override string ModuleVersion => "0.0.1";

        public static int MaxLevel = 16;
        public static int MaxSkillLevel = 5;
        public static int maxUltimateLevel = 5;

        private Dictionary<IntPtr, WarcraftPlayer> WarcraftPlayers = new();
        private EventSystem _eventSystem;
        public XpSystem XpSystem;
        public RaceManager raceManager;
        public EffectManager EffectManager;
        public CooldownManager CooldownManager;
        private Database _database;

        public int XpPerKill = 20;
        public float XpHeadshotModifier = 0.25f;
        public float XpKnifeModifier = 0.25f;

        public List<WarcraftPlayer> Players => WarcraftPlayers.Values.ToList();

        public WarcraftPlayer GetWcPlayer(CCSPlayerController player)
        {
            WarcraftPlayers.TryGetValue(player.Handle, out var wcPlayer);
            if (wcPlayer == null)
            {
                if (player.IsValid && !player.IsBot)
                {
                    WarcraftPlayers[player.Handle] = _database.LoadClientFromDatabase(player, XpSystem);
                }
                else
                {
                    return null;
                }
            }

            return WarcraftPlayers[player.Handle];
        }

        public void SetWcPlayer(CCSPlayerController player, WarcraftPlayer wcPlayer)
        {
            WarcraftPlayers[player.Handle] = wcPlayer;
        }

        public override void Load(bool hotReload)
        {
            base.Load(hotReload);

            if (_instance == null) _instance = this;

            XpSystem = new XpSystem(this);
            XpSystem.GenerateXpCurve(110, 1.07f, MaxLevel);

            _database = new Database();
            raceManager = new RaceManager();
            raceManager.Initialize();

            EffectManager = new EffectManager();
            EffectManager.Initialize();

            CooldownManager = new CooldownManager();
            CooldownManager.Initialize();

            AddCommand("ability1", "ability1", Ability1Pressed);
            AddCommand("ability2", "ability2", Ability2Pressed);
            AddCommand("ability3", "ability3", Ability3Pressed);
            AddCommand("ultimate", "ultimate", Ability4Pressed);

            AddCommand("changerace", "changerace", CommandChangeRace);
            AddCommand("raceinfo", "raceinfo", CommandRaceInfo);
            AddCommand("resetskills", "resetskills", CommandResetSkills);
            AddCommand("addxp", "addxp", CommandAddXp);
            AddCommand("skills", "skills", (client, _) => ShowSkillPointMenu(GetWcPlayer(client)));

            // XpPerKill = new ConVar("wcgo_xp_kill", "20", "Base amount of xp granted for a kill",
            //     ConVarFlags.None);
            // XpHeadshotModifier = new ConVar("wcgo_xp_headshot", "0.25", "% bonus xp granted for headshot",
            //     ConVarFlags.None);
            // XpKnifeModifier =
            //     new ConVar("wcgo_xp_knife", "0.50", "% bonus xp granted for knife", ConVarFlags.None);

            RegisterListener<Listeners.OnClientConnect>(OnClientPutInServerHandler);
            RegisterListener<Listeners.OnMapStart>(OnMapStartHandler);
            RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnectHandler);

            if (hotReload)
            {
                OnMapStartHandler(null);
            }

            _eventSystem = new EventSystem(this);
            _eventSystem.Initialize();

            _database.Initialize(ModuleDirectory);
        }

        private void CommandAddXp(CCSPlayerController? client, CommandInfo commandinfo)
        {
            var wcPlayer = GetWcPlayer(client);

            if (string.IsNullOrEmpty(commandinfo.ArgByIndex(1))) return;

            var xpToAdd = Convert.ToInt32(commandinfo.ArgByIndex(1));

            XpSystem.AddXp(client, xpToAdd);
        }

        private void CommandResetSkills(CCSPlayerController? client, CommandInfo commandinfo)
        {
            var wcPlayer = GetWcPlayer(client);

            for (int i = 0; i < 4; i++)
            {
                wcPlayer.SetAbilityLevel(i, 0);
            }

            if (XpSystem.GetFreeSkillPoints(wcPlayer) > 0)
            {
                ShowSkillPointMenu(wcPlayer);
            }
        }

        private void OnClientDisconnectHandler(int slot)
        {
            var player = new CCSPlayerController(NativeAPI.GetEntityFromIndex(slot + 1));
            // No bots, invalid clients or non-existent clients.
            if (!player.IsValid || player.IsBot) return;

            player.GetWarcraftPlayer().GetRace().PlayerChangingToAnotherRace();
            SetWcPlayer(player, null);
            _database.SaveClientToDatabase(player);
        }

        private void OnMapStartHandler(string mapName)
        {
            AddTimer(0.25f, StatusUpdate, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(60.0f, _database.SaveClients, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);

            // StringTables.AddFileToDownloadsTable("sound/warcraft/ui/questcompleted.mp3");
            // StringTables.AddFileToDownloadsTable("sound/warcraft/ui/gamefound.mp3");
            //
            // Server.PrecacheSound("warcraft/ui/questcompleted.mp3");
            // Server.PrecacheSound("warcraft/ui/gamefound.mp3");
            //
            // Server.PrecacheModel("models/weapons/w_ied.mdl");
            // Server.PrecacheSound("weapons/c4/c4_click.wav");
            // Server.PrecacheSound("weapons/hegrenade/explode3.wav");
            // Server.PrecacheSound("items/battery_pickup.wav");

            Server.PrintToConsole("Map Load Warcraft\n");
        }

        private void StatusUpdate()
        {
            var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
            foreach (var player in playerEntities)
            {
                if (!player.IsValid || !player.PawnIsAlive) continue;

                var wcPlayer = GetWcPlayer(player);

                if (wcPlayer == null) continue;

                var message = $"{wcPlayer.GetRace().DisplayName} ({wcPlayer.currentLevel})\n" +
                              (wcPlayer.IsMaxLevel
                                  ? ""
                                  : $"Experience: {wcPlayer.currentXp}/{wcPlayer.amountToLevel}\n") +
                              $"{wcPlayer.statusMessage}";

                player.PrintToCenter(message);
            }
        }

        private void OnClientPutInServerHandler(int slot, string name, string ipAddress)
        {
            var player = new CCSPlayerController(NativeAPI.GetEntityFromIndex(slot + 1));
            Console.WriteLine($"Put in server {player.Handle}");
            // No bots, invalid clients or non-existent clients.
            if (!player.IsValid || player.IsBot) return;

            if (!_database.ClientExistsInDatabase(player.SteamID))
            {
                _database.AddNewClientToDatabase(player);
            }

            WarcraftPlayers[player.Handle] = _database.LoadClientFromDatabase(player, XpSystem);

            Console.WriteLine("Player just connected: " + WarcraftPlayers[player.Handle]);
        }

        private void CommandRaceInfo(CCSPlayerController? client, CommandInfo commandinfo)
        {
            var menu = new ChatMenu("Race Information");
            var races = raceManager.GetAllRaces();
            foreach (var race in races.OrderBy(x => x.DisplayName))
            {
                menu.AddMenuOption(race.DisplayName, (player, option) =>
                {
                    player.PrintToChat("--------");
                    for (int i = 0; i < 4; i++)
                    {
                        var ability = race.GetAbility(i);
                        char color = i == 3 ? ChatColors.Gold : ChatColors.Purple;

                        player.PrintToChat(
                            $" {color}{ability.DisplayName}{ChatColors.Default}: {ability.GetDescription(0)}");
                    }

                    player.PrintToChat("--------");
                });
            }

            ChatMenus.OpenMenu(client, menu);
        }

        private void CommandChangeRace(CCSPlayerController? client, CommandInfo commandinfo)
        {
            var menu = new ChatMenu("Change Race");
            var races = raceManager.GetAllRaces();
            foreach (var race in races.OrderBy(x => x.DisplayName))
            {
                menu.AddMenuOption(race.DisplayName, ((player, option) =>
                {
                    _database.SaveClientToDatabase(player);

                    // Dont do anything if were already that race.
                    if (race.InternalName == player.GetWarcraftPlayer().raceName) return;

                    player.GetWarcraftPlayer().GetRace().PlayerChangingToAnotherRace();
                    player.GetWarcraftPlayer().raceName = race.InternalName;

                    _database.SaveCurrentRace(player);
                    _database.LoadClientFromDatabase(player, XpSystem);

                    player.GetWarcraftPlayer().GetRace().PlayerChangingToRace();

                    player.PlayerPawn.Value.CommitSuicide(false, false);
                }));
            }

            ChatMenus.OpenMenu(client, menu);
        }

        private void Ability1Pressed(CCSPlayerController? client, CommandInfo commandinfo)
        {
            GetWcPlayer(client)?.GetRace()?.InvokeAbility(0);
        }

        private void Ability2Pressed(CCSPlayerController? client, CommandInfo commandinfo)
        {
            GetWcPlayer(client)?.GetRace()?.InvokeAbility(1);
        }

        private void Ability3Pressed(CCSPlayerController? client, CommandInfo commandinfo)
        {
            GetWcPlayer(client)?.GetRace()?.InvokeAbility(2);
        }

        private void Ability4Pressed(CCSPlayerController? client, CommandInfo commandinfo)
        {
            GetWcPlayer(client)?.GetRace()?.InvokeAbility(3);
        }

        public override void Unload(bool hotReload)
        {
            base.Unload(hotReload);
        }

        public void ShowSkillPointMenu(WarcraftPlayer wcPlayer)
        {
            var menu = new ChatMenu($"Level up skills ({XpSystem.GetFreeSkillPoints(wcPlayer)} available)");
            var race = wcPlayer.GetRace();

            var handler = (CCSPlayerController player, ChatMenuOption option) =>
            {
                var wcPlayer = player.GetWarcraftPlayer();
                var abilityIndex = Convert.ToInt32(option);

                if (XpSystem.GetFreeSkillPoints(wcPlayer) > 0)
                    wcPlayer.GrantAbilityLevel(abilityIndex);

                if (XpSystem.GetFreeSkillPoints(wcPlayer) > 0)
                {
                    //Server.NextFrame(() => ShowSkillPointMenu(wcPlayer));
                    ShowSkillPointMenu(wcPlayer);
                }
            };

            for (int i = 0; i < 4; i++)
            {
                var ability = race.GetAbility(i);

                var displayString = $"{ability.DisplayName} ({wcPlayer.GetAbilityLevel(i)})";

                bool disabled = false;
                if (i == 3)
                {
                    if (wcPlayer.currentLevel < WarcraftPlugin.MaxLevel) disabled = true;
                    if (wcPlayer.GetAbilityLevel(i) >= 1) disabled = true;
                }
                else
                {
                    if (wcPlayer.GetAbilityLevel(i) >= WarcraftPlugin.MaxSkillLevel) disabled = true;
                }

                if (XpSystem.GetFreeSkillPoints(wcPlayer) == 0) disabled = true;

                var abilityIndex = i;
                menu.AddMenuOption(displayString, (player, option) =>
                {
                    var wcPlayer = player.GetWarcraftPlayer();

                    if (XpSystem.GetFreeSkillPoints(wcPlayer) > 0)
                        wcPlayer.GrantAbilityLevel(abilityIndex);

                    if (XpSystem.GetFreeSkillPoints(wcPlayer) > 0)
                    {
                        //Server.NextFrame(() => ShowSkillPointMenu(wcPlayer));
                        ShowSkillPointMenu(wcPlayer);
                    }
                }, disabled);
            }

            ChatMenus.OpenMenu(wcPlayer.GetPlayer(), menu);
        }
    }
}