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
using CounterStrikeSharp.API.Modules.Entities;
using Dapper;
using Microsoft.Data.Sqlite;

namespace WarcraftPlugin
{
    public class Database
    {
        private SqliteConnection _connection;

        public void Initialize(string directory)
        {
            _connection =
                new SqliteConnection(
                    $"Data Source={Path.Join(directory, "database.db")}");

            _connection.Execute(@"
CREATE TABLE IF NOT EXISTS `players` (
	`steamid` UNSIGNED BIG INT NOT NULL,
	`currentRace` VARCHAR(32) NOT NULL DEFAULT 'undead_scourge',
  `name` VARCHAR(64),
	PRIMARY KEY (`steamid`));");

            _connection.Execute(@"
CREATE TABLE IF NOT EXISTS `raceinformation` (
  `steamid` UNSIGNED BIG INT NOT NULL,
  `racename` VARCHAR(32) NOT NULL,
  `currentXP` INT NULL DEFAULT 0,
  `currentLevel` INT NULL DEFAULT 1,
  `amountToLevel` INT NULL DEFAULT 100,
  `ability1level` TINYINT NULL DEFAULT 0,
  `ability2level` TINYINT NULL DEFAULT 0,
  `ability3level` TINYINT NULL DEFAULT 0,
  `ability4level` TINYINT NULL DEFAULT 0,
  PRIMARY KEY (`steamid`, `racename`));
");
        }

        public bool ClientExistsInDatabase(ulong steamid)
        {
            return _connection.ExecuteScalar<int>("select count(*) from players where steamid = @steamid",
                new { steamid }) > 0;
        }

        public void AddNewClientToDatabase(CCSPlayerController player)
        {
            Console.WriteLine($"Adding client to database {player.SteamID}");
            _connection.Execute(@"
            INSERT INTO players (`steamid`, `currentRace`)
	        VALUES(@steamid, 'undead_scourge')",
                new { steamid = player.SteamID });
        }

        public WarcraftPlayer LoadClientFromDatabase(CCSPlayerController player, XpSystem xpSystem)
        {
            var dbPlayer = _connection.QueryFirstOrDefault<DatabasePlayer>(@"
            SELECT * FROM `players` WHERE `steamid` = @steamid",
                new { steamid = player.SteamID });

            if (dbPlayer == null)
            {
                AddNewClientToDatabase(player);
            }

            var raceInformationExists = _connection.ExecuteScalar<int>(@"
            select count(*) from `raceinformation` where steamid = @steamid AND racename = @racename",
                new { steamid = player.SteamID, racename = dbPlayer.CurrentRace }
            ) > 0;

            if (!raceInformationExists)
            {
                _connection.Execute(@"
                insert into `raceinformation` (steamid, racename)
                values (@steamid, @racename);",
                    new { steamid = player.SteamID, racename = dbPlayer.CurrentRace });
            }

            var raceInformation = _connection.QueryFirst<DatabaseRaceInformation>(@"
            SELECT * from `raceinformation` where `steamid` = @steamid AND `racename` = @racename",
                new { steamid = player.SteamID, racename = dbPlayer.CurrentRace });

            var wcPlayer = new WarcraftPlayer(player);
            wcPlayer.LoadFromDatabase(raceInformation, xpSystem);
            WarcraftPlugin.Instance.SetWcPlayer(player, wcPlayer);

            return wcPlayer;
        }

        public void SaveClientToDatabase(CCSPlayerController player)
        {
            var wcPlayer = WarcraftPlugin.Instance.GetWcPlayer(player);
            Server.PrintToConsole($"Saving {player.PlayerName} to database...");

            var raceInformationExists = _connection.ExecuteScalar<int>(@"
            select count(*) from `raceinformation` where steamid = @steamid AND racename = @racename",
                new { steamid = player.SteamID, racename = wcPlayer.raceName }
            ) > 0;

            if (!raceInformationExists)
            {
                _connection.Execute(@"
                insert into `raceinformation` (steamid, racename)
                values (@steamid, @racename);",
                    new { steamid = player.SteamID, racename = wcPlayer.raceName });
            }

            _connection.Execute(@"
UPDATE `raceinformation` SET `currentXP` = @currentXp,
 `currentLevel` = @currentLevel,
 `ability1level` = @ability1Level,
 `ability2level` = @ability2Level,
 `ability3level` = @ability3Level,
 `ability4level` = @ability4Level,
 `amountToLevel` = @amountToLevel WHERE `steamid` = @steamid AND `racename` = @racename;",
                new
                {
                    currentXp = wcPlayer.currentXp,
                    currentLevel = wcPlayer.currentLevel,
                    ability1Level = wcPlayer.GetAbilityLevel(0),
                    ability2Level = wcPlayer.GetAbilityLevel(1),
                    ability3Level = wcPlayer.GetAbilityLevel(2),
                    ability4Level = wcPlayer.GetAbilityLevel(3),
                    amountToLevel = wcPlayer.amountToLevel,
                    steamid = player.SteamID,
                    racename = wcPlayer.raceName
                });
        }

        public void SaveClients()
        {
            var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
            foreach (var player in playerEntities)
            {
                if (!player.IsValid) continue;
                
                var wcPlayer = WarcraftPlugin.Instance.GetWcPlayer(player);
                if (wcPlayer == null) continue;

                SaveClientToDatabase(player);
            }
        }

        public void SaveCurrentRace(CCSPlayerController player)
        {
            var wcPlayer = WarcraftPlugin.Instance.GetWcPlayer(player);

            _connection.Execute(@"
            UPDATE `players` SET `currentRace` = @currentRace, `name` = @name WHERE `steamid` = @steamid;",
                new
                {
                    currentRace = wcPlayer.raceName,
                    name = player.PlayerName,
                    steamid = player.SteamID
                });
        }
    }

    public class DatabasePlayer
    {
        public ulong SteamId { get; set; }
        public string CurrentRace { get; set; }
        public string Name { get; set; }
    }

    public class DatabaseRaceInformation
    {
        public ulong SteamId { get; set; }
        public string RaceName { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
        public int AmountToLevel { get; set; }
        public int Ability1Level { get; set; }
        public int Ability2Level { get; set; }
        public int Ability3Level { get; set; }
        public int Ability4Level { get; set; }
    }
}