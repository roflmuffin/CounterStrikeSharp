using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public class MenuManager
{
    private static readonly Dictionary<IntPtr, IMenuInstance> ActiveMenus = new();

    public static void OpenChatMenu(CCSPlayerController player, ChatMenu menu)
    {
        if (ActiveMenus.TryGetValue(player.Handle, out var activeMenu)) activeMenu.Reset();
        ActiveMenus[player.Handle] = new ChatMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }
    
    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        ActiveMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }
    
    public static void OpenHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        ActiveMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
        ActiveMenus[player.Handle].Display();
    }

    public static void CloseMenu(CCSPlayerController player)
    {
        ActiveMenus[player.Handle].Reset();
    }
    
    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        if (!ActiveMenus.ContainsKey(player.Handle)) return;
    
        ActiveMenus[player.Handle].OnKeyPress(player, key);
    }
}