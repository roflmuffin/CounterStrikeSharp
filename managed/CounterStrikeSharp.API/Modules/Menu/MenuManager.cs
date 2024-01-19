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
            ActiveCenterHtmlMenus.Remove(player.Handle);
        }
        
        if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            consoleMenu.Reset();
            ActiveConsoleMenus.Remove(player.Handle);
        }
        
        ActiveChatMenus[player.Handle] = new ChatMenuInstance(player, menu);
    }
    
    public static void OpenCenterHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            chatMenu.Reset();
            ActiveChatMenus.Remove(player.Handle);
        }
        
        if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            consoleMenu.Reset();
            ActiveConsoleMenus.Remove(player.Handle);
        }
        
        ActiveCenterHtmlMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
    }
    
    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            chatMenu.Reset();
            ActiveChatMenus.Remove(player.Handle);
        }
        
        if (ActiveCenterHtmlMenus.TryGetValue(player.Handle, out var centerHtmlMenu))
        {
            centerHtmlMenu.Reset();
            ActiveCenterHtmlMenus.Remove(player.Handle);
        }
        
        ActiveConsoleMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
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