/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public static class MenuManager
{
    private static readonly Dictionary<IntPtr, IMenuInstance> ActiveMenus = new();

    public static Dictionary<IntPtr, IMenuInstance> GetActiveMenus()
    {
        return ActiveMenus;
    }
    
    public static IMenuInstance? GetActiveMenu(CCSPlayerController player)
    {
        return !ActiveMenus.TryGetValue(player.Handle, out var value) ? null : value;
    }
    
    public static void CloseActiveMenu(CCSPlayerController player)
    {
        if (ActiveMenus.TryGetValue(player.Handle, out var activeMenu))
        {
            activeMenu.Reset();
        }

        ActiveMenus.Remove(player.Handle);
    }

    public static void OpenChatMenu(CCSPlayerController player, ChatMenu menu)
    {
        CloseActiveMenu(player);
        
        ActiveMenus[player.Handle] = new ChatMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }

    public static void OpenCenterHtmlMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
    {
        CloseActiveMenu(player);
        
        ActiveMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
        ActiveMenus[player.Handle].Display();
    }

    public static void OpenConsoleMenu(CCSPlayerController player, ConsoleMenu menu)
    {
        CloseActiveMenu(player);
        
        ActiveMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }

    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        GetActiveMenu(player)?.OnKeyPress(player, key);
    }
}
