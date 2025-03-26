using Microsoft.Extensions.Localization;

namespace CounterStrikeSharp.API.Core.Translations;

public static class LocalizerExtensions
{
    /// <summary>
    /// Returns a localized string using the locale of the specified player.
    /// <remarks>Defaults to the server language if the player is invalid.</remarks>
    /// </summary>
    public static string ForPlayer(this IStringLocalizer localizer, CCSPlayerController? player, string key)
    {
        using WithTemporaryCulture temporaryCulture = new WithTemporaryCulture(player.GetLanguage());
        return localizer[key];
    }

    /// <summary>
    /// <inheritdoc cref="ForPlayer(Microsoft.Extensions.Localization.IStringLocalizer,CounterStrikeSharp.API.Core.CCSPlayerController?,string)"/>
    /// </summary>
    public static string ForPlayer(this IStringLocalizer localizer, CCSPlayerController? player, string key, params object[] args)
    {
        using WithTemporaryCulture temporaryCulture = new WithTemporaryCulture(player.GetLanguage());
        return localizer[key, args];
    }
}
