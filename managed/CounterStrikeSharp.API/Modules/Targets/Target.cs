using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Targets;

public class Target
{
    private TargetType Type { get; set; }
    private string Raw { get; set; }
    private string Slug { get; set; }
    
    private static readonly Dictionary<string, TargetType> TargetTypeMap = new Dictionary<string, TargetType>(StringComparer.OrdinalIgnoreCase)
    {
        { "@all", TargetType.GroupAll },
        { "@bots", TargetType.GroupBots },
        { "@human", TargetType.GroupHumans },
        { "@alive", TargetType.GroupAlive },
        { "@dead", TargetType.GroupDead },
        { "@!me", TargetType.GroupNotMe },
        { "@me", TargetType.PlayerMe },
        { "@ct", TargetType.TeamCt },
        { "@t", TargetType.TeamT },
        { "@spec", TargetType.TeamSpec }
    };


    private static bool ConstTargetType(string target, [MaybeNullWhen(false)] out TargetType targetType)
    {
        targetType = TargetType.Invalid;
        if (!target.StartsWith("@"))
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
        if (!target.StartsWith("#"))
        {
            return false;
        }

        slug = target.TrimStart('#');
        if (slug.StartsWith("STEAM")) targetType = TargetType.IdSteamEscaped;
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

    private bool TargetPredicate(CCSPlayerController player, CCSPlayerController? caller)
    {
        switch (Type)
        {
            case TargetType.TeamCt:
                return player.TeamNum == (byte)CsTeam.CounterTerrorist;
            case TargetType.TeamT:
                return player.TeamNum == (byte)CsTeam.Terrorist;
            case TargetType.TeamSpec:
                return player.TeamNum == (byte)CsTeam.Spectator;
            case TargetType.GroupAll:
                return true;
            case TargetType.GroupBots:
                return player.IsBot;
            case TargetType.GroupHumans:
                return !player.IsBot;
            case TargetType.GroupAlive:
                return player.LifeState == (byte)LifeState_t.LIFE_ALIVE;
            case TargetType.GroupDead:
                return player.LifeState == (byte)LifeState_t.LIFE_DEAD;
            case TargetType.GroupNotMe:
                return player.SteamID != caller?.SteamID;
            case TargetType.PlayerMe:
                return player.SteamID == caller?.SteamID;
            case TargetType.IdUserid:
                return player.UserId.ToString() == Slug;
            case TargetType.IdSteamEscaped:
                return new SteamID(Slug.Replace("_", ":")).SteamId64.ToString() == Slug;
            case TargetType.IdSteam64:
                return player.SteamID.ToString() == Slug;
            case TargetType.ExplicitName:
            case TargetType.ImplicitName:
                return player.PlayerName.Contains(Slug, StringComparison.OrdinalIgnoreCase);
            default:
                return false;
        }
    }

    public TargetResult GetTarget(CCSPlayerController? caller)
    {
        var players = Utilities.GetPlayers().Where(player => TargetPredicate(player, caller)).ToList();

        var count = players.Count switch
        {
            0 => TargetResultCount.None,
            1 => TargetResultCount.Single,
            _ => TargetResultCount.Multiple
        };
        return new TargetResult() { Count = count, Players = players };
    }
}
