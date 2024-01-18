using System.Collections.Generic;
using System.Text;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class CenterMenuOption: IMenuOption
    {
        public Action<CCSPlayerController, IMenuOption> OnSelect { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }

        public CenterMenuOption(string text, bool disabled, Action<CCSPlayerController, IMenuOption> onSelect)
        {
            Text = text;
            Disabled = disabled;
            OnSelect = onSelect;
        }
    }

    public class CenterMenu: IMenu
    {
        public string Title { get; set; }
        public List<IMenuOption> MenuOptions { get; } = new();

        public CenterMenu(string title)
        {
            Title = title;
        }

        public IMenuOption AddMenuOption(string display, Action<CCSPlayerController, IMenuOption> onSelect, bool disabled = false)
        {
            var option = new CenterMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }

    public class CenterMenuInstance: BaseMenu
    {
        public CenterMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        private new void Display()
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
        private static readonly Dictionary<IntPtr, CenterMenuInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, CenterMenu menu)
        {
            ActiveMenus[player.Handle] = new CenterMenuInstance(player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
