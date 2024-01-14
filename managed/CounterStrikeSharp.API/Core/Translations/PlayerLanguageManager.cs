using System.Collections.Concurrent;
using System.Globalization;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Core.Translations;

public class PlayerLanguageManager : IPlayerLanguageManager
{
    private readonly ConcurrentDictionary<SteamID, CultureInfo> _playerLanguages = new();
    
    public static IPlayerLanguageManager Instance { get; private set; } = null!;

    public PlayerLanguageManager()
    {
        Instance = this;
    }
    
    public void SetLanguage(SteamID steamId, CultureInfo cultureInfo)
    {
        _playerLanguages[steamId] = cultureInfo;
    }

    public CultureInfo GetLanguage(SteamID steamId)
    {
        return _playerLanguages.TryGetValue(steamId, out var cultureInfo) ? cultureInfo : GetDefaultLanguage();
    }

    public CultureInfo GetDefaultLanguage()
    {
        return CultureInfo.DefaultThreadCurrentUICulture!;
    }
}