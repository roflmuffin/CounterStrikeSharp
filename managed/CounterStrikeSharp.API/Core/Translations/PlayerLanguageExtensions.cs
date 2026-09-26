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
        // Bots and HLTV have no Steam ID, and constructing a SteamID from zero throws.
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)
        {
            return PlayerLanguageManager.Instance.GetDefaultLanguage();
        }
        
        return PlayerLanguageManager.Instance.GetLanguage((SteamID)player.SteamID);
    }
}