using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Menu;
public class ChatMenu : BaseMenu
{
    public ChatMenu(string title) : base(title)
    {
            
    }
}

public class ChatMenuInstance : BaseMenuInstance
{
    
    public ChatMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
    {
        
    }

    public override void Display()
    {
        Player.PrintToChat(Menu.Title);
        Player.PrintToChat("---");

        var keyOffset = 1;

        for (var i = CurrentOffset;
             i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
             i++)
        {
            var option = Menu.MenuOptions[i];

            Player.PrintToChat(
                $" {(option.Disabled ? ChatColors.Grey : ChatColors.Green)} [!{keyOffset++}] {ChatColors.Default}{option.Text}");
        }

        if (HasPrevButton)
        {
            Player.PrintToChat($" {ChatColors.Yellow}[!7] {ChatColors.Default}-> Prev");
        }
            
        if (HasNextButton)
        {
            Player.PrintToChat($" {ChatColors.Yellow}[!8] {ChatColors.Default}-> Next");
        }
        
        Player.PrintToChat($" {ChatColors.Red}[!9] {ChatColors.Default}-> Exit");
    }
}