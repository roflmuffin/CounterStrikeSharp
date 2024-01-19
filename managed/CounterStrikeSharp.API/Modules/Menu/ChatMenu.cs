using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ChatMenu: BaseMenu
    {
        public ChatMenu(string title) : base(title)
        {
        }
    }

    public class ChatMenuInstance : BaseMenuInstance
    {
        public ChatMenuInstance(CCSPlayerController player, ChatMenu menu) : base(player, menu)
        {
        }

        public override void Display()
        {
            Player.PrintToChat(Menu.Title);
            Player.PrintToChat("---");

            var keyOffset = 1;

            if (HasPrevButton)
            {
                Player.PrintToChat($" {ChatColors.Yellow}!1 {ChatColors.Default}-> Prev");
                keyOffset++;
            }

            for (var i = CurrentOffset;
                 i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
                 i++)
            {
                var option = Menu.MenuOptions[i];

                Player.PrintToChat(
                    $" {(option.Disabled ? ChatColors.Grey : ChatColors.Green)} !{keyOffset++} {ChatColors.Default}{option.Text}");
            }

            if (HasNextButton)
            {
                Player.PrintToChat($" {ChatColors.Yellow}!8 {ChatColors.Default}-> Next");
            }
        }
    }
    
    public static class ChatMenus
    {
        [Obsolete("Use MenuManager.OpenChatMenu instead")]
        public static void OpenMenu(CCSPlayerController player, ChatMenu menu)
        {
            MenuManager.OpenChatMenu(player, menu);
        }

        [Obsolete("Use MenuManager.OnKeyPress instead")]
        public static void OnKeyPress(CCSPlayerController player, int key)
        {
            if (!MenuManager.ActiveChatMenus.ContainsKey(player.Handle)) return;

            MenuManager.ActiveChatMenus[player.Handle].OnKeyPress(player, key);
        }
    }
}
