using System.Globalization;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Core.Translations;

public static class PlayerLanguageExtensions
{
    /// <summary>
    /// Returns the players configured language, as set using the "css_lang" command.
    /// </summary>
    public static CultureInfo GetLanguage(this CCSPlayerController? player)
    {
        if (player == null || !player.IsValid) return PlayerLanguageManager.Instance.GetDefaultLanguage();
        
        return PlayerLanguageManager.Instance.GetLanguage((SteamID)player.SteamID);
    }
}