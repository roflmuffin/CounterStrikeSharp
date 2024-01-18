using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ChatMenuOption: IMenuOption
    {
        public Action<CCSPlayerController, IMenuOption> OnSelect { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }

        public ChatMenuOption(string text, bool disabled, Action<CCSPlayerController, IMenuOption> onSelect)
        {
            Text = text;
            Disabled = disabled;
            OnSelect = onSelect;
        }
    }

    public class ChatMenu: IMenu
    {
        public string Title { get; set; }
        public List<IMenuOption> MenuOptions { get; } = new();

        public ChatMenu(string title)
        {
            Title = title;
        }

        public IMenuOption AddMenuOption(string display, Action<CCSPlayerController, IMenuOption> onSelect, bool disabled = false)
        {
            var option = new ChatMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }

    public class ChatMenuInstance: BaseMenu
    {
        public ChatMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        private new void Display()
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
        private static readonly Dictionary<IntPtr, ChatMenuInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, IMenu menu)
        {
            ActiveMenus[player.Handle] = new ChatMenuInstance(player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.TryGetValue(player.Handle, out var value))
            {
                return;
            }
            
            value.OnKeyPress(player, key);
        }
    }
}
