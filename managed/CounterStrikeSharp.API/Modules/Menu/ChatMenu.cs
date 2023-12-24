using System.Collections.Generic;
using System.Text;
using static CounterStrikeSharp.API.Core.Listeners;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ChatMenuOption
    {
        public ChatMenuOption(string text, bool disabled, Action<CCSPlayerController, ChatMenuOption> onSelect)
        {
            Text = text;
            Disabled = disabled;
            OnSelect = onSelect;
        }

        public Action<CCSPlayerController, ChatMenuOption> OnSelect { get; set; }

        public string Text { get; set; }
        public bool Disabled { get; set; }
    }

    public class ChatMenu
    {
        public string Title { get; set; }
        public List<ChatMenuOption> MenuOptions { get; } = new();

        public ChatMenu(string title)
        {
            Title = title;
        }

        public ChatMenuOption AddMenuOption(string display, Action<CCSPlayerController, ChatMenuOption> onSelect, bool disabled = false)
        {
            var option = new ChatMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }

    public class ChatMenuInstance : BasePlugin
    {
        readonly int _numPerPage = 4;
        private readonly Stack<int> _prevPageOffsets = new();
        private readonly ChatMenu _menu;
        private Stack<ChatMenu> _menuStack = new Stack<ChatMenu>();

        int _page = 0;
        int _currentOffset = 0;
        private CCSPlayerController? _player;

        public ChatMenuInstance(CCSPlayerController player, ChatMenu menu)
        {
            _menu = menu;
            _player = player;

            RegisterListener<OnTick>(OnTick);
            Display();
        }

        private bool HasPrevButton => _page > 0;
        private bool HasNextButton => (_currentOffset + _numPerPage) < _menu.MenuOptions.Count;
        private int MenuItemsPerPage => _numPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);

        public override string ModuleName => throw new NotImplementedException();

        public override string ModuleVersion => throw new NotImplementedException();

        private void OnTick()
        {
            Display();
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
            if (_prevPageOffsets.Count > 0)
            {
                _page--;
                _currentOffset = _prevPageOffsets.Pop();
                Display();
            }
        }

        private void Display()
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
    }

    public static class ChatMenus
    {
        private static readonly Dictionary<int, ChatMenuInstance> ActiveMenus = new();

        public static void OpenMenu(CCSPlayerController player, ChatMenu menu)
        {
            ActiveMenus[(int)player.Handle] = new ChatMenuInstance(player, menu);
        }

        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (ActiveMenus.TryGetValue((int)player.Handle, out var activeMenu))
            {
                activeMenu.OnKeyPress(player, key);
            }
        }
    }
}
