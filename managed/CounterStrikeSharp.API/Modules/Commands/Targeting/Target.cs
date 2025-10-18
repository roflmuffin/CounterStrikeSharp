using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Commands.Targeting;

public class Target
{
    private TargetType Type { get; }
    private string Raw { get; }
    private string Slug { get; }

    internal CCSGameRulesProxy? _gameRulesEntity = null;

    public static readonly IReadOnlyDictionary<string, TargetType> TargetTypeMap = new Dictionary<string, TargetType>(StringComparer.OrdinalIgnoreCase)
    {
        { "@all", TargetType.GroupAll },
        { "@bots", TargetType.GroupBots },
        { "@human", TargetType.GroupHumans },
        { "@alive", TargetType.GroupAlive },
        { "@dead", TargetType.GroupDead },
        { "@!me", TargetType.GroupNotMe },
        { "@me", TargetType.PlayerMe },
        { "@aim", TargetType.PlayerAim },
        { "@ct", TargetType.TeamCt },
        { "@t", TargetType.TeamT },
        { "@spec", TargetType.TeamSpec }
    }.ToFrozenDictionary();


    private static bool ConstTargetType(string target, out TargetType targetType)
    {
        targetType = TargetType.Invalid;
        if (!target.StartsWith('@'))
        {
            return false;
        }

        return TargetTypeMap.TryGetValue(target, out targetType);
    }

    private bool IdTargetType(string target,
        out TargetType targetType,
        [MaybeNullWhen(false)] out string slug)
    {
        targetType = TargetType.Invalid;
        slug = null!;
        if (!target.StartsWith('#'))
        {
            return false;
        }

        slug = target.TrimStart('#');
        if (slug.StartsWith("STEAM") && slug.Contains(':')) targetType = TargetType.IdSteamEscaped;
        else if (!ulong.TryParse(slug, out _)) targetType = TargetType.ExplicitName;
        else if (slug.Length == 17) targetType = TargetType.IdSteam64;
        else targetType = TargetType.IdUserid;

        return true;
    }


    public Target(string target)
    {
        Raw = target.Trim();
        if (ConstTargetType(Raw, out var targetType))
        {
            Type = targetType;
            Slug = Raw;
        }
        else if (IdTargetType(Raw, out targetType, out var slug))
        {
            Type = targetType;
            Slug = slug;
        }
        else
        {
            Type = TargetType.ImplicitName;
            Slug = Raw;
        }
    }

    private bool TargetPredicate(CCSPlayerController player, CCSPlayerController? caller, CCSGameRules? gameRules = null)
    {
        return Type switch
        {
            TargetType.PlayerAim => caller != null && player == gameRules!.GetClientAimTarget(caller),
            TargetType.TeamCt => player.Team == CsTeam.CounterTerrorist,
            TargetType.TeamT => player.Team == CsTeam.Terrorist,
            TargetType.TeamSpec => player.Team == CsTeam.Spectator,
            TargetType.GroupAll => !player.IsHLTV,
            TargetType.GroupBots => player.IsBot,
            TargetType.GroupHumans => !player.IsBot && !player.IsHLTV,
            TargetType.GroupAlive => player.PlayerPawn is { IsValid: true, Value.LifeState: (byte)LifeState_t.LIFE_ALIVE },
            TargetType.GroupDead => player.PlayerPawn is { IsValid: true, Value.LifeState: (byte)LifeState_t.LIFE_DEAD or (byte)LifeState_t.LIFE_DYING },
            TargetType.GroupNotMe => player.SteamID != caller?.SteamID,
            TargetType.PlayerMe => player.SteamID == caller?.SteamID,
            TargetType.IdUserid => player.UserId.ToString() == Slug,
            TargetType.IdSteamEscaped when player.SteamID != 0 => (SteamID)player.SteamID == (SteamID)Slug,
            TargetType.IdSteam64 => player.SteamID.ToString() == Slug,
            TargetType.ExplicitName or TargetType.ImplicitName => player.PlayerName.Contains(Slug, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    public TargetResult GetTarget(CCSPlayerController? caller)
    {
        if (Type == TargetType.PlayerAim)
        {
            if (_gameRulesEntity == null || !_gameRulesEntity.IsValid)
            {
                _gameRulesEntity = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
            }
        }

        return new TargetResult() { Players = Utilities.GetPlayers().Where(player => TargetPredicate(player, caller, _gameRulesEntity?.GameRules)).ToList() };
    }

    /// <summary>
    /// Processes a target string, finds matching players, and applies specified filters.
    /// </summary>
    /// <param name="player">The player who executed the command.</param>
    /// <param name="targetString">The target string (e.g., player name, #userid, @all).</param>
    /// <param name="filter">Flags to filter the found targets.</param>
    /// <param name="tnIsMl">If true, the target name buffer will be an ML phrase. Otherwise, it will be normal string.</param>
    /// <param name="targetname">
    /// An output list that will contain the resolved target names. These may be localization keys 
    /// (e.g., "all", "ct") if <paramref name="tnIsMl"/> is true, or actual player names otherwise.
    /// </param>
    /// <param name="players">An output list that will be populated with the player entities matching the target string.</param>
    public static ProcessTargetResultFlag ProcessTargetString(CCSPlayerController? player,
        string targetString, ProcessTargetFilterFlag filter, bool tnIsMl,
        out string targetname, out List<CCSPlayerController> players)
    {
        targetname = string.Empty;
        players = new Target(targetString).GetTarget(player).Players;

        if (players.Count == 0)
        {
            return ProcessTargetResultFlag.TargetNone;
        }

        if (players.Count > 1 && filter.HasFlag(ProcessTargetFilterFlag.FilterNoMulti))
        {
            return ProcessTargetResultFlag.TargetAmbiguous;
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterNoImmunity))
        {
            players.RemoveAll(target => player != null && !AdminManager.CanPlayerTarget(new SteamID(player.SteamID), new SteamID(target.SteamID)));
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetImmune;
            }
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterNoBots))
        {
            players.RemoveAll(p => p.IsBot);
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetNotHuman;
            }
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterAlive))
        {
            players.RemoveAll(p => p.PlayerPawn.Value?.LifeState != (byte)LifeState_t.LIFE_ALIVE);
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetNotAlive;
            }
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterDead))
        {
            players.RemoveAll(p => p.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE);
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetNotDead;
            }
        }

        if (tnIsMl)
        {
            if (!TargetTypeMap.TryGetValue(targetString, out TargetType type))
                type = TargetType.PlayerMe;

            targetname = type switch
            {
                TargetType.GroupAll => "all",
                TargetType.GroupBots => "bots",
                TargetType.GroupHumans => "humans",
                TargetType.GroupAlive => "alive",
                TargetType.GroupDead => "dead",
                TargetType.GroupNotMe => "notme",
                TargetType.TeamCt => "ct",
                TargetType.TeamT => "t",
                TargetType.TeamSpec => "spec",
                _ => players[0].PlayerName
            };
        }
        else
        {
            targetname = string.Join(", ", players.Select(p => p.PlayerName));
        }

        return ProcessTargetResultFlag.TargetFound;
    }

    /// <summary>
    /// Wraps ProcessTargetString() and handles producing error messages for bad targets.
    /// </summary>
    /// <param name="player">The player who executed the command.</param>
    /// <param name="targetString">The target string (e.g., player name, #userid, @all).</param>
    /// <param name="nobots">Optional. Set to true if bots should NOT be targetted</param>
    /// <param name="immunity">Optional. Set to false to ignore target immunity.</param>
    public static CCSPlayerController? FindTarget(CCSPlayerController player, string targetString, bool nobots = false, bool immunity = true)
    {
        var filter = ProcessTargetFilterFlag.FilterNoMulti;

        if (nobots)
            filter |= ProcessTargetFilterFlag.FilterNoBots;

        if (!immunity)
            filter |= ProcessTargetFilterFlag.FilterNoImmunity;

        ProcessTargetResultFlag result;
        if ((result = ProcessTargetString(player, targetString, filter, false, out var targetname, out var players)) == ProcessTargetResultFlag.TargetFound)
            return players[0];

        ReplyToTargetError(player, result);
        return null;
    }


    /// <summary>
    /// Replies to a client with a given message describing a targetting failure reason.
    /// </summary>
    /// <param name="player">The player who executed the command.</param>
    /// <param name="resultFlag">The <see cref="ProcessTargetResultFlag"/> value indicating why it is failed.</param>
    public static void ReplyToTargetError(CCSPlayerController player, ProcessTargetResultFlag resultFlag)
    {
        switch (resultFlag)
        {
            case ProcessTargetResultFlag.TargetNone:
                player.PrintToChat(Application.Localizer["No matching client"]);
                break;
            case ProcessTargetResultFlag.TargetEmptyFilter:
                player.PrintToChat(Application.Localizer["No matching clients"]);
                break;
            case ProcessTargetResultFlag.TargetNotAlive:
                player.PrintToChat(Application.Localizer["Target must be alive"]);
                break;
            case ProcessTargetResultFlag.TargetNotDead:
                player.PrintToChat(Application.Localizer["Target must be dead"]);
                break;
            case ProcessTargetResultFlag.TargetImmune:
                player.PrintToChat(Application.Localizer["Unable to target"]);
                break;
            case ProcessTargetResultFlag.TargetNotHuman:
                player.PrintToChat(Application.Localizer["Cannot target bot"]);
                break;
            case ProcessTargetResultFlag.TargetAmbiguous:
                player.PrintToChat(Application.Localizer["More than one client matched"]);
                break;
        }
    }
}
