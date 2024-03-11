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

using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Menu;

public class ChatMenu : BaseMenu
{
    public ChatMenu(string title) : base(title)
    {
        ExitButton = false;
    }
}

public class ChatMenuInstance : BaseMenuInstance
{
    public ChatMenuInstance(CCSPlayerController player, ChatMenu menu) : base(player, menu)
    {
    }

    public override void Display()
    {
        Player.PrintToChat(Menu.Title);
        Player.PrintToChat("---");

        var keyOffset = 1;
        for (var i = CurrentOffset; i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count); i++)
        {
            var option = Menu.MenuOptions[i];

            Player.PrintToChat($" {(option.Disabled ? ChatColors.Grey : ChatColors.Green)} !{keyOffset++} {ChatColors.Default}{option.Text}");
        }

        if (HasPrevButton)
        {
            Player.PrintToChat($" {ChatColors.Yellow}!7 {ChatColors.Default}-> Prev");
        }
            
        if (HasNextButton)
        {
            Player.PrintToChat($" {ChatColors.Yellow}!8 {ChatColors.Default}-> Next");
        }

        if (Menu.ExitButton)
        {
            Player.PrintToChat($" {ChatColors.Red}!9 {ChatColors.Default}-> Close");
        }
    }
}
    
public static class ChatMenus
{
    [Obsolete("Use MenuManager.OpenChatMenu instead")]
    public static void OpenMenu(CCSPlayerController player, ChatMenu menu)
    {
        MenuManager.OpenChatMenu(player, menu);
    }

    [Obsolete("Use MenuManager.OnKeyPress instead")]
    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        MenuManager.OnKeyPress(player, key);
    }
}