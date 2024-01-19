using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public static class MenuManager
{
    public static readonly Dictionary<IntPtr, ChatMenuInstance> ActiveChatMenus = new();
    public static readonly Dictionary<IntPtr, CenterHtmlMenuInstance> ActiveCenterHtmlMenus = new();
    public static readonly Dictionary<IntPtr, ConsoleMenuInstance> ActiveConsoleMenus = new();

    private static void ResetMenus(CCSPlayerController player)
    {
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            chatMenu.Reset();
        }
        
        if (ActiveCenterHtmlMenus.TryGetValue(player.Handle, out var centerHtmlMenu))
        {
            centerHtmlMenu.Reset();
        }
        
        if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            consoleMenu.Reset();
        }

        ActiveChatMenus.Remove(player.Handle);
        ActiveCenterHtmlMenus.Remove(player.Handle);
        ActiveConsoleMenus.Remove(player.Handle);
    }
    
    public static void OpenChatMenu(CCSPlayerController player, ChatMenu menu)
    {
        ResetMenus(player);
        
        ActiveChatMenus[player.Handle] = new ChatMenuInstance(player, menu);
        ActiveChatMenus[player.Handle].Display();
    }
    
    public static void OpenCenterHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        ResetMenus(player);
        
        ActiveCenterHtmlMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
        ActiveCenterHtmlMenus[player.Handle].Display();
    }
    
    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        ResetMenus(player);
        
        ActiveConsoleMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
        ActiveConsoleMenus[player.Handle].Display();
    }

    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            chatMenu.OnKeyPress(player, key);
        }
        else if (ActiveCenterHtmlMenus.TryGetValue(player.Handle, out var centerHtmlMenu))
        {
            centerHtmlMenu.OnKeyPress(player, key);
        }
        else if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            consoleMenu.OnKeyPress(player, key);
        }
    }
}
