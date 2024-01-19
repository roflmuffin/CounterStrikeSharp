using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public static class MenuManager
{
    public static readonly Dictionary<IntPtr, ChatMenuInstance> ActiveChatMenus = new();
    public static readonly Dictionary<IntPtr, CenterHtmlMenuInstance> ActiveCenterHtmlMenus = new();
    public static readonly Dictionary<IntPtr, ConsoleMenuInstance> ActiveConsoleMenus = new();

    private static void ResetMenus(CCSPlayerController player)
    {
        Console.WriteLine("MenuManager.ResetMenus called!");
        // If we have a menu for any of the types, reset it.
        if (ActiveChatMenus.TryGetValue(player.Handle, out var chatMenu))
        {
            Console.WriteLine("active chat menu found, calling chatMenu.Reset()");
            chatMenu.Reset();
        }
        
        if (ActiveCenterHtmlMenus.TryGetValue(player.Handle, out var centerHtmlMenu))
        {
            Console.WriteLine("active center html menu found, calling centerHtmlMenu.Reset()");
            centerHtmlMenu.Reset();
        }
        
        if (ActiveConsoleMenus.TryGetValue(player.Handle, out var consoleMenu))
        {
            Console.WriteLine("active console menu found, calling consoleMenu.Reset()");
            consoleMenu.Reset();
        }
        
        // Console.WriteLine("Removing player.Handle from all ActiveMenu dictionaries");
        // ActiveChatMenus.Remove(player.Handle);
        // ActiveCenterHtmlMenus.Remove(player.Handle);
        // ActiveConsoleMenus.Remove(player.Handle);
    }
    
    public static void OpenChatMenu(CCSPlayerController player, ChatMenu menu)
    {
        ResetMenus(player);
        
        Console.WriteLine($"Creating a new chat menu instance ActiveChatMenus[{player.Handle}]");
        ActiveChatMenus[player.Handle] = new ChatMenuInstance(player, menu);
        ActiveChatMenus[player.Handle].Display();
    }
    
    public static void OpenCenterHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        ResetMenus(player);
        
        Console.WriteLine($"Creating a new center html menu instance ActiveCenterHtmlMenus[{player.Handle}]");
        ActiveCenterHtmlMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
        ActiveCenterHtmlMenus[player.Handle].Display();
    }
    
    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        ResetMenus(player);
        
        Console.WriteLine($"Creating a new console menu instance ActiveConsoleMenus[{player.Handle}]");
        ActiveConsoleMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
        ActiveConsoleMenus[player.Handle].Display();
    }

    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        Console.WriteLine("MenuManager.OnKeyPress called!");
        
        if (ActiveChatMenus.ContainsKey(player.Handle))
        {
            Console.WriteLine("active chat menu found, calling ChatMenus.OnKeyPress");
            ChatMenus.OnKeyPress(player, key);
        }
        else if (ActiveCenterHtmlMenus.ContainsKey(player.Handle))
        {
            Console.WriteLine("active center html menu found, calling CenterHtmlMenus.OnKeyPress");
            CenterHtmlMenus.OnKeyPress(player, key);
        }
        else if (ActiveConsoleMenus.ContainsKey(player.Handle))
        {
            Console.WriteLine("active console menu found, calling ConsoleMenus.OnKeyPress");
            ConsoleMenus.OnKeyPress(player, key);
        }
        else
        {
            Console.WriteLine("No active menu found");
        }
    }
}