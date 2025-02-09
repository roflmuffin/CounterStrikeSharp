using System.Drawing;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ScreenMenu : BaseMenu, IScreenMenu
    {
        private readonly BasePlugin _plugin;
        public Color TextColor { get; set; } = Color.OrangeRed;
        public string FontName { get; set; } = "Verdana Bold";
        public bool IsSubMenu { get; set; } = false;
        public ScreenMenu? ParentMenu { get; set; } = null;

        public ScreenMenu(string title, BasePlugin plugin) : base(title)
        {
            _plugin = plugin;
        }

        public override void Open(CCSPlayerController player)
        {
            MenuManager.OpenScreenMenu(_plugin, player, this);
        }
    }
}
