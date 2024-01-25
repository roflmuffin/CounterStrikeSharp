namespace CounterStrikeSharp.API.Modules.Menu;

public class ConsoleMenu : BaseMenu
{
    public ConsoleMenu(string title) : base(title)
    {
    }
}

public class ConsoleMenuInstance : BaseMenuInstance
{
    public ConsoleMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
    {
    }

    public override void Display()
    {
        Player.PrintToConsole(Menu.Title);
        Player.PrintToConsole("---");

        var keyOffset = 1;

        for (var i = CurrentOffset;
             i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
             i++)
        {
            var option = Menu.MenuOptions[i];

            Player.PrintToConsole(
                $"{(option.Disabled ? "[DISABLE]":"[ENABLE]")}[!{keyOffset++}] {option.Text}");
        }

        if (HasPrevButton)
        {
            Player.PrintToConsole("[!7] -> Prev");
        }
            
        if (HasNextButton)
        {
            Player.PrintToConsole("[!8] -> Next");
        }
        
        Player.PrintToConsole("[!9] -> Exit");
    }
}