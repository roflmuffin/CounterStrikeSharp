using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ChatMenu: BaseMenu
    {
        public ChatMenu(string title) : base(title)
        {
        }
    }

    public class ChatMenuInstanceInstance: BaseMenuInstance
    {
        public ChatMenuInstanceInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        public override void Display()
        {
            if (Player == null)
            {
                return;
            }
            
            Player.PrintToChat(Menu.Title);
            Player.PrintToChat("---");

            var keyOffset = 1;

            if (HasPrevButton)
            {
                Player.PrintToChat($" {ChatColors.Yellow}!1 {ChatColors.Default}-> Prev");
                keyOffset++;
            }

            for (var i = CurrentOffset;
                 i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
                 i++)
            {
                var option = Menu.MenuOptions[i];

                Player.PrintToChat(
                    $" {(option.Disabled ? ChatColors.Grey : ChatColors.Green)} !{keyOffset++} {ChatColors.Default}{option.Text}");
            }

            if (HasNextButton)
            {
                Player.PrintToChat($" {ChatColors.Yellow}!8 {ChatColors.Default}-> Next");
            }
        }
    }

    public static class ChatMenus
    {
        private static readonly Dictionary<IntPtr, ChatMenuInstanceInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, IMenu menu)
        {
            ActiveMenus[player.Handle] = new ChatMenuInstanceInstance(player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
