using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ConsoleMenuOption: IMenuOption
    {
        public Action<CCSPlayerController, IMenuOption> OnSelect { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }

        public ConsoleMenuOption(string text, bool disabled, Action<CCSPlayerController, IMenuOption> onSelect)
        {
            Text = text;
            Disabled = disabled;
            OnSelect = onSelect;
        }
    }

    public class ConsoleMenu: IMenu
    {
        public string Title { get; set; }
        public List<IMenuOption> MenuOptions { get; } = new();

        public ConsoleMenu(string title)
        {
            Title = title;
        }

        public IMenuOption AddMenuOption(string display, Action<CCSPlayerController, IMenuOption> onSelect, bool disabled = false)
        {
            var option = new ConsoleMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }

    public class ConsoleMenuInstance: BaseMenu
    {
        public ConsoleMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        private new void Display()
        {
            Player?.PrintToConsole(Menu.Title);
            Player?.PrintToConsole("---");

            int keyOffset = 1;

            if (HasPrevButton)
            {
                Player?.PrintToConsole($"!1 -> Prev");
                keyOffset++;
            }

            for (int i = CurrentOffset;
                 i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
                 i++)
            {
                var option = Menu.MenuOptions[i];

                Player?.PrintToConsole(
                    $" {(option.Disabled ? "[Enabled]" : "[Disabled] - ")} !{keyOffset++} {option.Text}");
            }

            if (HasNextButton)
            {
                Player?.PrintToConsole($"!8 -> Next");
            }
        }
    }

    public static class ConsoleMenus
    {
        private static readonly Dictionary<IntPtr, ConsoleMenuInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, ConsoleMenu menu)
        {
            ActiveMenus[player.Handle] = new ConsoleMenuInstance(player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
