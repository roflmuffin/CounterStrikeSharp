using System.Text;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class CenterHtmlMenu : BaseMenu
    {
        public CenterHtmlMenu(string title) : base(title)
        {
        }
    }

    public class CenterHtmlMenuInstance : BaseMenuInstance
    {
        private readonly BasePlugin _plugin;

        public CenterHtmlMenuInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
        {
            _plugin = plugin;
            RemoveOnTickListener();
            plugin.RegisterListener<Core.Listeners.OnTick>(Display);
        }
        
        public override void Display()
        {
            if (MenuManager.GetActiveMenu(Player) != this)
            {
                Reset();
                return;
            }
            
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
            Player.PrintToCenterHtml(currentPageText);
        }

        public override void Reset()
        {
            base.Reset();
            RemoveOnTickListener();
            
            // Send a blank message to clear the menu
            Player.PrintToCenterHtml(" ");
        }

        private void RemoveOnTickListener()
        {
            var onTick = new Core.Listeners.OnTick(Display);
            _plugin.RemoveListener("OnTick", onTick);
        }
    }
}
