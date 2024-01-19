using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public static class MenuManager
{
    public static readonly Dictionary<IntPtr, ChatMenuInstance> ActiveChatMenus = new();
    public static readonly Dictionary<IntPtr, CenterHtmlMenuInstance> ActiveCenterHtmlMenus = new();
    public static readonly Dictionary<IntPtr, ConsoleMenuInstance> ActiveConsoleMenus = new();
    
    public static void OpenChatMenu(CCSPlayerController player, ChatMenu menu)
    {
        if (ActiveCenterHtmlMenus.TryGetValue(player.Handle, out var centerHtmlMenu))
        {
            centerHtmlMenu.Reset();
        }
        
        if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            consoleMenu.Reset();
        }
        
        ActiveChatMenus[player.Handle] = new ChatMenuInstance(player, menu);
    }
    
    public static void OpenCenterHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            chatMenu.Reset();
        }
        
        if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            consoleMenu.Reset();
        }
        
        ActiveCenterHtmlMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
    }
    
    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            chatMenu.Reset();
        }
        
        if (ActiveCenterHtmlMenus.TryGetValue(player.Handle, out var centerHtmlMenu))
        {
            centerHtmlMenu.Reset();
        }
        
        ActiveConsoleMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
    }

    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        if (ActiveChatMenus.ContainsKey(player.Handle))
        {
            ChatMenus.OnKeyPress(player, key);
        }
        else if (ActiveCenterHtmlMenus.ContainsKey(player.Handle))
        {
            ChatMenus.OnKeyPress(player, key);
        }
        else if (ActiveConsoleMenus.ContainsKey(player.Handle))
        {
            ChatMenus.OnKeyPress(player, key);
        }
    }
}