using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Extensions;

public static class PlayerExtensions
{
    /// <summary>
    /// <inheritdoc cref="ChatColors.ForPlayer"/>
    /// </summary>
    public static char GetChatColor(this CCSPlayerController player)
    {
        return ChatColors.ForPlayer(player);
    }
}