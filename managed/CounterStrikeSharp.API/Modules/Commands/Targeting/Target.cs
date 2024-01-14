using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Commands.Targeting;

public class Target
{
    private TargetType Type { get; }
    private string Raw { get; }
    private string Slug { get; }
    
    private static readonly Dictionary<string, TargetType> TargetTypeMap = new(StringComparer.OrdinalIgnoreCase)
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


    private static bool ConstTargetType(string target, out TargetType targetType)
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
                return player.PlayerPawn is { IsValid: true, Value.LifeState: (byte)LifeState_t.LIFE_ALIVE };
            case TargetType.GroupDead:
                return player.PlayerPawn is { IsValid: true, Value.LifeState: (byte)LifeState_t.LIFE_DEAD or (byte)LifeState_t.LIFE_DYING };
            case TargetType.GroupNotMe:
                return player.SteamID != caller?.SteamID;
            case TargetType.PlayerMe:
                return player.SteamID == caller?.SteamID;
            case TargetType.IdUserid:
                return player.UserId.ToString() == Slug;
            case TargetType.IdSteamEscaped:
                return ((SteamID)player.SteamID).SteamId2 == Slug;
            case TargetType.IdSteam64:
                return ((SteamID)player.SteamID).SteamId64.ToString() == Slug;
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
        
        return new TargetResult() { Players = players };
    }
}
