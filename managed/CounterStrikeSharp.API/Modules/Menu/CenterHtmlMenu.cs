using System.Collections.Generic;
using System.Text;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class CenterHtmlMenuOption: IMenuOption
    {
        public Action<CCSPlayerController, IMenuOption> OnSelect { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }

        public CenterHtmlMenuOption(string text, bool disabled, Action<CCSPlayerController, IMenuOption> onSelect)
        {
            Text = text;
            Disabled = disabled;
            OnSelect = onSelect;
        }
    }

    public class CenterHtmlMenu: IMenu
    {
        public string Title { get; set; }
        public List<IMenuOption> MenuOptions { get; } = new();

        public CenterHtmlMenu(string title)
        {
            Title = title;
        }

        public IMenuOption AddMenuOption(string display, Action<CCSPlayerController, IMenuOption> onSelect, bool disabled = false)
        {
            var option = new CenterHtmlMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }

    public class CenterHtmlMenuInstance: BaseMenu
    {
        public CenterHtmlMenuInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
        {
            plugin.RegisterListener<Core.Listeners.OnTick>(Display);
        }

        private bool HasPrevButton => Page > 0;
        private bool HasNextButton => (CurrentOffset + NumPerPage) < Menu.MenuOptions.Count;
        private int MenuItemsPerPage => NumPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);
        
        private new void Display()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<b><font color='yellow'>{0}</font></b>", Menu.Title);
            builder.AppendLine("<br>");

            int keyOffset = 1;

            if (HasPrevButton)
            {
                builder.AppendFormat("<font color='yellow'>!1 -> Prev</font>");
                builder.AppendLine("<br>");
                keyOffset++;
            }

            for (var i = CurrentOffset; i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count); i++)
            {
                var option = Menu.MenuOptions[i];
                builder.AppendFormat("<font color='red'>!{0}</font> {1}", keyOffset++, option.Text);
                builder.AppendLine("<br>");
            }

            if (HasNextButton)
            {
                builder.AppendFormat("<font color='yellow'>!8 -> Next</font>");
                builder.AppendLine("<br>");
            }

            builder.AppendFormat("<font color='red'>!9 -> Close</font>");
            builder.AppendLine("<br>");

            var currentPageText = builder.ToString();
            Player?.PrintToCenterHtml(currentPageText);
        }
    }

    public static class CenterHtmlMenus
    {
        private static readonly Dictionary<IntPtr, CenterHtmlMenuInstance> ActiveMenus = new();

        public static void OpenMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
        {
            ActiveMenus[player.Handle] = new CenterHtmlMenuInstance(plugin, player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
