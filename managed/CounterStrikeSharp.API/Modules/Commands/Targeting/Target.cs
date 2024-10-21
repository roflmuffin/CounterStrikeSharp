using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CounterStrikeSharp.API.Core;
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
            TargetType.IdSteamEscaped => ((SteamID)player.SteamID).SteamId2 == Slug,
            TargetType.IdSteam64 => ((SteamID)player.SteamID).SteamId64.ToString() == Slug,
            TargetType.ExplicitName => player.PlayerName.Contains(Slug, StringComparison.OrdinalIgnoreCase),
            TargetType.ImplicitName => player.PlayerName.Contains(Slug, StringComparison.OrdinalIgnoreCase),
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
}
