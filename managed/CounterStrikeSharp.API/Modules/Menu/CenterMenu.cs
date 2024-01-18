using System.Collections.Generic;
using System.Text;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class CenterMenu: BaseMenu
    {
        public CenterMenu(string title) : base(title)
        {
        }
    }

    public class CenterMenuInstanceInstance: BaseMenuInstance
    {
        public CenterMenuInstanceInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        public override void Display()
        {
            var builder = new StringBuilder();
            builder.Append($"{Menu.Title}");
            builder.AppendLine();

            var keyOffset = 1;

            if (HasPrevButton)
            {
                builder.AppendFormat("!1 -> Prev");
                builder.AppendLine();
                keyOffset++;
            }

            for (var i = CurrentOffset; i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count); i++)
            {
                var option = Menu.MenuOptions[i];
                builder.Append($"!{keyOffset++} {option.Text}");
                builder.AppendLine();
            }

            if (HasNextButton)
            {
                builder.AppendFormat("!8 -> Next");
                builder.AppendLine();
            }

            builder.AppendFormat("!9 -> Close");
            builder.AppendLine();

            var currentPageText = builder.ToString();
            Player?.PrintToCenter(currentPageText);
        }
    }

    public static class CenterMenus
    {
        private static readonly Dictionary<IntPtr, CenterMenuInstanceInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, CenterMenu menu)
        {
            ActiveMenus[player.Handle] = new CenterMenuInstanceInstance(player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
