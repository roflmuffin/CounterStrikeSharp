using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace WithDatabaseDapper;

[MinimumApiVersion(80)]
public class WithDatabaseDapperPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Database (Dapper)";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A plugin that reads and writes from the database.";

    private SqliteConnection _connection = null!;

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Loading database from {Path}", Path.Join(ModuleDirectory, "database.db"));
        _connection = new SqliteConnection($"Data Source={Path.Join(ModuleDirectory, "database.db")}");
        _connection.Open();

        // Create the table if it doesn't exist
        // Run in a separate thread to avoid blocking the main thread
        Task.Run(async () =>
        {
            await _connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS `players` (
	                `steamid` UNSIGNED BIG INT NOT NULL,
	                `kills` INT NOT NULL DEFAULT 0,
	                PRIMARY KEY (`steamid`));");
        });
    }

    [GameEventHandler]
    public HookResult OnPlayerKilled(EventPlayerDeath @event, GameEventInfo info)
    {
        // Don't count suicides.
        if (@event.Attacker == @event.Userid) return HookResult.Continue;

        // Capture the steamid of the player as `@event` will not be available outside of this function.
        var steamId = @event.Attacker.AuthorizedSteamID?.SteamId64;

        if (steamId == null) return HookResult.Continue;

        // Run in a separate thread to avoid blocking the main thread
        Task.Run(async () =>
        {
            // insert or update the player's kills
            await _connection.ExecuteAsync(@"
                INSERT INTO `players` (`steamid`, `kills`) VALUES (@SteamId, 1)
                ON CONFLICT(`steamid`) DO UPDATE SET `kills` = `kills` + 1;",
                new
                {
                    SteamId = steamId
                });
        });

        return HookResult.Continue;
    }

    [ConsoleCommand("css_kills", "Get count of kills for a player")]
    public void OnKillsCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;

        // Capture the SteamID of the player as `@event` will not be available outside of this function.
        var steamId = player.AuthorizedSteamID.SteamId64;

        // Run in a separate thread to avoid blocking the main thread
        Task.Run(async () =>
        {
            var result = await _connection.QueryFirstOrDefaultAsync(@"SELECT `kills` FROM `players` WHERE `steamid` = @SteamId;",
                new
                {
                    SteamId = steamId
                });

            // Print the result to the player's chat. Note that this needs to be run on the game thread.
            // So we use `Server.NextFrame` to run it on the next game tick.
            Server.NextFrame(() => { player.PrintToChat($"Kills: {result?.kills ?? 0}"); });
        });
    }
}