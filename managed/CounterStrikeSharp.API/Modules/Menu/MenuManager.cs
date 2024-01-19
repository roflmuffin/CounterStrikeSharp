using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public static class MenuManager
{
    public static readonly Dictionary<IntPtr, IMenuInstance> ActiveMenus = new();

    private static void ResetMenus(CCSPlayerController player)
    {
        if (ActiveMenus.TryGetValue(player.Handle, out var activeMenu))
        {
            activeMenu.Reset();
        }

        ActiveMenus.Remove(player.Handle);
    }
    
    public static void OpenChatMenu(CCSPlayerController player, ChatMenu menu)
    {
        ResetMenus(player);
        
        ActiveMenus[player.Handle] = new ChatMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }
    
    public static void OpenCenterHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        ResetMenus(player);
        
        ActiveMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
        ActiveMenus[player.Handle].Display();
    }
    
    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        ResetMenus(player);
        
        ActiveMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }

    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        if (ActiveMenus.TryGetValue(player.Handle, out var activeMenu))
        {
            activeMenu.OnKeyPress(player, key);
        }
    }
}
