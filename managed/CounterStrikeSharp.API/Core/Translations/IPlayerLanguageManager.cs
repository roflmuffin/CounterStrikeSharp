using System.Globalization;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Core.Translations;

public interface IPlayerLanguageManager
{
    void SetLanguage(SteamID steamId, CultureInfo cultureInfo);
    CultureInfo GetLanguage(SteamID steamId);
    CultureInfo GetDefaultLanguage();
}