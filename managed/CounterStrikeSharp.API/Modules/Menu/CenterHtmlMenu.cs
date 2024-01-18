using System.Collections.Generic;
using System.Text;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class CenterHtmlMenu: BaseMenu
    {
        public CenterHtmlMenu(string title) : base(title)
        {
        }
    }

    public class CenterHtmlMenuInstanceInstance: BaseMenuInstance
    {
        public CenterHtmlMenuInstanceInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
        {
            plugin.RegisterListener<Core.Listeners.OnTick>(Display);
        }
        
        public override void Display()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("<b><font color='yellow'>{0}</font></b>", Menu.Title);
            builder.AppendLine("<br>");

            var keyOffset = 1;

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
        private static readonly Dictionary<IntPtr, CenterHtmlMenuInstanceInstance> ActiveMenus = new();

        public static void OpenMenu(BasePlugin plugin, CCSPlayerController player, CenterHtmlMenu menu)
        {
            ActiveMenus[player.Handle] = new CenterHtmlMenuInstanceInstance(plugin, player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
