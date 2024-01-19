using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ConsoleMenu: BaseMenu
    {
        public ConsoleMenu(string title) : base(title)
        {
        }
    }

    public class ConsoleMenuInstance: BaseMenuInstance
    {
        public ConsoleMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
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
        
        public override void Reset()
        {
            base.Reset();
            MenuManager.ActiveConsoleMenus.Remove(Player.Handle);
        }
    }

    public static class ConsoleMenus
    {
        public static void OpenMenu(CCSPlayerController player, ConsoleMenu menu)
        {
            MenuManager.OpenConsoleMenu(player, menu);
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!MenuManager.ActiveConsoleMenus.ContainsKey(player.Handle)) return;

            MenuManager.ActiveConsoleMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
