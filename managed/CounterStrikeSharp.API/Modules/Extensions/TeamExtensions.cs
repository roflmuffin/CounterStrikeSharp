using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Extensions;

public static class TeamExtensions
{
    /// <summary>
    /// <inheritdoc cref="ChatColors.ForTeam"/>
    /// </summary>
    /// <returns></returns>
    public static char GetChatColor(this CsTeam team)
    {
        return ChatColors.ForTeam(team);
    }
}