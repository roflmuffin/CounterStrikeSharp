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
        private readonly BasePlugin _plugin;

        public CenterHtmlMenuInstanceInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
        {
            _plugin = plugin;
            plugin.RegisterListener<Core.Listeners.OnTick>(Display);
        }
        
        public override void Display()
        {
            var builder = new StringBuilder();
            builder.Append($"<b><font color='yellow'>{Menu.Title}</font></b>");
            builder.AppendLine("<br>");

            var keyOffset = 1;

            if (HasPrevButton)
            {
                builder.AppendFormat("<font color='green'>!1</font> -> Prev");
                builder.AppendLine("<br>");
                keyOffset++;
            }

            for (var i = CurrentOffset; i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count); i++)
            {
                var option = Menu.MenuOptions[i];
                builder.Append($"<font color='green'>!{keyOffset++}</font> {option.Text}");
                builder.AppendLine("<br>");
            }

            if (HasNextButton)
            {
                builder.AppendFormat("<font color='yellow'>!8</font> -> Next");
                builder.AppendLine("<br>");
            }

            builder.AppendFormat("<font color='red'>!9</font> -> Close");
            builder.AppendLine("<br>");

            var currentPageText = builder.ToString();
            Player?.PrintToCenterHtml(currentPageText);
        }

        public override void Reset()
        {
            base.Reset();
            _plugin.RemoveListener("OnTick", Display);
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
