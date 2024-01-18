using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Generic;
using System.Text;
using static CounterStrikeSharp.API.Core.Listeners;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public enum DisplayMethod
    {
        Chat,
        CenterHtml,
        Center,
        Console
    }

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

    public class ChatMenu : IMenu
    {
        public string Title { get; set; }
        public List<IMenuOption> MenuOptions { get; } = new();
        public DisplayMethod DisplayMethod { get; private set; }

        public ChatMenu(string title, DisplayMethod displayMethod)
        {
            Title = title;
            DisplayMethod = displayMethod;
        }

        public IMenuOption AddMenuOption(string display, Action<CCSPlayerController, IMenuOption> onSelect, bool disabled = false)
        {
            var option = new ChatMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }

    public class ChatMenuInstance: IMenuInstance
    {
        private readonly int _numPerPage = 6;
        private readonly Stack<int> _prevPageOffsets = new();
        private readonly ChatMenu _menu;
        private CCSPlayerController? _player;

        int _page = 0;
        int _currentOffset = 0;

        public ChatMenuInstance(BasePlugin plugin, CCSPlayerController player, ChatMenu menu)
        {
            _menu = menu;
            _player = player;

            plugin.RegisterListener<OnTick>(OnTick);
        }

        private bool HasPrevButton => _page > 0;
        private bool HasNextButton => (_currentOffset + _numPerPage) < _menu.MenuOptions.Count;
        private int MenuItemsPerPage => _numPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);

        public void Display()
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

        internal void OnKeyPress(CCSPlayerController player, int key)
        {
            if (_player == null || player.Handle != _player.Handle) return;

            if (key == 8 && HasNextButton)
            {
                NextPage();
                return;
            }

            if (key == 1 && HasPrevButton)
            {
                PrevPage();
                return;
            }

            if (key == 9)
            {
                Reset();
                return;
            }

            var desiredValue = key;
            if (HasPrevButton) desiredValue = key - 1;

            var menuItemIndex = _currentOffset + desiredValue - 1;

            if (menuItemIndex >= 0 && menuItemIndex < _menu.MenuOptions.Count)
            {
                var menuOption = _menu.MenuOptions[menuItemIndex];

                if (!menuOption.Disabled)
                {
                    menuOption.OnSelect(_player, menuOption);
                    Reset();
                }
            }
        }

        private void OnTick()
        {
            if (_menu.DisplayMethod == DisplayMethod.CenterHtml)
            {
                Display();
            }
        }

        public void Reset()
        {
            _currentOffset = 0;
            _page = 0;
            _prevPageOffsets.Clear();
            _player = null;
        }

        public void NextPage()
        {
            _prevPageOffsets.Push(_currentOffset);
            _currentOffset += MenuItemsPerPage;
            _page++;
            Display();
        }

        public void PrevPage()
        {
            _page--;
            _currentOffset = _prevPageOffsets.Pop();
            Display();
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
