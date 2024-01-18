using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ConsoleMenu: BaseMenu
    {
        public ConsoleMenu(string title) : base(title)
        {
        }
    }

    public class ConsoleMenuInstanceInstance: BaseMenuInstance
    {
        public ConsoleMenuInstanceInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        public override void Display()
        {
            Player?.PrintToConsole(Menu.Title);
            Player?.PrintToConsole("---");

            var keyOffset = 1;

            if (HasPrevButton)
            {
                Player?.PrintToConsole($"!1 -> Prev");
                keyOffset++;
            }

            for (var i = CurrentOffset;
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
        private static readonly Dictionary<IntPtr, ConsoleMenuInstanceInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, ConsoleMenu menu)
        {
            ActiveMenus[player.Handle] = new ConsoleMenuInstanceInstance(player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
