using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Generic;
using System.Text;

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

        private bool HasPrevButton => Page > 0;
        private bool HasNextButton => (CurrentOffset + NumPerPage) < Menu.MenuOptions.Count;
        private int MenuItemsPerPage => NumPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);

        public new void Display()
        {
            if (_menu.DisplayMethod == DisplayMethod.Chat)
            {
                DisplayChat();
            }
            else if (_menu.DisplayMethod == DisplayMethod.CenterHtml)
            {
                DisplayCenterHtml();
            }
            else if (_menu.DisplayMethod == DisplayMethod.Center)
            {
                DisplayCenter();
            }
            else if (_menu.DisplayMethod == DisplayMethod.Console)
            {
                DisplayConsole();
            }
        }

        private void DisplayChat()
        {
            _player?.PrintToChat(_menu.Title);
            _player?.PrintToChat("---");

            int keyOffset = 1;

            if (HasPrevButton)
            {
                _player?.PrintToChat($" {ChatColors.Yellow}!1 {ChatColors.Default}-> Prev");
                keyOffset++;
            }

            for (int i = _currentOffset;
                 i < Math.Min(_currentOffset + MenuItemsPerPage, _menu.MenuOptions.Count);
                 i++)
            {
                var option = _menu.MenuOptions[i];

                _player?.PrintToChat(
                    $" {(option.Disabled ? ChatColors.Grey : ChatColors.Green)} !{keyOffset++} {ChatColors.Default}{option.Text}");
            }

            if (HasNextButton)
            {
                _player?.PrintToChat($" {ChatColors.Yellow}!8 {ChatColors.Default}-> Next");
            }
        }

        private void DisplayCenterHtml()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<b><font color='yellow'>{0}</font></b>", _menu.Title);
            builder.AppendLine("<br>");

            int keyOffset = 1;

            if (HasPrevButton)
            {
                builder.AppendFormat("<font color='yellow'>!1 -> Prev</font>");
                builder.AppendLine("<br>");
                keyOffset++;
            }

            for (int i = _currentOffset; i < Math.Min(_currentOffset + MenuItemsPerPage, _menu.MenuOptions.Count); i++)
            {
                var option = _menu.MenuOptions[i];
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
            _player?.PrintToCenterHtml(currentPageText);
        }

        private void DisplayCenter()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}", _menu.Title);
            builder.AppendLine();

            int keyOffset = 1;

            if (HasPrevButton)
            {
                builder.AppendFormat("!1 -> Prev");
                builder.AppendLine();
                keyOffset++;
            }

            for (int i = _currentOffset; i < Math.Min(_currentOffset + MenuItemsPerPage, _menu.MenuOptions.Count); i++)
            {
                var option = _menu.MenuOptions[i];
                builder.AppendFormat("!{0} {1}", keyOffset++, option.Text);
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
            _player?.PrintToCenter(currentPageText);
        }

        private void DisplayConsole()
        {
            _player?.PrintToConsole(_menu.Title);
            _player?.PrintToConsole("---");

            int keyOffset = 1;

            if (HasPrevButton)
            {
                _player?.PrintToConsole($"!1 -> Prev");
                keyOffset++;
            }

            for (int i = _currentOffset;
                 i < Math.Min(_currentOffset + MenuItemsPerPage, _menu.MenuOptions.Count);
                 i++)
            {
                var option = _menu.MenuOptions[i];

                _player?.PrintToConsole(
                    $" {(option.Disabled ? "[Enabled]" : "[Disabled] - ")} !{keyOffset++} {option.Text}");
            }

            if (HasNextButton)
            {
                _player?.PrintToConsole($"!8 -> Next");
            }
        }

        private void OnTick()
        {
            if (_menu.DisplayMethod == DisplayMethod.CenterHtml)
            {
                Display();
            }
        }
    }

    public static class ChatMenus
    {
        private static readonly Dictionary<IntPtr, ChatMenuInstance> ActiveMenus = new();

        public static void OpenMenu(BasePlugin plugin, CCSPlayerController player, ChatMenu menu)
        {
            ActiveMenus[player.Handle] = new ChatMenuInstance(plugin, player, menu);
            ActiveMenus[player.Handle].Display();
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!ActiveMenus.ContainsKey(player.Handle)) return;

            ActiveMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
